using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBServer
{
    class ServerOption
    {
        public int ServerIndex { get; set; }

        public string Name { get; set; }


        public int ReqWorkerThreadCount { get; set; } = 0;

        public string DBAddres { get; set; }
                                

        public List<string> MQServerAddressList { get; set; }

        public string MQSubjectPrefix { get; set; }

        public string MQSubQueueName { get; set; }

    }
}
