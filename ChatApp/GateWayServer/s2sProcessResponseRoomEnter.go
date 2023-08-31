package main

import (
	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

func processMQPacketResponseRoomEnter(sessionIndex int32,
				sessionUniqueId uint64,
				dataSize int16,
				mqDataBody []byte) {

	var mqPacket MQEnterRoomResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		sendEnterRoomResult(sessionIndex, sessionUniqueId, ERROR_CODE_ENTER_ROOM_MQ_PACKET_DECODING_FAIL, -1)
		return
	}

	connSession := connectionMgrInst.getSession(sessionIndex)
	if connSession == nil {
		NTELIB_LOG_DEBUG("processMQPacketResponseRoomEnter - fail Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return;
	}

	if mqPacket.result == ERROR_CODE_NONE {
		connSession.roomEnter(mqPacket.roomNumber)
	} else {
		connSession.setBackState()
	}

	sendEnterRoomResult(sessionIndex, sessionUniqueId, mqPacket.result, mqPacket.roomNumber)
}