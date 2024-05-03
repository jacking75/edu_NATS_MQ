using ServerCommon;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Room
    {
        public int Index { get; private set; }
        public int Number { get; private set; }

        int MaxUserCount = 0;

        public List<RoomUser> UserList { get; private set; } = new List<RoomUser>();

        public static Action<int, string, byte[], int> SendMqDataDelegate;
        public static Action<string, byte[], int> SendMqDataAllDelegate;


        public void Init(int index, int number, int maxUserCount)
        {
            Index = index;
            Number = number;
            MaxUserCount = maxUserCount;
        }

        public bool AddUser(string userID, UInt16 gateWaySereverIndex, Int32 netSessionIndex, UInt64 netSessionUniqueID)
        {
            if(GetUser(netSessionUniqueID) != null)
            {
                return false;
            }

            var roomUser = new RoomUser();
            roomUser.Set(userID, gateWaySereverIndex, netSessionIndex, netSessionUniqueID);
            UserList.Add(roomUser);

            return true;
        }

        public void RemoveUser(UInt64 netSessionUniqueID)
        {
            var index = UserList.FindIndex(x => x.NetSessionUniqueID == netSessionUniqueID);
            UserList.RemoveAt(index);
        }

        public bool RemoveUser(RoomUser user)
        {
            return UserList.Remove(user);
        }

        public RoomUser GetUserByID(string userID)
        {
            return UserList.Find(x => x.UserID == userID);
        }

        public RoomUser GetUser(UInt64 netSessionUniqueID)
        {
            return UserList.Find(x => x.NetSessionUniqueID == netSessionUniqueID);
        }

        public int CurrentUserCount()
        {
            return UserList.Count();
        }
                
        public void BroadcastRelayPacket(Int32 excludeUserSessionID, byte[] mqDataBuffer, byte[] relayData)
        {
            var responseMq = new MQResRelay()
            {
                ID = (UInt16)MQPacketID.MQ_PACKET_ID_RELAY_RES,
                SenderInitial = MQSenderInitialHelper.ChatServerInitialToNumber,
                RelayData = relayData
            };
            var responseMqSize = responseMq.Encode(mqDataBuffer);
            MQPacketHeader.WriteHeadMultiUserDataOffset(mqDataBuffer, (UInt16)responseMqSize);


            List<GWUserSessionIDList> gwIdsList = new List<GWUserSessionIDList>();

            foreach (var user in UserList)
            {
                if (user.NetSessionIndex == excludeUserSessionID)
                {
                    continue;
                }

                DivideGWUserUniqueIdList(user.GatewayServerIndex, user.NetSessionIndex, gwIdsList);
            }

            foreach (var gwIds in gwIdsList)
            {
                var writePos = responseMqSize;
                
                FastBinaryWrite.UInt16(mqDataBuffer, writePos, (UInt16)gwIds.SessionIDList.Count);
                writePos += 2;

                foreach(var sessionID in gwIds.SessionIDList)
                {
                    FastBinaryWrite.Int32(mqDataBuffer, writePos, sessionID);
                    writePos += 4;
                }
  
                var subject = $"GATE.{gwIds.ServerIndex}";
                SendMqDataAllDelegate(subject, mqDataBuffer, writePos);
            }
        }

        void DivideGWUserUniqueIdList(UInt16 gwServerIndex, Int32 sessionID, List<GWUserSessionIDList> gwIDList)
        {
            foreach (var gwIds in gwIDList)
            {
                if (gwIds.ServerIndex == gwServerIndex)
                {
                    gwIds.SessionIDList.Add(sessionID);
                    return;
                }
            }

            var newGWIdList = new GWUserSessionIDList();
            newGWIdList.ServerIndex = gwServerIndex;
            newGWIdList.SessionIDList.Add(sessionID);
            gwIDList.Add(newGWIdList);

        }


        public class GWUserSessionIDList
        {
            public UInt16 ServerIndex;            
            public List<Int32> SessionIDList = new ();
        }
    }


    
}
