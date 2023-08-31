using MessagePack;
using ServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServer
{
    public class MqDataProcess
    {
        //TODO DB와 Redis 객체가 있어야 한다

        Dictionary<UInt16, Action<int, byte[]>> RequestFuncDic = new ();

        Action<Int32, string, byte[], int> MQSendDelegate;

        UInt16 MyServerIndex = 0;

        byte[] EncodingBuffer = new byte[8012];


        public void Init(UInt16 myServerIndex, Action<Int32, string, byte[], int> mqSendFunc)
        {
            MyServerIndex = myServerIndex;
            MQSendDelegate = mqSendFunc;

            SetHandler();
        }

        void SetHandler()
        {
            RequestFuncDic.Add((UInt16)MQPacketID.MQ_PACKET_ID_DB_LOGIN_REQ, LoginReq);

        }

        public void ReqProcess(int mqIndex, byte[] mqData)
        {
            var mqHeader = new ServerCommon.MQPacketHeader();
            mqHeader.Read(mqData);
                        
            if (RequestFuncDic.ContainsKey(mqHeader.ID))
            {
                RequestFuncDic[mqHeader.ID](mqIndex, mqData);
            }
            else
            {
                Console.Write("Unknown MQ Req Id: " + mqHeader.ID);
            }
        }


        void LoginReq(int mqIndex, byte[] mqData)
        {
            try
            {
                //Console.WriteLine($"LoginReq");
                var requestMQ = new MQReqLogin();
                requestMQ.Decode(mqData);

                //TODO 레디스에서 인증 정보를 읽고, 대조하고, 인증완료 저장한다

                var responseMq = new MQResLogin()
                {
                    ID = MQPacketID.MQ_PACKET_ID_DB_LOGIN_RES.ToUInt16(),
                    SenderInitial = MQSenderInitialHelper.DBServerInitialToNumber,
                    SenderIndex = MyServerIndex,
                    UserNetSessionIndex = requestMQ.UserNetSessionIndex,
                    UserNetSessionUniqueID = requestMQ.UserNetSessionUniqueID,
                    
                    Result = (Int16)SErrorCode.None
                };

                var sendDataSize = responseMq.Encode(EncodingBuffer);
                
                var subject = $"GATE.{requestMQ.SenderIndex}";
                MQSendDelegate(mqIndex, subject, EncodingBuffer, sendDataSize);

                Console.WriteLine($"Response MQResLBLogin. subject:{subject}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"LoginReq. Exception:{ex.ToString()}");
            }
        }

        void LobbyLoginTemp(byte[] mqData)
        {
            //var requestMq = MessagePackSerializer.Deserialize<ServerCommon.MQReqLBLogin>(mqData);
            //Console.WriteLine($"Sender:{requestMq.Sender}. UserID:{requestMq.UserID}");

            //TODO 레디스에 확인해 본다

            //var responseMq = new ServerCommon.MQResLBLogin()
            //{
            //    ID = ServerCommon.MQ_LBDB_DATA_ID.RES_LOGIN.ToUInt16(),
            //    Sender = MySenderName,
            //    Result = (Int16)ServerCommon.ERROR_CODE.NONE, 
            //    UserNetSessionID = requestMq.UserNetSessionID,
            //    UserID = requestMq.UserID
            //};
            //var sendData = MessagePackSerializer.Serialize(responseMq);

            //var routingKey = $"{MySenderName}_TO_{requestMq.Sender}";
            //MQSendFunc(routingKey, sendData);

            //Console.WriteLine($"Response MQResLBLogin. routingKey:{routingKey}");
        }

    }
}
