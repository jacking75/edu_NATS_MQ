using System;
using System.Collections.Generic;
using System.Text;

namespace CenterServer.PKHandler
{
    public class Base
    {
        public static ServerOption ServerOpt;

        public static Action<Int32, string, byte[], int> SendMqDataDelegate;

        const int MaxPacketLength = 4096;
        protected byte[] MQPacketEnCodeBuffer = new byte[MaxPacketLength];

        public virtual void Process(int mqIndex, ServerCommon.MQPacketHeader mqHead, byte[] mqData) { }


        
    }
}
