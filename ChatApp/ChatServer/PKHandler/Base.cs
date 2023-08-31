using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.PKHandler
{
    public class Base
    {
        public static ServerOption ServerOpt;

        public static Action<int, string, byte[], int> SendMqDataDelegate;

        protected int StartRoomNumber = 0;
        protected List<Room> RoomList;

        //UserNetSessionUniqueID는 절대 중복이 되지 않는다!!!
        static Dictionary<UInt64, Int32> RoomNumberByUserNetSessionUniqueID = new Dictionary<UInt64, Int32>();

        const int MaxPacketLength = 4096;
        protected static byte[] MQPacketEnCodeBuffer = new byte[MaxPacketLength];

        
        public virtual void Process(int mqIndex, ServerCommon.MQPacketHeader mqHead, byte[] mqData) { }



        protected void RegistUserRoomNumber(UInt64 sessionUniqueID, Int32 roomNumber)
        {
            RoomNumberByUserNetSessionUniqueID.Add(sessionUniqueID, roomNumber);
        }

        protected bool UnRegistUserRoomNumber(UInt64 sessionUniqueID)
        {
            return RoomNumberByUserNetSessionUniqueID.Remove(sessionUniqueID);
        }

        protected Room GetUserRoom(UInt64 sessionUniqueID)
        {
            if(RoomNumberByUserNetSessionUniqueID.TryGetValue(sessionUniqueID, out var roomNum))
            {
                return GetRoom(roomNum);
            }

            return null;
        }

        protected Room GetRoom(int roomNumber)
        {
            var index = roomNumber - StartRoomNumber;

            if (index < 0 || index >= RoomList.Count())
            {
                return null;
            }

            return RoomList[index];
        }

       
    }
}
