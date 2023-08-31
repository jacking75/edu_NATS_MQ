using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    // 1001 ~ 1999
    public enum SErrorCode : Int16
    {
        None = 0, // 에러가 아니다
               
        RoomEnterInvalidRoomNum = 1021,
        RoomEnterDuplicateUserID = 1022,
    }   

    
    
}
