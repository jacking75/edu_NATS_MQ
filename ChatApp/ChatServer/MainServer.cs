using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZLogger;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ServerCommon;


namespace ChatServer
{
    public class MainServer : IHostedService
    {
        readonly IHostApplicationLifetime AppLifetime;
        public static ILogger<MainServer> GlobalLogger;

        ServerOption ServerOpt;


        PacketProcessor MainPacketProcessor = new PacketProcessor();
        RoomManager RoomMgr = new RoomManager();

        MqManager MQMgr = new MqManager();


        public MainServer(IHostApplicationLifetime appLifetime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
        {
            ServerOpt = serverConfig.Value;
            GlobalLogger = logger;
            AppLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            AppLifetime.ApplicationStarted.Register(AppOnStarted);
            AppLifetime.ApplicationStopped.Register(AppOnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void AppOnStarted()
        {
            GlobalLogger.ZLogInformation("OnStarted");

            Start(ServerOpt);
        }

        private void AppOnStopped()
        {
            GlobalLogger.ZLogInformation("OnStopped - begin");

            Stop();

            GlobalLogger.ZLogInformation("OnStopped - end");
        }
            
        public void Start(ServerOption serverOpt)
        {
            try
            {
                CreateComponent(serverOpt);

                GlobalLogger.ZLogInformation("서버 생성 성공");
            }
            catch (Exception ex)
            {
                GlobalLogger.ZLogError($"서버 생성 실패: {ex.ToString()}");
            }          
        }

        
        public void Stop()
        {            
            MQMgr.Destory();

            MainPacketProcessor.Destory();
        }

        public SErrorCode CreateComponent(ServerOption serverOpt)
        {
            Room.SendMqDataDelegate = this.SendData;
            Room.SendMqDataAllDelegate = this.SendDataAll;
            RoomMgr.CreateRooms(serverOpt);

            MainPacketProcessor = new PacketProcessor();
            MainPacketProcessor.SendMqDataDelegate = this.SendData;
            MainPacketProcessor.CreateAndStart(serverOpt, RoomMgr.GetRoomList());
          
            MQMgr.Init(serverOpt.ServerIndex, 
                serverOpt.MQServerAddressList, serverOpt.MQSubjectPrefix,
                ReceivedMQData);

            Console.WriteLine("CreateComponent - Success");
            return SErrorCode.None;
        }


        void ReceivedMQData(int index, byte[] data)
        {
            MainPacketProcessor.InsertMQMessage(index, data);
        }

        public void SendData(int mqIndex, string subject, byte[] sendData, int count)
        {
            try
            {
                MQMgr.Send(mqIndex, subject, sendData, count);
            }
            catch(Exception ex)
            {
                // TimeoutException 예외가 발생할 수 있다
                Console.WriteLine($"{ex.ToString()},  {ex.StackTrace}");
            }
        }

        public void SendDataAll(string subject, byte[] sendData, int count)
        {
            try
            {
                MQMgr.SendAll(subject, sendData, count);
            }
            catch (Exception ex)
            {
                // TimeoutException 예외가 발생할 수 있다
                Console.WriteLine($"{ex.ToString()},  {ex.StackTrace}");
            }
        }

    }    
}
