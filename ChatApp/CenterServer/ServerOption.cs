using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterServer
{
    public class ServerOption
    {
        public UInt16 ServerIndex { get; set; }

        public string Name { get; set; }

        public int ThreadCount { get; set; } = 0;
               

        public List<string> MQServerAddressList { get; set; }
        public string MQSubjectPrefix { get; set; }

        public List<RoomRangeSubject> RoomRangeSubjectList { get; set; }
    }

    public class RoomRangeSubject
    {
        public Int32 StartNum { get; set; }
        public Int32 LastNum { get; set; }
        public string MQServerAddress { get; set; }
        public string MQSubject { get; set; }
    }
}
