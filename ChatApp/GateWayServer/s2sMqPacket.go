package main

import . "GateWayServer/gohipernetFake"


type MQReqChatServerMQInfo struct {
	header MQPacketHeader
}

func (req MQReqChatServerMQInfo) EncodingPacket() ([]byte, int16) {
	totalSize := mqPacketHeaderSize
	sendBuf := make([]byte, totalSize)

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, req.header)
	return sendBuf, totalSize
}


type MQResChatServerMQInfo struct {
	header MQPacketHeader
	count int16
	startRoomNums []int32
	lastRoomNums []int32
	mqServers []string
	mqSubjects []string
}

func (resp *MQResChatServerMQInfo) Decoding(bodyData []byte) bool {
	resp.startRoomNums = make([]int32, 0, 16)
	resp.lastRoomNums = make([]int32, 0, 16)
	resp.mqServers = make([]string, 0, 16)
	resp.mqSubjects = make([]string, 0, 16)

	reader := MakeReader(bodyData, true)
	resp.count, _ = reader.ReadS16()

	for i := int16(0); i < resp.count; i++ {
		startRoomNum, _ := reader.ReadS32()
		lastRoomNum, _ := reader.ReadS32()
		server, _ := reader.ReadString()
		subject, _ := reader.ReadString()

		resp.startRoomNums = append(resp.startRoomNums, startRoomNum)
		resp.lastRoomNums = append(resp.lastRoomNums, lastRoomNum)
		resp.mqServers = append(resp.mqServers, server)
		resp.mqSubjects = append(resp.mqSubjects, subject)
	}

	return true
}


type MQLoginReqPacket struct {
	header MQPacketHeader
	userID []byte
	userPW []byte
}

func (loginReq MQLoginReqPacket) EncodingPacket(sendBuf []byte) []byte {
	totalSize := mqPacketHeaderSize + MAX_USER_ID_BYTE_LENGTH + MAX_USER_PW_BYTE_LENGTH

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, loginReq.header)
	writer.WriteBytes(loginReq.userID[:])
	writer.WriteBytes(loginReq.userPW[:])
	return sendBuf[:totalSize]
}

type MQLoginResPacket struct {
	header MQPacketHeader
	result int16
}

func (loginRes *MQLoginResPacket) Decoding(bodyData []byte) bool {
	bodySize := 2
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	loginRes.result, _ = reader.ReadS16()
	return true
}


// 방 입장
type MQEnterRoomReqPacket struct {
	header MQPacketHeader
	userID []byte
	roomNumber int32
}

func (request MQEnterRoomReqPacket) EncodingPacket(sendBuf []byte) []byte {
	totalSize := mqPacketHeaderSize + MAX_USER_ID_BYTE_LENGTH + 4

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteLenAndBytes(request.userID[:])
	writer.WriteS32(request.roomNumber)
	return sendBuf[:totalSize]
}

type MQEnterRoomResPacket struct {
	header MQPacketHeader
	result int16
	roomNumber int32
}

func (response *MQEnterRoomResPacket) Decoding(bodyData []byte) bool {
	bodySize := 6
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.result, _ = reader.ReadS16()
	response.roomNumber, _ = reader.ReadS32()
	return true
}


// 방 나가기
type MQRoomLeaveReqPacket struct {
	header MQPacketHeader
	IsDisconnected int16
}

func (request MQRoomLeaveReqPacket) EncodingPacket(sendBuf []byte) []byte {
	totalSize := mqPacketHeaderSize + 2

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteS16(request.IsDisconnected)
	return sendBuf[:totalSize]
}

type MQRoomLeaveResPacket struct {
	header MQPacketHeader
	result int16
}

func (response *MQRoomLeaveResPacket) Decoding(bodyData []byte) bool {
	bodySize := 2
	if len(bodyData) != bodySize {
		return false
	}

	reader := MakeReader(bodyData, true)
	response.result, _ = reader.ReadS16()
	return true
}


// 릴레이 패킷
type MQRelayPacket struct {
	header MQPacketHeader
	relayPacket []byte
}

func (request *MQRelayPacket) EncodingPacket(sendBuf []byte) []byte {
	relayDataSize := int16(len(request.relayPacket))
	totalSize := mqPacketHeaderSize + relayDataSize

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)
	writer.WriteBytes(request.relayPacket[:])
	return sendBuf[:totalSize]
}

// 릴레이 패킷 - 방 나가기
type MQRelayPacketLeaveRoom struct {
	header MQPacketHeader
	relayPacket []byte
}

func (request *MQRelayPacketLeaveRoom) EncodingPacket(sendBuf []byte) []byte {
	relayDataSize := int16(8)
	totalSize := mqPacketHeaderSize + relayDataSize

	writer := MakeWriter(sendBuf, true)
	EncodingMQPacketHeader(&writer, request.header)

	var temp [3]byte
	writer.WriteBytes(temp[:])
	writer.WriteU16(uint16(relayDataSize))
	writer.WriteU16(uint16(PACKET_ID_ROOM_LEAVE_REQ))
	return sendBuf[:totalSize]
}