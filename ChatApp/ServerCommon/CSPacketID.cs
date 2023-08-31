using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{    
    // 클라이언트 
    // 101 ~ 1000
    public enum CSPacketID : int
    {                      
        // 클라이언트
        CS_BEGIN        = 100,

        //PACKET_ID_ECHO = 101,

        // Ping(Heart-beat)
        //PACKET_ID_PING_REQ = 201,
        //PACKET_ID_PING_RES = 202,
        
        //PACKET_ID_ERROR_NTF = 203,

        // 로그인
        RequestLogin = 701,
        ResponseLogin = 702,


        RequestRoomEnter = 721,
        ResponseRoomEnter = 722,
        //PACKET_ID_ROOM_USER_LIST_NTF = 723,
        //PACKET_ID_ROOM_NEW_USER_NTF = 724,

        RequestRoomLeave = 726,
        ResponseRoomLeave = 727,
        //PACKET_ID_ROOM_LEAVE_USER_NTF = 728,

        PACKET_ID_ROOM_CHAT_REQ = 761,
        PACKET_ID_ROOM_CHAT_NOTIFY = 762,

        
        CS_END          = 1000,        
    }

    
    
}
