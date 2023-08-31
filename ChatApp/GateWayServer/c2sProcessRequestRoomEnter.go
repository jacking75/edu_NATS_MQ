package main

func processPacketRoomEnter(connSession * connection,
							bodySize int16, bodyData []byte) {
	sessionIndex := connSession.getIndex()
	sessionUniqueID := connSession.getNetworkUniqueID()

	var request RoomEnterReqPacket
	if (&request).Decoding(bodyData) == false {
		sendEnterRoomResult(sessionIndex, sessionUniqueID, ERROR_CODE_ROOM_ENTER_PACKET_DECODING, -1)
		return
	}

	// 방입장 준비 상태로 변경
	if connSession.setPreRoom() == false {
		sendEnterRoomResult(sessionIndex, sessionUniqueID,  ERROR_CODE_ROOM_ENTER_INVALID_STATE, -1)
		return
	}

	mqSubject, ok := getChatServerMQInfo(request.RoomNumber)
	if ok == false {
		sendEnterRoomResult(sessionIndex, sessionUniqueID,  ERROR_CODE_ROOM_ENTER_INVALID_ROOM_NUM, -1)
		return
	}

	sendToMqRequestEnterRoom(uint16(_appConfig.myServerIndex),
							mqSubject,
							connSession,
							request.RoomNumber)

}