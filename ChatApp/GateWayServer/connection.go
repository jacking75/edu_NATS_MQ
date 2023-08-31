package main

import (
	"sync/atomic"
)

const (
	CONNECTION_STATE_NONE = 0
	CONNECTION_STATE_CONN = 1
	CONNECTION_STATE_PRE_LOGIN = 2
	CONNECTION_STATE_LOGIN = 3
	CONNECTION_STATE_PRE_ROOM = 4
	CONNECTION_STATE_ROOM = 5
)

type connection struct {
	_index int32 // 네트워크 세션ID와 동일 값

	_networkUniqueID uint64 //네트워크 세션의 유니크 ID

	_userID       [MAX_USER_ID_BYTE_LENGTH]byte
	_userIDLength int8
	_roomNumber int32

	_state int32
	_connectTimeSec int64 // 연결된 시간

	buffer []byte;
}

func (conn *connection) Init(index int32, bufSize int32) {
	conn._index = index
	conn.buffer = make([]byte, bufSize);
	conn.Clear()
}

func (conn *connection) _ClearUserId() {
	conn._userIDLength = 0
}

func (conn *connection) Clear() {
	conn._state = CONNECTION_STATE_NONE
	conn._roomNumber = -1
	conn._ClearUserId()
	conn.SetConnectTimeSec(0, 0)
}

func (conn *connection) getIndex() int32 {
	return conn._index
}

func (conn *connection) getNetworkUniqueID() uint64 {
	return atomic.LoadUint64(&conn._networkUniqueID)
}

func (conn *connection) validNetworkUniqueID(uniqueId uint64) bool {
	return atomic.LoadUint64(&conn._networkUniqueID) == uniqueId
}

func (conn *connection) GetNetworkInfo() (int32, uint64) {
	index := conn.getIndex()
	uniqueID := atomic.LoadUint64(&conn._networkUniqueID)
	return index, uniqueID
}

func (conn *connection) setUserID(userID []byte) {
	conn._userIDLength = int8(len(userID))
	copy(conn._userID[:], userID)
}

func (conn *connection) getUserID() []byte {
	return conn._userID[0:conn._userIDLength]
}

func (conn *connection) getUserIDLength() int8 {
	return conn._userIDLength
}

func (conn *connection) SetConnectTimeSec(timeSec int64, uniqueID uint64) {
	conn._state = CONNECTION_STATE_CONN
	atomic.StoreInt64(&conn._connectTimeSec, timeSec)
	atomic.StoreUint64(&conn._networkUniqueID, uniqueID)
}

func (conn *connection) GetConnectTimeSec() int64 {
	return atomic.LoadInt64(&conn._connectTimeSec)
}

func (conn *connection) setPreLogin(userID []byte) bool {
	if(atomic.CompareAndSwapInt32(&conn._state, CONNECTION_STATE_CONN, CONNECTION_STATE_PRE_LOGIN) == false) {
		return false;
	}

	conn.setUserID(userID)
	return true;
}

func (conn *connection) setLogin() {
	conn._state = CONNECTION_STATE_LOGIN
	conn._roomNumber = -1
}

func (conn *connection) setPreRoom() bool {
	if(atomic.CompareAndSwapInt32(&conn._state, CONNECTION_STATE_LOGIN, CONNECTION_STATE_PRE_ROOM) == false) {
		return false;
	}

	return true;
}

func (conn *connection) roomEnter(roomNumber int32) {
	conn._state = CONNECTION_STATE_ROOM
	conn._roomNumber = roomNumber
}

func (conn *connection) roomLeave() {
	conn._state = CONNECTION_STATE_LOGIN
	conn._roomNumber = -1
}

func (conn *connection) setBackState() {
	conn._state -= 1
}

func (conn *connection) IsAuth() bool {
	if conn._state >= CONNECTION_STATE_LOGIN {
		return true
	}

	return false
}

func (conn *connection) getRoomNumber() int32 {
	if conn._state != CONNECTION_STATE_ROOM {
		return -1
	}

	return conn._roomNumber
}

