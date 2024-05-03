using ServerCommon;

using ZLogger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ChatServer
{
    class PacketProcessor
    {
        bool IsThreadRunning = false;
        System.Threading.Thread ProcessThread = null;

        //receive쪽에서 처리하지 않아도 Post에서 블럭킹 되지 않는다. 
        //BufferBlock<T>(DataflowBlockOptions) 에서 DataflowBlockOptions의 BoundedCapacity로 버퍼 가능 수 지정. 
        //BoundedCapacity 보다 크게 쌓이면 블럭킹 된다. default 값은 -1이고 무한대이다.
        BufferBlock<(int,byte[])> MsgBuffer = new ();
                
        Tuple<int,int> RoomNumberRange = new Tuple<int, int>(-1, -1);
        List<Room> RoomList = new List<Room>();
     
        public Action<int, string, byte[], int> SendMqDataDelegate;

        Dictionary<UInt16, PKHandler.Base> PacketHandlerMap = new ();


        public void CreateAndStart(ServerOption ServerOpt, List<Room> roomList)
        {
            PKHandler.Base.SendMqDataDelegate = SendMqDataDelegate;
            PKHandler.Base.ServerOpt = ServerOpt;

            var maxUserCount = ServerOpt.RoomMaxCount * ServerOpt.RoomMaxUserCount;
            
            RoomList = roomList;
            var minlobbyNum = RoomList[0].Number;
            var maxlobbyNum = RoomList[0].Number + RoomList.Count() - 1;
            RoomNumberRange = new Tuple<int, int>(minlobbyNum, maxlobbyNum);
            
            RegistPacketHandler((UInt16)ServerOpt.ServerIndex);

            IsThreadRunning = true;
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();
        }
        
        public void Destory()
        {
            IsThreadRunning = false;
            MsgBuffer.Complete();
        }

        public void InsertMQMessage(int index, byte[] data)
        {
            MsgBuffer.Post((index, data));
        }


        void RegistPacketHandler(UInt16 serverIndex)
        {
            PacketHandlerMap.Add((UInt16)MQPacketID.MQ_PACKET_ID_ROOM_ENTER_REQ, new PKHandler.EnterRoom(RoomList));
            PacketHandlerMap.Add((UInt16)MQPacketID.MQ_PACKET_ID_ROOM_LEAVE_REQ, new PKHandler.LeaveRoom(RoomList));
            PacketHandlerMap.Add((UInt16)CSPacketID.PACKET_ID_ROOM_CHAT_REQ, new PKHandler.ChatRoom(RoomList));

        }

        void Process()
        {
            UInt16 packetID = 0;
            var mqHeader = new ServerCommon.MQPacketHeader();

            while (IsThreadRunning)
            {
                try
                {
                    var (mqIndex, packetData) = MsgBuffer.Receive();
                    if(packetData == null)
                    {
                        break;
                    }

                    mqHeader.Read(packetData);
                    packetID = mqHeader.ID;

                    // 릴레이 패킷이라면
                    if (packetID == (UInt16)MQPacketID.MQ_PACKET_ID_RELAY_REQ)
                    {
                        packetID = PacketIDFromRelqyPacket(packetData);
                    }

                    if (PacketHandlerMap.ContainsKey(packetID))
                    {
                        PacketHandlerMap[packetID].Process(mqIndex, mqHeader, packetData);
                    }
                    else
                    {
                        MainServer.GlobalLogger.ZLogDebug($"세션 번호: {mqHeader.UserNetSessionIndex}, PacketID: {packetID}, 받은 데이터 크기: {packetData.Length}");
                    }
                }
                catch (Exception ex)
                {
                    if(IsThreadRunning)
                    {
                        MainServer.GlobalLogger.ZLogError($"ExceptionHelper.ToString(ex)");
                    }
                }
            }
        }


        UInt16 PacketIDFromRelqyPacket(byte[] relayPacket)
        {
            return MsgPackPacketHeaderInfo.ReadPacketID(relayPacket, MQPacketHeader.Size);
        }

        


    }
}
