package main

// 1001 ~ 2000
const (
	// Center
	MQ_PACKET_ID_REQ_CHAT_SERVER_MQ_SUBJECT = 5001
	MQ_PACKET_ID_RES_CHAT_SERVER_MQ_SUBJECT = 5002

	// DB
	MQ_PACKET_ID_DB_LOGIN_REQ = 6001
	MQ_PACKET_ID_DB_LOGIN_RES = 6002

	// Chat 7001 ~ 7999
	MQ_PACKET_ID_ROOM_ENTER_REQ = 7001
	MQ_PACKET_ID_ROOM_ENTER_RES = 7002

	MQ_PACKET_ID_ROOM_LEAVE_REQ = 7011
	MQ_PACKET_ID_ROOM_LEAVE_RES = 7012

	MQ_PACKET_ID_RELAY_REQ = 7101
	MQ_PACKET_ID_RELAY_RES = 7102
)