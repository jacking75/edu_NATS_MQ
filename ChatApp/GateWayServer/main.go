package main

import (
	. "GateWayServer/gohipernetFake"
	"flag"
)

func main() {
	NetLibInitLog()
	netConfig, appConfig := parseAppConfig()
	netConfig.WriteNetworkConfig(true)

	start(netConfig, appConfig)
}

type arrayChatServerIndexFlag []string
var arrayChatServerIndex arrayChatServerIndexFlag
func (i *arrayChatServerIndexFlag) String() string {
	return "my ChatServerIndex representation"
}
func (i *arrayChatServerIndexFlag) Set(value string) error {
	*i = append(*i, value)
	return nil
}

func parseAppConfig() (NetworkConfig, configAppServer) {
	NTELIB_LOG_INFO("[[Setting NetworkConfig]]")

	appConfig := configAppServer{}
	netConfig := NetworkConfig{}

	flag.BoolVar(&netConfig.IsTcp4Addr, "c_IsTcp4Addr", true, "bool flag")
	flag.StringVar(&netConfig.BindAddress, "c_BindAddress", "192.168.0.11:11021", "string flag")
	flag.IntVar(&netConfig.MaxSessionCount, "c_MaxSessionCount", 0, "int flag")
	flag.IntVar(&netConfig.MaxPacketSize, "c_MaxPacketSize", 0, "int flag")
	flag.StringVar(&appConfig.name, "c_Name", "c_Name", "string flag")
	flag.StringVar(&appConfig.mqAddress, "c_MQServerAddress", "0.0.0.0", "string flag")
	flag.IntVar(&appConfig.myServerIndex, "c_MyServerIndex", 0, "int flag")

	flag.Parse()

	netConfig.ServerIndex = uint16(appConfig.myServerIndex)
	return netConfig, appConfig
}