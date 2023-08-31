# Configuration
- JSON, YAML로 적을 수 있다.  
- Lines can be commented with # and //
  
예   
```
listen: 127.0.0.1:4222
authorization: {
    token: "3secret"
}
```

Configuration Reloading  
> nats-server --signal reload  


## 설정 정보를 사용하여 서버 실행
  
```
nats-server -c server.conf
```
  
  
## 변수
  
```
# Define a variable in the config
TOKEN: "secret"

# Reference the variable
authorization {
    token: $TOKEN
}
```  
  

## Include Directive
server.conf:  
```
listen: 127.0.0.1:4222
include ./auth.conf
```
  

auth.conf:  
```
authorization: {
    token: "f0oBar"
}
```
  
> nats-server -c server.conf
    
  
## Configuration Properties
Connectivity    
| Property         | Description                                                                                                                                       | Default                               |
|------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------|
| host             | Host for client connections.                                                                                                                      | 0.0.0.0                               |
| port             | Port for client connections.                                                                                                                      | 4222                                  |
| listen           | Listen specification <host>:<port> for client connections. Either use this or the options host and/or port.                                       | same as host, port                    |
| client_advertise | 클라이언트나 타 서버에 어드바이즈 하기 위한 대체 클라이언트 리슨 기능 <host>:<port> 또는 <host> . NAT를 사용한 클라스터 설정에 편리하다 |
| tls              | Configuration map for tls for client and http monitoring.                                                                                         |                                       |
| cluster          | Configuration map for [cluster](https://docs.nats.io/nats-server/configuration/clustering).                                                                                                                    |                                       |
| gateway          | Configuration map for [gateway](https://docs.nats.io/nats-server/configuration/gateways).                                                                                                                    |                                       |
| leafnode         | Configuration map for a [leafnode](https://docs.nats.io/nats-server/configuration/leafnodes).                                                                                                                 |                                       |
  

Connection Timeouts    
| Property       | Description                                                                                                                                                                                                                                                | Default |
|----------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------|
| ping_interval  | 클라이언ㅌ, 리프 노드 및 루트에 ping을 보내는 시간. 메시지나 클라이언트 측의 ping 등의 클라이언트 트래픽이 이쓴 경우 서버는 ping을 보내지 않는다. 이 값은 [클라이언트가 사용](https://docs.nats.io/developing-with-nats/connecting/pingpong)하는 값 보다 크게 하는 것을 추천한다. | "2m"    |
| ping_max       | 서버가 원하는 ping 응답 수에 대응하지 않으면 접속을 끊는다                                                                                                       | 2       |
| write_deadline | 서버가 쓰기(송신)를 할 때 블럭하는 최대 시간(클라이언트가 수신하지 않아서 수신버퍼가 차면 블럭된다). 이 값을 넘으면 접속을 끊는다. 클라이언트에서의 대처법은 [slow consumer](https://docs.nats.io/developing-with-nats/events/slow)을 참고한다.                                                                     | "2s"    |
     
  
Limits   
| Property          | Description                                                                                                                                                                    | Default      |
|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------|
| max_connections   | Maximum number of active client connections.                                                                                                                                   | 64K          |
| max_control_line  | 프로토콜 라인의 최대 길이(서브젝트와 큐 그룹을 합친 길이). 이 값을 크게하면 사용하면 [클라이언트의 변경](https://docs.nats.io/developing-with-nats/connecting/misc#set-the-maximum-control-line-size)이 필요할 때가 있다. 모든 트래픽에 적용된다. | 4Kb          |
| max_payload       | 메시지 페이로드의 최대 바이트 수. 이 사이즈를 작게하면 클라이언트에 [청킹](https://docs.nats.io/developing-with-nats/connecting/misc#get-the-maximum-payload-size)을 구해야할 수도 있다. 클라이언트와 리프 노드의 페이로드에 적용된다.                 | 1Mb          |
| max_pending       | 접속을 위해 버퍼링 되는 최대 바이트 수로 클라이언트 접속에 사용된다.                                                                                               | 64Mb         |
| max_subscriptions | 클라이언트와 리프 노드 계정의 접속당 서버스크립션 최대 수.                                                                                                  | 0, unlimited |

   
  
여기에서 소개하지 않은 설정은 NATS 문서를 확인하기 바란다.  

<br>        
<br>    


## 사용 예
- 서버 주소 지정  
config.conf  
```
listen: 127.0.0.1:24001
```
실행
```
nats-server -c config.conf
```
  
  