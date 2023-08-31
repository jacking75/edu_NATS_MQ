using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using ServerCommon;

namespace CenterServer.PKHandler
{
    class ChatServerMQSubject : Base
    {        
        public override void Process(int mqIndex, MQPacketHeader mqHead, byte[] mqData)
        {
            try
            {
                ProcessImpl(mqIndex, mqHead, mqData);
                Console.WriteLine("ChatServerMQSubject - Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void ProcessImpl(int receiverIndex, MQPacketHeader mqHead, byte[] mqData)
        {
            var mqPacketHeader = new MQPacketHeader();
            mqPacketHeader.Type = 0;
            mqPacketHeader.ID = (UInt16)MQPacketID.MQ_PACKET_ID_RES_CHAT_SERVER_MQ_SUBJECT;
            mqPacketHeader.SenderInitial = MQSenderInitialHelper.CenterServerInitialToNumber;
            mqPacketHeader.SenderIndex = ServerOpt.ServerIndex;
            mqPacketHeader.UserNetSessionIndex = mqHead.UserNetSessionIndex;
            mqPacketHeader.UserNetSessionUniqueID = mqHead.UserNetSessionUniqueID;
            mqPacketHeader.MultiUserDataOffset = 0;
            var writePos = mqPacketHeader.Write(MQPacketEnCodeBuffer);

            var response = new MQResChatServerMQSubject();
            response.Count = (Int16)ServerOpt.RoomRangeSubjectList.Count;

            foreach (var chatInfo in ServerOpt.RoomRangeSubjectList)
            {
                response.StartRoomNumList.Add(chatInfo.StartNum);
                response.LastRoomNumList.Add(chatInfo.LastNum);
                response.MQServerList.Add(chatInfo.MQServerAddress);
                response.SubjectList.Add(chatInfo.MQSubject);
            }

            var mqDataSize = response.EncodeBody(MQPacketEnCodeBuffer, writePos);

            var subject = $"GATE.{mqHead.SenderIndex}";
            SendMqDataDelegate(receiverIndex, subject, MQPacketEnCodeBuffer, mqDataSize);
        }

    }
}
