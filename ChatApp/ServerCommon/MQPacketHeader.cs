using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class MQSenderInitialHelper
    {
        public static Byte DBServerInitialToNumber = Convert.ToByte('D');
        public static Byte GateWayServerInitialToNumber = Convert.ToByte('W');
        public static Byte CenterServerInitialToNumber = Convert.ToByte('C');
        public static Byte ChatServerInitialToNumber = Convert.ToByte('T');
    }

    public class MQPacketHeader
    {
        public const Int32 Size = 20;
        public const Int32 MultiUserDataOffsetPos = 18;

        public byte Type;
        public UInt16 ID;
        public byte SenderInitial;
        public UInt16 SenderIndex;
        public Int32 UserNetSessionIndex;
        public UInt64 UserNetSessionUniqueID;        
        public UInt16 MultiUserDataOffset;

        public int Read(byte[] buffer)
        {
            var pos = 0;

            Type = buffer[pos];
            pos += 1;

            ID = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            SenderInitial = buffer[pos];
            pos += 1;

            SenderIndex = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            UserNetSessionIndex = FastBinaryRead.Int32(buffer, pos);
            pos += 4;

            UserNetSessionUniqueID = FastBinaryRead.UInt64(buffer, pos);
            pos += 8;

            MultiUserDataOffset = FastBinaryRead.UInt16(buffer, pos);
            pos += 2;

            return pos;
        }

        public int Write(byte[] buffer)
        {
            var pos = 0;

            buffer[pos] = Type;
            pos += 1;

            FastBinaryWrite.UInt16(buffer, pos, ID);
            pos += 2;

            buffer[pos] = SenderInitial;
            pos += 1;

            FastBinaryWrite.UInt16(buffer, pos, SenderIndex);
            pos += 2;

            FastBinaryWrite.Int32(buffer, pos, UserNetSessionIndex);
            pos += 4;

            FastBinaryWrite.UInt64(buffer, pos, UserNetSessionUniqueID);
            pos += 8;

            FastBinaryWrite.UInt16(buffer, pos, MultiUserDataOffset);
            pos += 2;

            return pos;
        }


        public static int Write(byte[] buffer,
                            UInt16 packetID,
                            Byte senderInitial, 
                            UInt16 senderIndex,
                            Int32 userNetSessionIndex, 
                            UInt64 userNetSessionUniqueID 
                            )
        {
            var pos = 0;

            buffer[pos] = 0;
            pos += 1;

            FastBinaryWrite.UInt16(buffer, pos, packetID);
            pos += 2;

            buffer[pos] = senderInitial;
            pos += 1;

            FastBinaryWrite.UInt16(buffer, pos, senderIndex);
            pos += 2;

            FastBinaryWrite.Int32(buffer, pos, userNetSessionIndex);
            pos += 4;

            FastBinaryWrite.UInt64(buffer, pos, userNetSessionUniqueID);
            pos += 8;

            FastBinaryWrite.UInt16(buffer, pos, 0);
            pos += 2;

            return pos;
        }

        static public void WriteHeadMultiUserDataOffset(byte[] buffer, UInt16 offset)
        {
            FastBinaryWrite.UInt16(buffer, MultiUserDataOffsetPos, offset);
        }

    }
}
