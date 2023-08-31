package main

import (
	"go.uber.org/zap"

	. "GateWayServer/gohipernetFake"
)

func distributePacketInit(appConfig configAppServer) {
	_appConfig = appConfig
}

func distributePacket(sessionIndex int32, sessionUniqueId uint64, packetData []byte) {
	packetID := peekPacketID(packetData)
	bodySize, bodyData := peekPacketBody(packetData)
	NTELIB_LOG_DEBUG("distributePacket", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueId), zap.Uint16("PacketID", packetID))

	if _enableClientRequestPacketIDRange(packetID, sessionIndex) == false{
		return
	}

	connSession := connectionMgrInst.getSession(sessionIndex)
	if connSession == nil {
		NTELIB_LOG_DEBUG("distributePacket - fail Invalid sessionIndex", zap.Int32("sessionIndex", sessionIndex))
		return;
	}

	switch packetID {
	case PACKET_ID_LOGIN_REQ:
		processRequestLogin(connSession, bodySize, bodyData)
		break
	case PACKET_ID_ROOM_ENTER_REQ:
		processPacketRoomEnter(connSession, bodySize, bodyData)
		break
	case PACKET_ID_ROOM_LEAVE_REQ:
		processPacketRoomLeave(connSession, bodySize, bodyData)
		break
	default:
		if _enableClientRequestRelayPacketIDRange(packetID) {
			_processPacketRelay(connSession, packetData)
		} else {
			NTELIB_LOG_ERROR("Unknown PacketId", zap.Uint16("id", packetID))
		}
	}

	NTELIB_LOG_DEBUG("_distributePacket", zap.Int32("sessionIndex", sessionIndex), zap.Uint16("PacketId", packetID))
}


func _processPacketRelay(connSession * connection, packetData []byte) {
	mqSubject, ok := getChatServerMQInfo(connSession.getRoomNumber())
	if ok == false {
		return
	}

	sendToMqRequestRelay(uint16(_appConfig.myServerIndex),
		mqSubject,
		connSession,
		packetData)
}

func _enableClientRequestPacketIDRange(packetID uint16, sessionIndex int32) bool {
	if packetID <= PACKET_ID_C2S_START || packetID >= PACKET_ID_C2S_END {
		NTELIB_LOG_DEBUG("_distributePacket. Invalid Packet Range", zap.Int32("sessionIndex", sessionIndex), zap.Uint16("PacketId", packetID))
		return false
	}

	return true
}

func _enableClientRequestRelayPacketIDRange(packetID uint16) bool {
	if packetID > PACKET_ID_META_RELAY_BEGIN ||
		packetID < PACKET_ID_META_RELAY_END {
		return true
	}

	return false
}

var _appConfig configAppServer