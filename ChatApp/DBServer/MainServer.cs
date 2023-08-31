using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZLogger;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DBServer
{
    class MainServer : IHostedService
    {
        private readonly IHostApplicationLifetime AppLifetime;
        private readonly ILogger<MainServer> AppLogger;

        ServerOption ServerOpt;

        MqManager MQMgr = new MqManager();
        
        MqDataProcessManager ReqWorkerMgr = new MqDataProcessManager();


        public MainServer(IHostApplicationLifetime appLifetime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
        {
            ServerOpt = serverConfig.Value;
            AppLogger = logger;
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
            AppLogger.LogInformation("OnStarted");

            Start(ServerOpt);
        }

        private void AppOnStopped()
        {
            AppLogger.LogInformation("OnStopped - begin");

            Stop();

            AppLogger.LogInformation("OnStopped - end");
        }



        public void Start(ServerOption serverOpt)
        {
            MQMgr.Init(serverOpt.ServerIndex, serverOpt.MQServerAddressList, serverOpt.MQSubjectPrefix, ReceivedMQData);

            
            ReqWorkerMgr.Init((UInt16)serverOpt.ServerIndex, serverOpt.ReqWorkerThreadCount, MQMgr.Send);
            ReqWorkerMgr.Start();
        }


        public void Stop()
        {
            Console.WriteLine("Server Stop <<<<");

            ReqWorkerMgr.Stop();
            
            MQMgr.Destory();

            Console.WriteLine("Server Stop >>>");
        }


        void ReceivedMQData(int index, byte[] data)
        {
            ReqWorkerMgr.AddReqData(index, data);
        }


    }
}
