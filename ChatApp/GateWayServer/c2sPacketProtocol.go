package main

import (
	"encoding/binary"
	. "GateWayServer/gohipernetFake"
)

const (
	MAX_USER_ID_BYTE_LENGTH      = 16
	MAX_USER_PW_BYTE_LENGTH      = 16
	PACKET_HEADER_SIZE = 8
	PACKET_HEADER_TOTALSIZE_POS = 3
	PACKET_HEADER_ID_POS = 5
)

var _clientSessionHeaderSize int16
var _ServerSessionHeaderSize int16

func Init_packet() {
	_clientSessionHeaderSize = PACKET_HEADER_SIZE
	_ServerSessionHeaderSize = PACKET_HEADER_SIZE
}

func ClientHeaderSize() int16 {
	return _clientSessionHeaderSize
}

// Header의 PacketID만 읽는다
func peekPacketID(rawData []byte) uint16 {
	pos := PACKET_HEADER_ID_POS
	packetID := binary.LittleEndian.Uint16(rawData[pos:])
	return packetID
}

// 보디데이터의 참조만 가져간다
func peekPacketBody(rawData []byte) (bodySize int16, refBody []byte) {
	pos := PACKET_HEADER_TOTALSIZE_POS
	headerSize := ClientHeaderSize()
	totalSize := int16(binary.LittleEndian.Uint16(rawData[pos:]))
	bodySize = totalSize - headerSize

	if bodySize > 0 {
		refBody = rawData[headerSize:]
	}

	return bodySize, refBody
}

func EncodingPacketHeader(writer *RawPacketData, totalSize int16, pktId int16, packetType int8) {
	writer.WriteS8(0)
	writer.WriteS8(0)
	writer.WriteS8(0)
	writer.WriteS16(totalSize)
	writer.WriteS16(pktId)
	writer.WriteS8(packetType)
}


///<<< 패킷 인코딩/디코딩

// [[[ 로그인 ]]] PACKET_ID_LOGIN_REQ
type LoginReqPacket struct {
	UserID []byte
	UserPW []byte
}

func (loginReq LoginReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_LOGIN_REQ, 0)
	writer.WriteBytes(loginReq.UserID[:])
	writer.WriteBytes(loginReq.UserPW[:])
	return sendBuf, totalSize
}

func (loginReq *LoginReqPacket) Decoding(bodyData []byte) bool {
	bodySize := MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	loginReq.UserID = reader.ReadBytes(MAX_USER_ID_BYTE_LENGTH)
	loginReq.UserPW = reader.ReadBytes(MAX_USER_PW_BYTE_LENGTH)
	return true
}

type LoginResPacket struct {
	Result int16
}

func (loginRes LoginResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_LOGIN_RES, 0)
	writer.WriteS16(loginRes.Result)
	return sendBuf, totalSize
}

// [[[  ]]]   PACKET_ID_ERROR_NTF
type ErrorNtfPacket struct {
	ErrorCode int16
}

func (response ErrorNtfPacket) EncodingPacket(errorCode int16) ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ERROR_NTF, 0)
	writer.WriteS16(errorCode)
	return sendBuf, totalSize
}

func (response *ErrorNtfPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != 2 {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.ErrorCode, _ = reader.ReadS16()
	return true
}

/// [ 방 입장 ]
type RoomEnterReqPacket struct {
	RoomNumber int32
}

func (request RoomEnterReqPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + (4)
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_ENTER_REQ, 0)
	writer.WriteS32(request.RoomNumber)
	return sendBuf, totalSize
}

func (request *RoomEnterReqPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != (4) {
		return false
	}

	reader := MakeReader(bodyData, true)
	request.RoomNumber, _ = reader.ReadS32()
	return true
}

type RoomEnterResPacket struct {
	Result           int16
	RoomNumber       int32
}

func (response RoomEnterResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2 + 4
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_ENTER_RES, 0)
	writer.WriteS16(response.Result)
	writer.WriteS32(response.RoomNumber)
	return sendBuf, totalSize
}

func (response *RoomEnterResPacket) Decoding(bodyData []byte) bool {
	if len(bodyData) != (2 + 4) {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.Result, _ = reader.ReadS16()
	response.RoomNumber, _ = reader.ReadS32()
	return true
}

//<<< 방에서 나가기
type RoomLeaveResPacket struct {
	Result int16
}

func (response RoomLeaveResPacket) EncodingPacket() ([]byte, int16) {
	totalSize := _clientSessionHeaderSize + 2
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingPacketHeader(&writer, totalSize, PACKET_ID_ROOM_LEAVE_RES, 0)
	writer.WriteS16(response.Result)
	return sendBuf, totalSize
}

func (response *RoomLeaveResPacket) Decoding(bodyData []byte) bool {
	reader := MakeReader(bodyData, true)
	response.Result, _ = reader.ReadS16()
	return true
}

