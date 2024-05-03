using ServerCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace CenterServer;

public class PacketDistributor
{
    bool IsThreadRunning = false;
    System.Threading.Thread ProcessThread = null;

    BufferBlock<(int,byte[])> MsgBuffer = new ();

    Dictionary<UInt16, PKHandler.Base> PacketHandlerMap = new Dictionary<UInt16, PKHandler.Base>();

    public Action<Int32, string, byte[], int> SendMqDataDelegate;


    public void CreateAndStart(ServerOption option)
    {
        Console.WriteLine("[PacketDistributor.CreateAndStart] - begin");

        PKHandler.Base.SendMqDataDelegate = SendMqDataDelegate;
        PKHandler.Base.ServerOpt = option;

        RegistPacketHandler();

        IsThreadRunning = true;
        ProcessThread = new System.Threading.Thread(this.Process);
        ProcessThread.Start();
                    
        Console.WriteLine("[PacketDistributor.CreateAndStart] - end");
    }

    public void Destory()
    {
        IsThreadRunning = false;
        MsgBuffer.Complete();
    }

    public void Distribute(int mqIndex, byte[] mqData) => MsgBuffer.Post((mqIndex,mqData));

    void Process()
    {
        var mqHeader = new ServerCommon.MQPacketHeader();

        while (IsThreadRunning)
        {
            try
            {
                var mqData = MsgBuffer.Receive();
                
                mqHeader.Read(mqData.Item2);

                if (PacketHandlerMap.ContainsKey(mqHeader.ID))
                {
                    PacketHandlerMap[mqHeader.ID].Process(mqData.Item1, mqHeader, mqData.Item2);
                }
                else
                {
                    Console.WriteLine($"mqId: {mqHeader.ID}");
                }
            }
            catch (Exception ex)
            {
                if(IsThreadRunning)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }

    void RegistPacketHandler()
    {
        PacketHandlerMap.Add((UInt16)MQPacketID.MQ_PACKET_ID_REQ_CHAT_SERVER_MQ_SUBJECT, new PKHandler.ChatServerMQSubject()); 

    }
           
    
}
