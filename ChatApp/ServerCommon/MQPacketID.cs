using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCommon
{
    // 5000 번부터 시작한다
    public enum MQPacketID : UInt16
    {
        //Gateway 서버쪽에 정의된 패킷 이름과 같게 하기 위해서 관련된 패킷은 대문자로 이름을 선언한다

        //Center  5001 ~ 5999
        MQ_PACKET_ID_REQ_CHAT_SERVER_MQ_SUBJECT = 5001,
        MQ_PACKET_ID_RES_CHAT_SERVER_MQ_SUBJECT = 5002,

        // DB 6000 ~ 6999
        MQ_PACKET_ID_DB_LOGIN_REQ = 6001,
        MQ_PACKET_ID_DB_LOGIN_RES = 6002,


        // Chat 7001 ~ 7999
        MQ_PACKET_ID_ROOM_ENTER_REQ = 7001,
        MQ_PACKET_ID_ROOM_ENTER_RES = 7002,

        MQ_PACKET_ID_ROOM_LEAVE_REQ = 7011,
        MQ_PACKET_ID_ROOM_LEAVE_RES = 7012,
        
        MQ_PACKET_ID_RELAY_REQ = 7101,
        MQ_PACKET_ID_RELAY_RES = 7102,
    }



    



}
