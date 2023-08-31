using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class MQReqChatServerMQSubject : MQPacketHeader
    {        
    }

    public class MQResChatServerMQSubject : MQPacketHeader
    {
        public Int16 Count;
        public List<Int32> StartRoomNumList = new ();
        public List<Int32> LastRoomNumList = new();
        public List<string> MQServerList = new();
        public List<string> SubjectList = new ();


        public int EncodeBody(byte[] buffer, int writePos)
        {
            FastBinaryWrite.Int16(buffer, writePos, Count);
            writePos += 2;

            for(var i = 0; i < Count; ++i)
            {
                FastBinaryWrite.Int32(buffer, writePos, StartRoomNumList[i]);
                writePos += 4;

                FastBinaryWrite.Int32(buffer, writePos, LastRoomNumList[i]);
                writePos += 4;

                var len = FastBinaryWrite.StringNadLength(buffer, writePos, MQServerList[i]);
                writePos += len;

                len = FastBinaryWrite.StringNadLength(buffer, writePos, SubjectList[i]);
                writePos += len;
            }
            

            return writePos;
        }
    }
}
