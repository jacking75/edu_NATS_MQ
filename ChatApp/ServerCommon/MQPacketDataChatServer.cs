using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class MQReqRoomEnter : MQPacketHeader
    {
        public string UserID;
        public Int32 RoomNumber;


        public void Decode(byte[] buffer)
        {
            var pos = MQPacketHeader.Size;
            int stringReadLen = 0;
            
            (stringReadLen, UserID) = FastBinaryRead.String(buffer, pos);
            pos += stringReadLen;

            RoomNumber = FastBinaryRead.Int32(buffer, pos);
            pos += 4;
        }
    }

    public class MQResRoomEnter : MQPacketHeader
    {
        public Int16 Result;
        public Int32 RoomNumber;


        public int Encode(byte[] buffer)
        {
            var pos = base.Write(buffer);

            FastBinaryWrite.Int16(buffer, pos, Result);
            pos += 2;

            FastBinaryWrite.Int32(buffer, pos, RoomNumber);
            pos += 4;

            return pos;
        }        
    }


    public class MQReqRoomLeave : MQPacketHeader
    {
        public Int16 IsDisconnected;

        public void Decode(byte[] buffer)
        {
            var pos = MQPacketHeader.Size;
            IsDisconnected = FastBinaryRead.Int16(buffer, pos);
            pos += 2;
        }
    }

    public class MQResRoomLeave : MQPacketHeader
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

    public class MQResRelay : MQPacketHeader
    {
        public byte[] RelayData;

        public int Encode(byte[] buffer)
        {
            var pos = base.Write(buffer);

            var copyLen = FastBinaryWrite.Bytes(buffer, pos, RelayData);
            pos += copyLen;

            return pos;
        }
    }
}
