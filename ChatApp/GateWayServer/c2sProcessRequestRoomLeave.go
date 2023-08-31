package main

func processPacketRoomLeave(connSession * connection,
							bodySize int16, bodyData []byte) {
	sessionIndex := connSession.getIndex()
	sessionUniqueID := connSession.getNetworkUniqueID()

	mqSubject, ok := getChatServerMQInfo(connSession.getRoomNumber())
	if ok == false {
		sendLeaveRoomResult(sessionIndex, sessionUniqueID,  ERROR_CODE_LEAVE_ROOM_INVALID_STATE)
		return
	}

	sendToMqRequestLeaveRoom(uint16(_appConfig.myServerIndex),
							mqSubject,
							connSession,
							false)

}