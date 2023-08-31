using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class MsgPackPacketHeaderInfo
    {
        const int MsgPackDumySize = 3;
        public const int Size = 8;

        public static UInt16 GetTotalSize(byte[] data, int startPos)
        {
            return BitConverter.ToUInt16(data, startPos + MsgPackDumySize);
        }

        public static UInt16 ReadPacketID(byte[] data, int offset)
        {
            var pos = offset + MsgPackDumySize + 2;
            return BitConverter.ToUInt16(data, pos);
        }


        public static void Write(byte[] data, UInt16 totalSize, UInt16 packetID)
        {
            var totalSizeBytes = BitConverter.GetBytes(totalSize);
            var packetIDBytes = BitConverter.GetBytes(packetID);

            Buffer.BlockCopy(totalSizeBytes, 0, data, MsgPackDumySize, 2);
            Buffer.BlockCopy(packetIDBytes, 0, data, MsgPackDumySize + 2, 2);
        }
    }
}
