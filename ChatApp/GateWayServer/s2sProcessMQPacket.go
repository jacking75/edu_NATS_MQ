package main

import "go.uber.org/zap"
import . "GateWayServer/gohipernetFake"

func inComingMQData(data []byte) {
	NTELIB_LOG_DEBUG("inComingMQData", zap.Int("mqDataSize", len(data)))

	var header MQPacketHeader
	reader := MakeReader(data, true)
	DecodingMQPacketHeader(&reader, &header)

	mqPacketProcess(header.userNetSessionIndex,
		header.userNetSessionUniqueID,
		header.mqPacketID,
		int16(len(data)),
		data)
}

func mqPacketProcess(sessionIndex int32, sessionUniqueId uint64,
					packetID uint16,
					packetSize int16, packet []byte) {
	bodySize := packetSize - mqPacketHeaderSize
	bodyData := packet[mqPacketHeaderSize:]

	switch packetID {
	case MQ_PACKET_ID_RES_CHAT_SERVER_MQ_SUBJECT:
		_processMQPacketResponeChatServerMQInfo(bodyData)
		break
	case MQ_PACKET_ID_DB_LOGIN_RES:
		_processMQPacketDBResponseLogin(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_ROOM_ENTER_RES:
		processMQPacketResponseRoomEnter(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_ROOM_LEAVE_RES:
		_processMQPacketResponseLeaveRoom(sessionIndex, sessionUniqueId, bodySize, bodyData)
		break
	case MQ_PACKET_ID_RELAY_RES:
		sendRelayPacketToClient(packet)
	default:
		NTELIB_LOG_ERROR("Unknown PacketId", zap.Uint16("id", packetID))
		break
	}
}


func _processMQPacketResponeChatServerMQInfo(mqDataBody []byte) {
	var mqPacket MQResChatServerMQInfo
	if (&mqPacket).Decoding(mqDataBody) == false {
		NTELIB_LOG_ERROR("[_processMQPacketResponeChatServerMQInfo] mq decoding")
		return
	}

	responseChatServerMQInfo(mqPacket);
}


func _processMQPacketDBResponseLogin(sessionIndex int32, sessionUniqueID uint64, dataSize int16, mqDataBody []byte) {
	var mqPacket MQLoginResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		NTELIB_LOG_ERROR("[_processMQPacketDBResponseLogin] mq decoding")
		return
	}


	if mqPacket.result == ERROR_CODE_NONE {
		//성공. 상태를 로그인으로 바꾼다
		if connectionMgrInst.setLogin(sessionIndex, sessionUniqueID) {
			sendLoginResult(sessionIndex, sessionUniqueID, mqPacket.result)
		}
	} else {
		session := connectionMgrInst.getSessionStrictly(sessionIndex, sessionUniqueID)
		if session == nil {
			return
		}

		session.setBackState()
		sendLoginResult(sessionIndex, sessionUniqueID, mqPacket.result)
	}
}



func _processMQPacketResponseLeaveRoom(sessionIndex int32, sessionUniqueID uint64, dataSize int16, mqDataBody []byte) {

	var mqPacket MQRoomLeaveResPacket
	if (&mqPacket).Decoding(mqDataBody) == false {
		sendLeaveRoomResult(sessionIndex, sessionUniqueID, ERROR_CODE_LEAVE_ROOM_MQ_PACKET_DECODING_FAIL)
		return
	}

	if mqPacket.result == ERROR_CODE_NONE {
		session := connectionMgrInst.getSessionStrictly(sessionIndex, sessionUniqueID)
		if session == nil {
			return
		}
		session.roomLeave()
	}

	sendLeaveRoomResult(sessionIndex, sessionUniqueID, mqPacket.result)
}

