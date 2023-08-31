using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    class MqManager
    {
        List<ServerCommon.MQReceiver> ReceiverList = new();
        List<ServerCommon.MQSender> SenderList = new();

        public void Init(Int32 serverIndex, List<string> mqServerAddressList, string subPreFix, Action<int, byte[]> receivedMQDataEvent)
        {
            int index = 0;
            var subName = subPreFix + serverIndex;

            foreach (var address in mqServerAddressList)
            {
                var receiver = new ServerCommon.MQReceiver();
                receiver.Init(index, address, subName, null);
                receiver.ReceivedMQData = receivedMQDataEvent;
                ReceiverList.Add(receiver);


                var sender = new ServerCommon.MQSender();
                sender.Init(address);
                SenderList.Add(sender);

                Console.WriteLine($"MQ Index:{index}, Address:{address}, Sub:{subName}");

                ++index;
            }
        }

        public void Destory()
        {
            foreach (var receiver in ReceiverList)
            {
                receiver.Destory();
            }

            foreach (var sender in SenderList)
            {
                sender.Destory();
            }
        }

        public void Send(int mqIndex, string subject, byte[] payload, int payloadLen)
        {
            SenderList[mqIndex].Send(subject, payload, 0, payloadLen);
        }

        public void SendAll(string subject, byte[] payload, int payloadLen)
        {
            foreach(var sender in SenderList)
            {
                sender.Send(subject, payload, 0, payloadLen);
            }
        }
    }


}
