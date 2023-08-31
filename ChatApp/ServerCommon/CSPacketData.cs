using MessagePack; //https://github.com/neuecc/MessagePack-CSharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{  
    [MessagePackObject]
    public class MsgPackPacketHeader
    {
        [Key(0)]
        public Byte[] Head = new Byte[MsgPackPacketHeaderInfo.Size];
    }


    [MessagePackObject]
    public class RoomChatReqPacket : MsgPackPacketHeader
    {
        [Key(1)]
        public string Message;
    }

    [MessagePackObject]
    public class RoomChatNtfPacket : MsgPackPacketHeader
    {
        [Key(1)]
        public string UserID;
        [Key(2)]
        public string Message;
    }


}
