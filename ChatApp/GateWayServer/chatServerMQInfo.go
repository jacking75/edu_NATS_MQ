package main

import (
	"go.uber.org/zap"
	"sync"

	. "GateWayServer/gohipernetFake"
)

var _roomNumMQInfo *sync.Map

func init_chatServerMQSubject(serverIndex uint16) {
	_roomNumMQInfo = new(sync.Map)

	sendToMqRequestChatServerMQInfo(serverIndex)
}

func responseChatServerMQInfo(mqRes MQResChatServerMQInfo) {
	for i := int16(0); i < mqRes.count; i++ {
		startRoomNum := mqRes.startRoomNums[i];
		lastRoomNum := mqRes.lastRoomNums[i];
		mqServerAddress := mqRes.mqServers[i];
		mqSubject := mqRes.mqSubjects[i];

		for j := startRoomNum; j <= lastRoomNum; j++ {
			_roomNumMQInfo.LoadOrStore(j, mqSubject)
		}

		NTELIB_LOG_INFO("[responseChatServerMQInfo]", zap.Int32("startRoom", startRoomNum), zap.Int32("lastRoom", lastRoomNum), zap.String("mqServerAddress",mqServerAddress), zap.String("mqSubject",mqSubject))
	}
}

func getChatServerMQInfo(roomNum int32) (string, bool) {
	value, ok := _roomNumMQInfo.Load(roomNum)
	if ok == false {
		return string(""), false
	}
	return value.(string), true
}

