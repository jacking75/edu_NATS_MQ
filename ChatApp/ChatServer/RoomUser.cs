using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class RoomUser
    {
        public string UserID { get; private set; }
        public UInt16 GatewayServerIndex { get; private set; }
        public Int32 NetSessionIndex { get; private set; }
        public UInt64 NetSessionUniqueID { get; private set; }

        public void Set(string userID, UInt16 gatewayServerIndex, Int32 netSessionIndex, UInt64 netSessionUniqueID)
        {
            UserID = userID;
            GatewayServerIndex = gatewayServerIndex;
            NetSessionIndex = netSessionIndex;
            NetSessionUniqueID = netSessionUniqueID;
        }
    }
}
