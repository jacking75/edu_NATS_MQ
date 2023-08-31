using ServerCommon;

using MessagePack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.PKHandler
{
    public class ChatRoom : Base
    {
        public ChatRoom(List<Room> roomList)
        {
            StartRoomNumber = roomList[0].Number;
            RoomList = roomList;
        }

        public override void Process(int mqIndex, MQPacketHeader mqHead, byte[] mqData)
        {
            try
            {
                ProcessImpl(mqIndex, mqHead, mqData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void ProcessImpl(int receiverIndex, MQPacketHeader mqHeader, byte[] mqData)
        {
            var sessionIndex = mqHeader.UserNetSessionIndex;
            var sessionUniqueID = mqHeader.UserNetSessionUniqueID;
            Console.WriteLine("ChatRoom");

            var room = GetUserRoom(sessionUniqueID);
            if (room == null)
            {
                return;
            }

            var roomUser = room.GetUser(sessionUniqueID);
            if(roomUser == null)
            {
                return;
            }

            var relayPacketData = new ReadOnlyMemory<byte>(mqData, MQPacketHeader.Size, (mqData.Length - MQPacketHeader.Size));
            var requestPkt = MessagePackSerializer.Deserialize<RoomChatReqPacket>(relayPacketData);

            var notifyPacket = new RoomChatNtfPacket();
            notifyPacket.UserID = roomUser.UserID;
            notifyPacket.Message = requestPkt.Message;
            var relayData = MessagePackSerializer.Serialize(notifyPacket);
            MsgPackPacketHeaderInfo.Write(relayData, (UInt16)relayData.Length, (UInt16)CSPacketID.PACKET_ID_ROOM_CHAT_NOTIFY);
                        
            room.BroadcastRelayPacket(0, MQPacketEnCodeBuffer, relayData);

            Console.WriteLine("ChatRoom - Success");

        }
           
    }
}
