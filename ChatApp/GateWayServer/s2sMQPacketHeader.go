package main

import . "GateWayServer/gohipernetFake"

var mqPacketHeaderSize int16 = 20
type MQPacketHeader struct {
	packetType int8
	mqPacketID uint16
	senderInitial int8
	senderIndex uint16
	userNetSessionIndex int32
	userNetSessionUniqueID uint64
	multiUserDataOffset uint16
}

func EncodingMQPacketHeader(writer *RawPacketData, header MQPacketHeader) {
	writer.WriteS8(header.packetType)
	writer.WriteU16(header.mqPacketID)
	writer.WriteS8(header.senderInitial)
	writer.WriteU16(header.senderIndex)
	writer.WriteS32(header.userNetSessionIndex)
	writer.WriteU64(header.userNetSessionUniqueID)
	writer.WriteU16(header.multiUserDataOffset)
}

func DecodingMQPacketHeader(reader *RawPacketData, header *MQPacketHeader) {
	header.packetType, _ = reader.ReadS8()
	header.mqPacketID, _ = reader.ReadU16()
	header.senderInitial, _ = reader.ReadS8()
	header.senderIndex, _ = reader.ReadU16()
	header.userNetSessionIndex, _ = reader.ReadS32()
	header.userNetSessionUniqueID, _ = reader.ReadU64()
	header.multiUserDataOffset, _ = reader.ReadU16()
}
