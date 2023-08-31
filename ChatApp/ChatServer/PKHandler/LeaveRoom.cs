using ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.PKHandler
{
    public class LeaveRoom : Base
    {
        public LeaveRoom(List<Room> roomList)
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
            Console.WriteLine("LeaveRoom");

            var room = GetUserRoom(sessionUniqueID);
            if (room == null)
            {
                return;
            }

            var reqPacket = new MQReqRoomLeave();
            reqPacket.Decode(mqData);

            room.RemoveUser(sessionUniqueID);
            
            UnRegistUserRoomNumber(sessionUniqueID);

            //TODO 방의 다른 유저에게 유저가 나감을 알린다.

            if (reqPacket.IsDisconnected != 1)
            {
                ResponseRoomLeave(receiverIndex, mqHeader.SenderIndex,
                                    sessionIndex, sessionUniqueID,
                                   SErrorCode.None);
            }

            Console.WriteLine("LeaveRoom - Success");
        }

        void ResponseRoomLeave(int mqIndex, UInt16 senderindex,
                                Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID,
                                SErrorCode errorCode)
        {
            var responseMq = new MQResRoomLeave()
            {
                ID = MQPacketID.MQ_PACKET_ID_ROOM_LEAVE_RES.ToUInt16(),
                SenderInitial = MQSenderInitialHelper.ChatServerInitialToNumber,
                SenderIndex = Base.ServerOpt.ServerIndex,
                UserNetSessionIndex = userNetSessionIndex,
                UserNetSessionUniqueID = userNetSessionUniqueID,

                Result = (Int16)errorCode,
            };

            var sendDataSize = responseMq.Encode(MQPacketEnCodeBuffer);

            var subject = $"GATE.{senderindex}";
            SendMqDataDelegate(mqIndex, subject, MQPacketEnCodeBuffer, sendDataSize);

        }
    }
}
