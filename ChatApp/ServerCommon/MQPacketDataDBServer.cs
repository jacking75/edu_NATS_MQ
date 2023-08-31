using MessagePack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class MQReqLogin : MQPacketHeader
    {
        const int MAX_USER_ID_BYTE_LENGTH = 16;
        const int MAX_USER_PW_BYTE_LENGTH = 16;

        public string UserID;
        public string UserPW;


        public void Decode(byte[] buffer)
        {
            var pos = base.Read(buffer);

            UserID = FastBinaryRead.String(buffer, pos, MAX_USER_ID_BYTE_LENGTH);
            pos += MAX_USER_ID_BYTE_LENGTH;

            UserPW = FastBinaryRead.String(buffer, pos, MAX_USER_PW_BYTE_LENGTH);
            pos += MAX_USER_PW_BYTE_LENGTH;
        }
    }

    public class MQResLogin : MQPacketHeader
    {
        public Int16 Result;
       
        public int Encode(byte[] buffer)
        {
            var pos = base.Write(buffer);

            FastBinaryWrite.Int16(buffer, pos, Result);
            pos += 2;

            return pos;
        }
    }


    [MessagePackObject]
    public class MQMsgPackHead
    {
        [Key(0)]
        public UInt16 ID;
        [Key(1)]
        public string Sender;
    }

    // 로비의 로그인 요청
    [MessagePackObject]
    public class MQReqLBLogin : MQMsgPackHead
    {
        [Key(2)]
        public string UserNetSessionID;
        [Key(3)]
        public string UserID;
        [Key(4)]
        public string AuthToken;
    }

    [MessagePackObject]
    public class MQResLBLogin : MQMsgPackHead
    {
        [Key(2)]
        public Int16 Result;
        [Key(3)]
        public string UserNetSessionID;
        [Key(4)]
        public string UserID;
    }
    
}