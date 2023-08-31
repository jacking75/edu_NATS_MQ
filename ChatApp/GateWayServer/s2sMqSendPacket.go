package main

func sendToMqRequestChatServerMQInfo(serverIndex uint16) {
	reqMQPkt := MQReqChatServerMQInfo{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = serverIndex
	reqMQPkt.header.userNetSessionIndex = 0
	reqMQPkt.header.userNetSessionUniqueID = 0
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_REQ_CHAT_SERVER_MQ_SUBJECT

	mqData, _ := reqMQPkt.EncodingPacket()
	mqSend(centerServerMQSubject, mqData)
}

func sendToMqRequestLogin(serverIndex uint16,
	connSession * connection,
	userID []byte, userPW []byte) {
	reqMQPkt := MQLoginReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = serverIndex
	reqMQPkt.header.userNetSessionIndex = connSession.getIndex()
	reqMQPkt.header.userNetSessionUniqueID = connSession.getNetworkUniqueID()
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_DB_LOGIN_REQ
	reqMQPkt.userID = userID
	reqMQPkt.userPW = userPW

	mqData := reqMQPkt.EncodingPacket(connSession.buffer)
	mqSend(MQDBReqSubject, mqData)
}

func sendToMqRequestEnterRoom(myServerIndex uint16,
							mqSubject string,
							connSession * connection,
							roomNumber int32) {
	reqMQPkt := MQEnterRoomReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = connSession.getIndex()
	reqMQPkt.header.userNetSessionUniqueID = connSession.getNetworkUniqueID()
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_ROOM_ENTER_REQ
	reqMQPkt.roomNumber = roomNumber
	reqMQPkt.userID = connSession.getUserID()

	mqData := reqMQPkt.EncodingPacket(connSession.buffer)
	mqSend(mqSubject, mqData)
}

func sendToMqRequestLeaveRoom(myServerIndex uint16,
	mqSubject string,
	connSession * connection,
	isDisconnected bool) {
	reqMQPkt := MQRoomLeaveReqPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = connSession.getIndex()
	reqMQPkt.header.userNetSessionUniqueID = connSession.getNetworkUniqueID()
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_ROOM_LEAVE_REQ

	if isDisconnected {
		reqMQPkt.IsDisconnected = 1
	}

	mqData := reqMQPkt.EncodingPacket(connSession.buffer)
	mqSend(mqSubject, mqData)
}

func sendToMqRequestRelay(myServerIndex uint16,
							mqSubject string,
							connSession * connection,
							relayData []byte) {
	reqMQPkt := MQRelayPacket{}
	reqMQPkt.header.senderInitial = 'W'
	reqMQPkt.header.senderIndex = myServerIndex
	reqMQPkt.header.userNetSessionIndex = connSession.getIndex()
	reqMQPkt.header.userNetSessionUniqueID = connSession.getNetworkUniqueID()
	reqMQPkt.header.mqPacketID = MQ_PACKET_ID_RELAY_REQ
	reqMQPkt.relayPacket = relayData

	mqData := reqMQPkt.EncodingPacket(connSession.buffer)
	mqSend(mqSubject, mqData)
}

