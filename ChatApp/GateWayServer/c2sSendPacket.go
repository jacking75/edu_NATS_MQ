package main

import (
	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

func sendLoginResult(sessionIndex int32, sessionUniqueId uint64, result int16) {
	var response LoginResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("SendLoginResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendEnterRoomResult(sessionIndex int32, sessionUniqueId uint64, result int16, roomNumber int32) {
	var response RoomEnterResPacket
	response.Result = result
	response.RoomNumber = roomNumber
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("sendEnterRoomResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendLeaveRoomResult(sessionIndex int32, sessionUniqueId uint64, result int16) {
	var response RoomLeaveResPacket
	response.Result = result
	sendPacket, _ := response.EncodingPacket()

	NetLibIPostSendToClient(sessionIndex, sessionUniqueId, sendPacket)
	NTELIB_LOG_DEBUG("sendLeaveRoomResult", zap.Int32("sessionIndex", sessionIndex), zap.Int16("result", result))
}

func sendRelayPacketToClient(mqPacket []byte) {
	reader := MakeReader(mqPacket, true)

	mqPacketHeader := MQPacketHeader{}
	DecodingMQPacketHeader(&reader, &mqPacketHeader);

	if mqPacketHeader.multiUserDataOffset == 0 {
		bodyData := mqPacket[mqPacketHeaderSize:]
		NetLibIPostSendToClient(mqPacketHeader.userNetSessionIndex,
							mqPacketHeader.userNetSessionUniqueID,
							bodyData)

		NTELIB_LOG_DEBUG("sendRelayPacketToClient", zap.Int32("sessionIndex", mqPacketHeader.userNetSessionIndex))
	} else {
		bodyData := mqPacket[mqPacketHeaderSize:mqPacketHeader.multiUserDataOffset]

		reader.Seek(int(mqPacketHeader.multiUserDataOffset));

		userCount, _ := reader.ReadU16()
		for i := uint16(0); i < userCount; i++ {
			sessionIndex, _ := reader.ReadS32()
			connSession := connectionMgrInst.getSession(sessionIndex)
			if( connSession == nil) {
				continue
			}

			NetLibIPostSendToClient(sessionIndex, connSession.getNetworkUniqueID(), bodyData)
		}
	}
}