module GateWayServer

go 1.12

require github.com/nats-io/nats.go v1.10.0

require (
	GateWayServer/gohipernetFake v0.0.0
	github.com/golang/protobuf v1.4.3 // indirect
	github.com/nats-io/nats-server/v2 v2.1.9 // indirect
	github.com/vmihailenco/msgpack/v5 v5.0.0-rc.2
	go.uber.org/zap v1.16.0
)

replace GateWayServer/gohipernetFake v0.0.0 => ./gohipernetFake
