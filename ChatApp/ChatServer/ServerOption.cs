using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ServerOption
    {
        public UInt16 ServerIndex { get; set; }

        public string Name { get; set; }
                
        public int RoomMaxCount { get; set; } = 0;

        public int RoomMaxUserCount { get; set; } = 0;

        public int RoomStartNumber { get; set; } = 0;


        public List<string> MQServerAddressList { get; set; }

        public string MQSubjectPrefix { get; set; }



    }    
}
