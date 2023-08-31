# [Clustering](https://docs.nats.io/nats-server/configuration/clustering)
NATS는 클라스터 모드로 각 서버를 실행하는 것을 지원한다. 
대용량 메시지 시스템과 고 가용성을 실현하기 위해 서버를 클러스터화 할 수 있다.  
    
NATS 서버는 알고 있는 모든 서버에 대해서 주위에 알리고, 접속하여 동적으로 완전한 메시를 만들어서 이것을 실현한다. 클라이언트가 특정 서버에 접속하거나, 재 접속하면 현재의 클라스터 멤버에 대해서 정보를 통지한다. 이 동작에 의해 클라스터는 성장하거나 축소하거나 셀프회복이 가능하다. 풀 메시는 꼭 명시적으로 설정할 필요는 없다.  
  
NATS 클러스터와 된 서버에는 1 홉으로 전송이 제한되는 것을 주의해야 한다. 이것은 각 nats-server 인스턴스가 클라이언트에서 수신한 메시지를 라우트를 가지고 있는 바로 옆 nats-server 인스턴스에만 전송하는 것을 의미한다. 라우트에서 수신한 메시지는 로컬 클라이언트에만 배신된다.  
  
클러스터가 완전한 메시를 형성하고, NATS가 의도한 대로 기능하기 위해서는 일시적인 에러를 허용해야 한다. 서버가 서로간에 접속을 하고, 클라잉너트가 클러스터 내의 가 서버에 접속해야 하기 때문이다.      
     

## Cluster URLs    
클라이언트는 listen port를 추가하고, nats-server는 "cluster" URL (-cluster 옵션)을 listen 한다. 추가 nats-server 서버는 이 URL을 -routes 인수에 추가해서 클러스터 내에 참가할 수 있다. 이 옵션들은 설정 파일로 지정할 수 있지만 이 문서에서는 간단하게 하기 위해 명렬줄 방식만 사용한다.   
  

## Running a Simple Cluster 
<pre>
# Server A - the 'seed server'
> nats-server -p 4222 -cluster nats://localhost:4248

# Server B
> nats-server -p 5222 -cluster nats://localhost:5248 -routes nats://localhost:4248
# Check the output of the server for the selected client and route ports.

# Server C
> nats-server -p 6222 -cluster nats://localhost:6248 -routes nats://localhost:4248
# Check the output of the server for the selected client and route ports.
</pre>  
  
각 서버는 클라이언트와 클러스터 포트를 지정한다.  routes 옵션을 지정한 서버는 시드 서버로서 라우트를 확립한다. 클러스터링 프로토콜은 클라스터 멤버를 알리기 때문에 모든 서버는 클러스터 내의 타 서버를 발견할 수 있다. 서버가 발견되면 풀 메시를 형성하기 위해 자동적으로 이 서버에 접속을 시도한다.  보통 하나의 머신에 하나의 서버 인스턴스 만을 실행되므로 클라이언트 포트(4222)와 클러스터 포트(4248)를 재 이용하고, 시드 서버의 호스트/포트로의 단순한 라우트를 사용할 수 있다.  

마찬가지로 클러스터 내의 임의의 서버에 접속하는 클라이언트는 클러스터 내의 타 서버를 알아낸다. 서버로의 접속이 끊어진 경우 클라이언트는 다른 모든 기존 서버에 대한 접속을 시도한다.  
  
시드 서버를 위한 명시적인 설정은 없다. 시드 서버는 단순히 클러스터의 타 멤버나 클라이언트가 서버를 발견하기 위한 출발점으로만 동작할 뿐이다. 그래서 이들 서버는 클라이언트가 접속 URL 리스트에 클러스터 멤버가 라우트의 리스트에 들어가 있는 서버이다. 모든 서버가 이 리스트에 포함되어 있을 필요는 없으므로 설정의 귀찮음을 줄인다. 그러나 타 서버나 클라이언트 정상으로 접속할 수 있을지는 시드 서버가 가동하고 있는지에 의존한다. 복수의 시드 서버를 상요하고 있는 경우 이들 서버는 routes 옵션을 이용하여 서로 라우트를 확립해야 한다.  
  

## Command Line Options
아래의 클러스터 옵션을 지원한다.  
<pre>
--routes [rurl-1, rurl-2]     Routes to solicit and connect
--cluster nats://host:port    Cluster URL for solici
</pre> 
  
NATS 서버가 지정된 URL로 라우팅 되면 자신의 클러스터 URL을 라우팅에 있는 모든 다른 서버에 알려서 효과적으로 모든 다른 서버들에 라우팅 메시를 생성한다.  
클러스터링은 서버 [구성 파일](https://docs.nats.io/nats-server/configuration/clustering/cluster_config )을 사용하여 구성할 수도 있다.  
  
  
## 예) 3개의 서버 클러스터
아래 예제는 동일한 호스트에서 3개의 서버 클러스터를 실행하는 것을 보여준다.  
시드 서버로 시작하여 명령줄 매개 변수를 사용하여 디버그 정보를 생성한다.
<pre>
nats-server -p 4222 -cluster nats://localhost:5222 -D
</pre>  
  
또는 아래와 유사한 내용으로 구성 파일을 사용할 수도 있다. seed.conf
<pre>
# Cluster Seed Node

listen: 127.0.0.1:4222
http: 8222

cluster {
  listen: 127.0.0.1:4248
}
</pre>  
  
그리고 아래와 같이 서버를 시작한다  
<pre>
nats-server -config ./seed.conf -D
</pre>  

그러면 아래와 유사한 로그가 출력된다
<pre>
[83329] 2020/02/12 16:04:52.369039 [INF] Starting nats-server version 2.1.4
[83329] 2020/02/12 16:04:52.369130 [DBG] Go build version go1.13.6
[83329] 2020/02/12 16:04:52.369133 [INF] Git commit [not set]
[83329] 2020/02/12 16:04:52.369360 [INF] Starting http monitor on 127.0.0.1:8222
[83329] 2020/02/12 16:04:52.369436 [INF] Listening for client connections on 127.0.0.1:4222
[83329] 2020/02/12 16:04:52.369441 [INF] Server id is NDSGCS74MG5ZUMBOVWOUJ5S3HIOW
[83329] 2020/02/12 16:04:52.369443 [INF] Server is ready
[83329] 2020/02/12 16:04:52.369534 [INF] Listening for route connections on 127.0.0.1:4248
</pre>  
  
호스트 이름과 포트를 독립적으로 지정할 수도 있다. 포트는 필수이다.  
호스트 이름을 지정하지 않으면 '0.0.0.0'으로 바인딩 된다.  
<pre>
cluster {
  host: 127.0.0.1
  port: 4248
} 
</pre>   
  
이제 각각 시드 서버에 연결할 두 번째 서버를 시작한다.  
<pre>
nats-server -p 5222 -cluster nats://localhost:5248 -routes nats://localhost:4248 -D
</pre>  
동일한 호스트에서 실행하는 경우 클라이언트 연결 및 다른 경로를 허용하는데 사용하는 포트를 서로 다르게 해야한다.  
아래는 생성된 로그이다.  
<pre>
[83330] 2020/02/12 16:05:09.661047 [INF] Starting nats-server version 2.1.4
[83330] 2020/02/12 16:05:09.661123 [DBG] Go build version go1.13.6
[83330] 2020/02/12 16:05:09.661125 [INF] Git commit [not set]
[83330] 2020/02/12 16:05:09.661341 [INF] Listening for client connections on 0.0.0.0:5222
[83330] 2020/02/12 16:05:09.661347 [INF] Server id is NAABC2CKRVPZBIECMLZZA6L3PK
[83330] 2020/02/12 16:05:09.661349 [INF] Server is ready
[83330] 2020/02/12 16:05:09.662429 [INF] Listening for route connections on localhost:5248
[83330] 2020/02/12 16:05:09.662676 [DBG] Trying to connect to route on localhost:4248
[83330] 2020/02/12 16:05:09.663308 [DBG] 127.0.0.1:4248 - rid:1 - Route connect msg sent
[83330] 2020/02/12 16:05:09.663370 [INF] 127.0.0.1:4248 - rid:1 - Route connection created
[83330] 2020/02/12 16:05:09.663537 [DBG] 127.0.0.1:4248 - rid:1 - Registering remote route "NDSGCS74MG5ZUMBOVWOUJ5S3HIOW"
[83330] 2020/02/12 16:05:09.663549 [DBG] 127.0.0.1:4248 - rid:1 - Sent local subscriptions to route
</pre>  

시드의 서버 로그에서 실제 연결이 되고 수락됨을 알 수 있다.  
<pre>
[83329] 2020/02/12 16:05:09.663386 [INF] 127.0.0.1:62941 - rid:1 - Route connection created
[83329] 2020/02/12 16:05:09.663665 [DBG] 127.0.0.1:62941 - rid:1 - Registering remote route "NAABC2CKRVPZBIECMLZZA6L3PK"
[83329] 2020/02/12 16:05:09.663681 [DBG] 127.0.0.1:62941 - rid:1 - Sent local subscriptions to route
</pre>  
  
마지막으로 세 번째 서버를 시작한다.  
<pre>
nats-server -p 6222 -cluster nats://localhost:6248 -routes nats://localhost:4248 -D
</pre>  
다시 말하지만 다른 클라이언트 포틑와 클러스터 주소를 사용하지만 여전히 주소에서 동일한 시드 서버를 가리킨다. nats://localhost:4248  
<pre>
[83331] 2020/02/12 16:05:12.838022 [INF] Listening for client connections on 0.0.0.0:6222
[83331] 2020/02/12 16:05:12.838029 [INF] Server id is NBE7SLUDLFIMHS2U6347N3DQEJ
[83331] 2020/02/12 16:05:12.838031 [INF] Server is ready
...
[83331] 2020/02/12 16:05:12.839203 [INF] Listening for route connections on localhost:6248
[83331] 2020/02/12 16:05:12.839453 [DBG] Trying to connect to route on localhost:4248
[83331] 2020/02/12 16:05:12.840112 [DBG] 127.0.0.1:4248 - rid:1 - Route connect msg sent
[83331] 2020/02/12 16:05:12.840198 [INF] 127.0.0.1:4248 - rid:1 - Route connection created
[83331] 2020/02/12 16:05:12.840324 [DBG] 127.0.0.1:4248 - rid:1 - Registering remote route "NDSGCS74MG5ZUMBOVWOUJ5S3HIOW"
[83331] 2020/02/12 16:05:12.840342 [DBG] 127.0.0.1:4248 - rid:1 - Sent local subscriptions to route
[83331] 2020/02/12 16:05:12.840717 [INF] 127.0.0.1:62946 - rid:2 - Route connection created
[83331] 2020/02/12 16:05:12.840906 [DBG] 127.0.0.1:62946 - rid:2 - Registering remote route "NAABC2CKRVPZBIECMLZZA6L3PK"
[83331] 2020/02/12 16:05:12.840915 [DBG] 127.0.0.1:62946 - rid:2 - Sent local subscriptions to route
</pre>  

먼저 시드 서버에 대한 경로가 생성되고, 이 후 두 번째 서버의 ID 인 경로가 승인된다....IOW...3PK  
  
시드 서버의 로그는 세 번째 서버의 경로를 수락했음을 보여준다.
<pre>
[83329] 2020/02/12 16:05:12.840111 [INF] 127.0.0.1:62945 - rid:2 - Route connection created
[83329] 2020/02/12 16:05:12.840350 [DBG] 127.0.0.1:62945 - rid:2 - Registering remote route "NBE7SLUDLFIMHS2U6347N3DQEJ"
[83329] 2020/02/12 16:05:12.840363 [DBG] 127.0.0.1:62945 - rid:2 - Sent local subscriptions to route
</pre>  
  
그리고 두 번째 서버의 로그는 세 번째 서버에 연결 되었음을 보여준다. 
<pre>
[83330] 2020/02/12 16:05:12.840529 [DBG] Trying to connect to route on 127.0.0.1:6248
[83330] 2020/02/12 16:05:12.840684 [DBG] 127.0.0.1:6248 - rid:2 - Route connect msg sent
[83330] 2020/02/12 16:05:12.840695 [INF] 127.0.0.1:6248 - rid:2 - Route connection created
[83330] 2020/02/12 16:05:12.840814 [DBG] 127.0.0.1:6248 - rid:2 - Registering remote route "NBE7SLUDLFIMHS2U6347N3DQEJ"
[83330] 2020/02/12 16:05:12.840827 [DBG] 127.0.0.1:6248 - rid:2 - Sent local subscriptions to route
</pre>  
  
이 시점에서 NATS 서버의 풀 메시 클러스터가 되었다.  


## 클러스터 테스트
이제 아래처럼 동작한다.  
첫 번째 서버(포트 4222)에 가입한다. 그런 다음 각 서버(포트 4222, 5222, 6222)에 게시한다.   문제없이 메시지를 받을 수 있어야 한다.
<pre>
nats-sub -s "nats://127.0.0.1:4222" hello &
nats-pub -s "nats://127.0.0.1:4222" hello world_4222

[#1] Received on [hello] : 'world_4222'

nats-pub -s "nats://127.0.0.1:5222" hello world_5222

[#2] Received on [hello] : 'world_5222'

nats-pub -s "nats://127.0.0.1:6222" hello world_6222

[#3] Received on [hello] : 'world_6222'

nats-pub -s "nats://127.0.0.1:4222,nats://127.0.0.1:5222,nats://127.0.0.1:6222" hello whole_world

[#4] Received on [hello] : 'whole_world'

# A random server was picked: NATS server logs for the second server, port 5222
[83330] 2020/02/12 16:22:56.384754 [DBG] 127.0.0.1:63210 - cid:9 - Client connection created
[83330] 2020/02/12 16:22:56.386467 [DBG] 127.0.0.1:63210 - cid:9 - Client connection closed
</pre>  


## 클러스터에 연결
https://docs.nats.io/developing-with-nats/connecting/cluster  
  
클러스터에 연결할 때 고려해야 할 몇 가지 사항이 있다.  
- 각 클러스터 구성원에 대한 URL 전달(반 선택 사항)
- 연결 알고리즘
- 재 연결 알고리즘(나중에 논의 됨)
- 서버 제공 URL
  
클라이언트 라이브러리가 처음 연결을 시도 할 때 연결 옵션 또는 기능에 제공된 URL 목록을 사용한다. 이러한 URL은 일반적으로 모든 클라이언트가 동일한 서버에 연결되지 않도록 무작위 순서로 사용한다. 첫 번째 성공적인 연결을 사용한다. 무작위로 선택하는 것은 명시적으로 비활성화 할 수 있다.    
  
클라이언트가 서버에 연결되면 서버는 알려진 추가 서버에 대한 URL 목록을 제공 할 수 있다. 이를 통해 클라이언트는 한 서버에 연결하고 다시 연결하는 동안 다른 서버를 사용할 수 있다.  
  
초기 연결을 보장하려면 코드에 합리적인 일선 또는 **시드 서버** 목록이 포함되어야 한다. 이러한 서버는 클러스터의 다른 구성원에 대해 알고, 클라이언트에게 해당 구성원에 대해 알릴 수 있다. 그러나 연결 메서드에서 클러스터의 모든 유효한 구성원을 전달하도록 클라이언트를 구성 할 필요는 없다.

여러 연결 옵션을 전달할 수 있는 기능을 제공함으로써 NATS는 시스템이 다운되거나 클라이언트가 사용할 수 없게 될 가능성을 처리 할 수 ​​있다. 클라이언트-서버 프로토콜의 일부로 알려진 서버 목록을 클라이언트에게 제공하는 서버의 기능을 추가함으로써 클러스터가 생성한 메시는 클라이언트가 실행되는 동안 유기적으로 확장 및 변경 될 수 있다.  
  
실패 동작은 라이브러리에 따라 다르다. 연결 실패 시 발생하는 상황에 대한 정보는 클라이언트 라이브러리의 문서를 확인한다.  
아래는 Go의 예제 코드이다.    
```
servers := []string{"nats://127.0.0.1:1222", "nats://127.0.0.1:1223", "nats://127.0.0.1:1224"}

nc, err := nats.Connect(strings.Join(servers, ","))
if err != nil {
    log.Fatal(err)
}
defer nc.Close()

// Do something with the connection
```  
  

