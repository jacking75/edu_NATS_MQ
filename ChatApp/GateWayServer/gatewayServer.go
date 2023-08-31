package main

import (
	"encoding/binary"
	"go.uber.org/zap"
	"strconv"
	"strings"
	"time"

	. "GateWayServer/gohipernetFake"
)

type configAppServer struct {
	name string
	mqAddress string
	myServerIndex int

}

type GateWayServer struct {
	ServerIndex int
	IP          string
	Port        int
	//PacketChan      chan Packet
}

func start(netConfig NetworkConfig, appConfig configAppServer) {
	NTELIB_LOG_INFO("CreateServer !!!")

	var server GateWayServer
	server.ServerIndex = appConfig.myServerIndex

	if server.setIPAddress(netConfig.BindAddress) == false {
		NTELIB_LOG_ERROR("fail. server address")
		return
	}

	Init_packet()

	distributePacketInit(appConfig)

	connectionMgrInst.init(netConfig.MaxSessionCount, int32(netConfig.MaxSessionCount))

	inComingMqDataFunc := func(data []byte) {
		inComingMQData(data)
	}

	mqItit(appConfig, inComingMqDataFunc)

	init_chatServerMQSubject(uint16(appConfig.myServerIndex))

	stopFunc := func() {
		server.Stop()
	}
	go signalsHandling_goroutine(stopFunc)

	initAndStartNetwork(&server, &netConfig);

	waitingServerStop()
	time.Sleep(1 * time.Second)
	NTELIB_LOG_INFO("END !!!!!!!")
}

func (server *GateWayServer) setIPAddress(ipAddress string) bool {
	results := strings.Split(ipAddress, ":")
	if len(results) != 2 {
		return false
	}

	server.IP = results[0]
	server.Port, _ = strconv.Atoi(results[1])

	NTELIB_LOG_INFO("Server Address", zap.String("IP", server.IP), zap.Int("Port", server.Port))
	return true
}

func initAndStartNetwork(server *GateWayServer, netConfig *NetworkConfig) {
	networkFunctor := SessionNetworkFunctors{}
	networkFunctor.OnConnect = _onConnect
	networkFunctor.OnReceive = _onReceive
	networkFunctor.OnReceiveBufferedData = nil
	networkFunctor.OnClose = OnClose
	networkFunctor.PacketTotalSizeFunc = packetTotalSize
	networkFunctor.PacketHeaderSize = _PACKET_HEADER_SIZE
	networkFunctor.IsClientSession = true

	NetLibInitNetwork(_PACKET_HEADER_SIZE, _PACKET_HEADER_SIZE)
	NetLibStartNetwork(netConfig, networkFunctor)
}

func (server *GateWayServer) Stop() {
	NTELIB_LOG_INFO("Stop GateWayServer Start !!!")

	mqClose()

	NetLibStop() // 이 함수가 꼭 제일 먼저 호출 되어야 한다.

	NTELIB_LOG_INFO("Stop GateWayServer End !!!")
}

const _PACKET_HEADER_SIZE = 8
func packetTotalSize(data []byte) int16 {
	totalsize := binary.LittleEndian.Uint16(data[3:])
	return int16(totalsize)
}

func _onConnect(sessionIndex int32, sessionUniqueID uint64) {
	NTELIB_LOG_INFO("client _onConnect", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueID))

	connectionMgrInst.addSession(sessionIndex, sessionUniqueID)
}

func _onReceive(sessionIndex int32, sessionUniqueID uint64, data []byte) bool {
	NTELIB_LOG_DEBUG("_onReceive", zap.Int32("sessionIndex", sessionIndex),
		zap.Uint64("sessionUniqueID", sessionUniqueID),
		zap.Int("packetSize", len(data)))

	distributePacket(sessionIndex, sessionUniqueID, data)
	return true
}

func OnClose(sessionIndex int32, sessionUniqueID uint64) {
	NTELIB_LOG_INFO("client OnCloseClientSession", zap.Int32("sessionIndex", sessionIndex), zap.Uint64("sessionUniqueId", sessionUniqueID))

	_disConnectClient(sessionIndex, sessionUniqueID)
}

func _disConnectClient(sessionIndex int32, sessionUniqueId uint64) {
	connSession := connectionMgrInst.getSessionStrictly(sessionIndex, sessionUniqueId)
	if connSession == nil {
		return
	}

	roomNumber := connSession.getRoomNumber()
	mqSubject, ok := getChatServerMQInfo(roomNumber)
	if ok {
		sendToMqRequestLeaveRoom(uint16(_appConfig.myServerIndex),
			mqSubject,
			connSession,
			true)
	}

	connectionMgrInst.removeSession(sessionIndex, connSession.IsAuth())
	NTELIB_LOG_INFO("DisConnectClient - Login User", zap.Int32("sessionIndex", sessionIndex))
}