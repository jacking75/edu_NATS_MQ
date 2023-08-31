using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerCommon;


namespace ChatServer.PKHandler
{
    public class EnterRoom : Base
    {
        public EnterRoom(List<Room> roomList)
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
            Console.WriteLine("EnterRoom");

            var reqPacket = new MQReqRoomEnter();
            reqPacket.Decode(mqData);

            var roomNumber = reqPacket.RoomNumber;
            var room = GetRoom(roomNumber);
            if (room == null)
            {
                ResponseRoomEnter(receiverIndex, mqHeader.SenderIndex,
                                sessionIndex, sessionUniqueID,
                               0, SErrorCode.RoomEnterInvalidRoomNum);
                return;
            }

            if (room.AddUser(reqPacket.UserID, mqHeader.SenderIndex, sessionIndex, sessionUniqueID) == false)
            {
                ResponseRoomEnter(receiverIndex, mqHeader.SenderIndex,
                                sessionIndex, sessionUniqueID,
                               0, SErrorCode.RoomEnterDuplicateUserID);
                return;
            }

            
            //TODO 방의 다른 유저에게 새로운 유저가 들어옴을 알린다.

            RegistUserRoomNumber(sessionUniqueID, reqPacket.RoomNumber);

            ResponseRoomEnter(receiverIndex, mqHeader.SenderIndex,
                                sessionIndex, sessionUniqueID,
                               roomNumber, SErrorCode.None);
            Console.WriteLine("EnterRoom - Success");
          
        }

        void ResponseRoomEnter(int mqIndex, UInt16 senderindex, 
                                Int32 userNetSessionIndex, UInt64 userNetSessionUniqueID, 
                                Int32 rooomNumber, SErrorCode errorCode)
        {
            var responseMq = new MQResRoomEnter()
            {
                ID = MQPacketID.MQ_PACKET_ID_ROOM_ENTER_RES.ToUInt16(),
                SenderInitial = MQSenderInitialHelper.ChatServerInitialToNumber,
                SenderIndex = Base.ServerOpt.ServerIndex,
                UserNetSessionIndex = userNetSessionIndex,
                UserNetSessionUniqueID = userNetSessionUniqueID,

                Result = (Int16)errorCode,
                RoomNumber = rooomNumber
            };

            var sendDataSize = responseMq.Encode(MQPacketEnCodeBuffer);

            var subject = $"GATE.{senderindex}";
            SendMqDataDelegate(mqIndex, subject, MQPacketEnCodeBuffer, sendDataSize);

        }
    }
}
