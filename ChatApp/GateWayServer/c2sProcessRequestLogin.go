package main

import (
	"bytes"
	"go.uber.org/zap"
	. "GateWayServer/gohipernetFake"
)

func processRequestLogin(connSession * connection,
	bodySize int16, bodyData []byte) {
	sessionIndex := connSession.getIndex()
	sessionUniqueId := connSession.getNetworkUniqueID()

	var request LoginReqPacket
	if (&request).Decoding(bodyData) == false {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_PACKET_DECODING_FAIL)
		return
	}

	userId := bytes.Trim(request.UserID[:], "\x00")
	if len(userId) <= 0 {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_LOGIN_USER_INVALID_ID)
		return
	}

	userPw := bytes.Trim(request.UserPW[:], "\x00")
	if len(userPw) <= 0 {
		sendLoginResult(sessionIndex, sessionUniqueId, ERROR_CODE_LOGIN_USER_INVALID_PW)
		return
	}


	if connSession.setPreLogin(userId) == false {
		NTELIB_LOG_DEBUG("processRequestLogin - fail setPreLogin", zap.Int32("sessionIndex", sessionIndex))
		return;
	}

	sendToMqRequestLogin(uint16(_appConfig.myServerIndex),
		connSession,
		request.UserID, request.UserPW)
}