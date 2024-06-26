# NATS MQ  학습 
  
# NATS
- [JetStream](https://blog.naver.com/sssang97/223076616610 )  
- [JetStream: KV](https://blog.naver.com/sssang97/223077757847 )   
- [Learn NATS by Example](https://natsbyexample.com/ )
  

## 개요
- **Secure. Simple. Scalable. Open Source.**
- GO 언어로 구현 된 오픈 소스 메시징 시스템
- NATS Messaging
    - https://nats.io/
    - https://en.wikipedia.org/wiki/NATS_Messaging
- 메시징 서비스
    - Publish/Subscribe
    - Request/Reply
    - Queueing
- Streaming
    - 인메모리(+외부 저장소)로 메시지를 저장하고, 클라이언트에 최소 1회는 메시지를 보낼 수 있다.
	- At-least-once
	- Publisher/Subscriber Rate Limit NATS 스트리밍을 구현하는 서버가 NATS 스트리밍 서버로 NATS 서버를 베이스로 구현되어 있다. 내부에서는 Protocol Buffers도 사용하고 있다.
- 프로토콜은 텍스트 베이스로 간단
- 최대 1회, 최저 1회, 정확하게 1회 기능을 지원
- 메시지 배달 보장
    - 일반적으로 "at-most-once"(최대 1회) 라고 불리는 전달을 구현하고 있다. 이것은 메시지가 지정된 퍼블리셔에서 순차적으로 무사하게 도착하는 것을 보증 하고 있는 것을 의미하지만 서로 다른 퍼블리셔에 걸쳐서 도착하는 것은 아니다. NATS는 on으로 dial-tone을 제공하기 위한 필요한 것을 모두 하고 있다. 그러나 subscriber에 문제가 있거나 오프라인이 되거나 하면 기본적으로 NATS 플랫폼은 TCP 신뢰성만 제공하는 단순한 pub-sub transport system이라서 메시지를 수신할 수 없다.
- 느린 클라이언트를 자동으로 접속 끊음(Auto Pruning of Client)
    - Ping-Pong 타임 아웃 또는 패딩 메시지가 한계치를 넘었을 때(10MB)
- 메시지 1개 사이즈의 제한이 있지만 이것은 서버에 의해서 강제 되고, 접속 설정 시에 클라이언트에 통지 된다. 현재 최대 크기는 1MB 이다.
- [Multi-tenancy](http://www.itworld.co.kr/news/101255) and Sharing
- Message Retention and Persistence
    - 메모리, 파일 및 데이터베이스의 영속성을 지원한다. 메시지는 시간, 카운트, 시퀸스 번호로 재생할 수 있고, 내구성 있는 서브스크립션을 지원하고 있다.
	- NATS 스트리밍에 의해 스크립트는 오래된 로그 세그멘트를 cold stroage에 저장할 수 있다.
- High Availability and Fault Tolerance
    - Core NATS는 self-healing 기능을 갖춘 메쉬 클라스터를 지원하고, 클라이언트에 고 가용성을 제고한다.
	- NATS 스트리밍은 2개의 모드(FT와 풀 클라스터링)을 갖춘 warm failover 백업 서버를 탑재하고 있다.
	- JetStream은 미러링을 내장한 수평 확장을 지원한다.
- monitoring
    - http로 엔드 포인트를 가지고 있다( json )
    - top 명령어
    - Prometheus로의 exporter가 있다
        - NATS Prometheus Exporter
- Loging
    - 옵션으로 로그 출력이 가능
- 클라이언트 라이브러리는 프로그래밍 언어의 대부분을 지원. https://nats.io/download/ 
- 성숙한 콘텐츠
- Integrations
    - WebSockets, a Kafka bridge, an IBM MQ Bridge, a Redis Connector, Apache Spark, Apache Flink, CoreOS, Elastic, Elasticsearch, Prometheus, Telegraf, Logrus, Fluent Bit, Fluentd, OpenFAAS, HTTP, and MQTT (coming soon), and [more](https://nats.io/download/#connectors-and-utilities)
- 배신 이력 데이터로 재생 재배신 가능
    - 이력 데이터는 메모리 또는 세컨드리 스토리지에 저장되고, NATS에 기초한(NATS와 호환성 있는) 이벤트 스트리밍 서비스인 NATS Streaming을 사용하여 재생할 수 있다.    
- sub.Unsubscribe()을 호출하면 서브스크립션에 남아 있는 데이터를 완전하게 정리할 수 있다.
- Subjects는 관심(서브스크립션)을 토대로 동적으로 만들어지고, 삭제된다. 이것은 클라이언트가 서브스크립션 할때까지 서브젝트는 NATS 클러스터 내에 존재하지 않고, 최후에 서브스크라이브한 클라이언트가 이 서브젝트에서 서브스크라이브를 해제하면 서브젝트는 삭제 된다는 것을 의미한다.

  
## 각 MQ별 성능 비교
![NATS](./images/011.PNG)  
출처: https://nats.io/about/  

[다른 MQ류와 비교표](https://docs.nats.io/compare-nats  )  
  
  
## 실행
- [공식 문서](https://nats-io.github.io/docs/nats_server/running.html )
- 크로스 플랫폼 지원  
- [Installing](https://docs.nats.io/nats-server/installation)
    - docker로 한방에 실행 가능
    - 실행 파일 다운로드 후 실행
    - 직접 코드 빌드 후 실행
    - 실행 파일 및 코드 [다운로드](https://github.com/nats-io/nats-server/releases/ )
- [Running](https://docs.nats.io/nats-server/running)
- NATS를 실행하는 서버는 gnatsd 라고 부른다.
- 서버의 port 번호
    - :4222: client port
    - :6222: route port
    - :8222: http port

### 명령어
- 디폴트 서버 실행하기  
	<PRE>
	gnatsd
	</PRE>
- [Flags](https://docs.nats.io/nats-server/flags)
- 모니터링 기능 활성화 하기
	<PRE>
	gnatsd -m 8222
	</PRE>  
	8222 포트를 사용해 모니터링 기능을 제공한다
- 단일 사용자/패스워드 설정하기
	<PRE>
	gnatsd -DV --user someuser --pass somepassword
	</PRE> 
- 구성 파일에서 단일 사용자/패스워드 설정하기
	<PRE>
	authorization
	{
		user     : user1
		password : password1
		timeout  : 1
	}
	</PRE>	 
- 구성 파일에서 라우터 연결용 단일 사용자/패스워드 설정하기
	<PRE>
	cluster
	{
		authorization
		{
			user      : user1
			password  : password1
			timeout   : 0.5
		 }
	}
	</PRE>	 
- 구성 파일에서 복수 사용자/패스워드 설정하기
	<PRE>
	authorization
	{
		users =
		[
			{ user : user1, password : password1 }
			{ user : user2, password : password2 }
		]
	}
	</PRE>	 
- 사용자/패스워드를 사용해 서버 연결 문자열 만들기
	<PRE>
	nats://user1:password1@192.168.29.100:4222
	</PRE>	 
- 구성 파일을 사용해 서버 실행하기
	<PRE>
	d:\NATS\Server01\Bin\gnatsd.exe --config d:\NATS\Server01\Config\server.conf
	</PRE>	 
- 서버 권한 설정하기
	```
	authorization
	{
		ADMIN =
		{
			publish    = ">"
			subscriber = ">"
		}

		REQUESTOR =
		{
			publish    = ["req.foo", "req.bar"]
			subscriber = "_INBOX.>"
		}

		RESPONDER =
		{
			publish    = "_INBOX.>"
			subscriber = ["req.foo", "req.bar"]
		}

		DEFAULT_PERMISSIONS =
		{
			publish    = "SANDBOX.*"
			subscriber = ["PUBLIC.>", "_INBOX.>"]
		}

		PASS : abcdefghijklmnopqrstuvwxyz0123456789

		users =
		{
			{ user : joe    , password : foo  , permissions : $ADMIN     }
			{ user : alice  , password : bar  , permissions : $REQUESTOR }
			{ user : bob    , password : $PASS, permissions : $RESPONDER }
			{ user : charlie, password : bar                             }
		}
	}
	``` 
	- joe는 ADMIN 권한을 갖는다. 모든 주제를 발행하고 구독할 수 있다. 
	- alice는 REQUESTOR 권한을 갖는다. "req.foo", "req.bar" 주제를 발행할 수 있다. "_INBOX.>" 주제를 구독할 수 있다.  
	- bob는 RESPONDER 권한을 갖는다. "_INBOX.>" 주제를 발행할 수 있다. "req.foo", "req.bar" 주제를 구독할 수 있다.  
	- charlie는 권한이 없어서 디폴트 권한을 갖는다.  
    	  	
  

## 핵심 설명  
[출처](https://www.slideshare.net/hayahitokawamitsu/with-nats-with-kubernetes )    
  
![NATS](./images/001.PNG)    
![NATS](./images/002.PNG)    
![NATS](./images/003.PNG)    
![NATS](./images/004.PNG)    
![NATS](./images/005.PNG)    
![NATS](./images/006.PNG)    
![NATS](./images/007.PNG)    
![NATS](./images/008.PNG)    
![NATS](./images/009.PNG)    
![NATS](./images/010.PNG)    
  

## 벤치 마크에서 성능에 영향을 주는 요소
- 클러스터를 구성하는 NATS 서버의 대수
- 클러스터를 구성하는 VM 시스템 사양
- 클러스터의 네트워크 대역
- 게시자 수
- 구독자 수
- 게시자를 실행하는 VM 시스템 사양
- 구독자가 실행하는 VM 시스템 사양
- 발행되는 메시지의 수
- 발행되는 메시지의 크기  
    
[벤치마크 툴](https://docs.nats.io/nats-tools/natsbench)	   
      

  
## NATS 프로토콜의 기본 
  
### 서브젝트 이름
메시지의 전달 대상을 결정하는 ID 같은 것. 대소 문자를 구분하고 . 로 토큰을 구분한다.  
  
`subj.hello.one` , `SUBJ.HELLO.one` 등등.  
  
서브젝트에 와일드 카드를 사용할 수 있다.  
- `*` : 뭐든지 일치한다. `subj.hello.* SUBJ.*`
- `>` : `subj.>`의 경우 `subj.hello` `subj.hoge.fuga`에 매치하지만 subj만으로는 매치하지 않는다.
  
`*` , `>` 모두 토큰의 일부에 사용할 수 없다. `su*j.hello`는 사용할 수 없다.(엄밀히 말하면 지정할 수는 있지만)  
  

### 프로토콜 메시지
NATS의 프로토콜에 규정 되어 있는 메시지는 아래와 같다.  
- INFO : 클라이언트와 서버에서 TCP/IP 연결이 확립된 후에 서버에서 전송 되는 정보
- CONNECT : 클라이언트에서 연결 정보를 보낸다
- PUB : 메시지 전달하기
- SUB : 메시지 구독하기
- UNSUB : 메시지 구독 중지
- MSG : 클라이언트의 실제 전달 메시지
- PING/PONG: 이른바 ping-pong
- +OK: 프로토콜 메시지에 제대로 응답. CONNECT에서 verbose 모드를 false로 결정하는 경우 생략된다.(대부분의 클라이언트에서 기본 OFF로 하고 있다)
- ERR : 오류가 발생했을 때 서버로부터 통지 되는 프로토콜
  
프로토콜에 대한 자세한 내용은 설명을 생략한다.   
매우 단순하기 때문에 [문서](https://nats-io.github.io/docs/nats_protocol/nats-protocol.html )를 읽으면 자세한 것은 알 수있다.  


## 메시징 모델
아래의 예제 코드는 Go를 사용한다.  
[Go 예제 코드](https://github.com/nats-io/nats.go/tree/master/examples )  
[go-nats-examples](https://github.com/nats-io/go-nats-examples)  
  
### Publish/Subscribe 
NATS의 Pub/Sub는 Redis에서 사용하는 일반적인 "토픽 기반"의 Pub/Sub 이다. NATS는 토픽을 Subject 라고 부른다.  
NATS의 Subject는 계층 구조를 취할 수 있고, .(점)로 구분하여 표현한다.   
Subscriber는 이 계층 구조의 일부가 와일드 카드로 *(별표)를 사용할 수 있다.  
또한 > 를 사용하여 하위 계층 모두를 표현할 수 있다.   
  
예를 들어, Subscriber가 foo.bar.*를 구독하는 경우 foo.bar.baz 및 버튼 foo.bar.qux 등의 메시지를 받을 수 있지만 foo.bar.baz.qux은 받을 수 없다. 한편, foo.bar.> 를 구독하는 경우 foo.bar.baz.qux도 받을 수 있다.  
  
**Publisher**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	if err := nc.Publish("subjectFoo", []byte("bodyBar")); err != nil {
		log.Fatal(err)
	}
}
```
  
**Subscriber**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	sub, err := nc.Subscribe("subjectFoo", callback)
	if err != nil {
		log.Fatal(err)
	}
	log.Printf("Subject: %s", sub.Subject)
	log.Printf("Queue: %s", sub.Queue)
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
   
동기 처리로 하고 싶은 경우는 *nats.Conn.SubscribeSync를 사용한다  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	log.Printf("Subject: %s", sub.Subject)
	log.Printf("Queue: %s", sub.Queue)
    
    sub, err := nc.Subscribe("subjectFoo", callback)
	if err != nil {
		log.Fatal(err)
	}
    
    for {
        msg, err := sub.NextMsgWithContext(context.Background())
        if err != nil {
            log.Fatal(err)
        }
        callback(msg)
    }
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
  
### Request/Reply
Request/Reply 모델은 대부분 Pub/Sub 모델이지만 Subscriber 측에서 회신을 기대한다는 점이 다르다.  
메시지를 보낼 때 함께 전달된 Subject에 Subscriber가 답장을 보내는 형태로 구현한다.   
Go 클라이언트의 경우 Subscriber 측은 callback 함수 내에서 회신을 반환하도록 구현하는 것 말고는 다른 큰 차이는 없다.   
한편 Publisher 측은 답장을 기다리기 때문에 *nats.Conn.Request 함수를 사용한다.  
    
**Request**  
```
package main

import (
	"log"
	"time"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	msg, err := nc.Request("subjectFoo", []byte("bodyBar"), 10*time.Second)
	if err != nil {
		log.Fatal(err)
	}
	log.Print(string(msg.Data))
}
```
  
**Reply**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	if _, err := nc.Subscribe("subjectFoo", callback); err != nil {
		log.Fatal(err)
	}
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()
	nc.Publish(message.Reply, []byte("ReplyBaz"))
}
```
    
이 구현에서는 여러 클라이언트가 있다면 먼저 도착한 회신만 사용된다.   
여러 클라이언트로부터의 회신을 충족시키고 싶다면, 다음과 같이 *nats.Conn.NewRespInbox를 사용한다.    
   
**Request(for multiple client)**  
```
package main

import (
	"log"

	nats "github.com/nats-io/go-nats"
)

func main() {
	nc, err := nats.Connect("localhost:4222")
	if err != nil {
		log.Fatal(err)
	}
	defer nc.Close()

	inbox := nc.NewRespInbox()
	if err := nc.PublishRequest("subjectFoo", inbox, []byte("bodyBar")); err != nil {
		log.Fatal(err)
	}
	if _, err := nc.Subscribe(inbox, callback); err != nil {
		log.Fatal(err)
	}
	ch := make(chan struct{})
	<-ch
}

func callback(message *nats.Msg) {
	log.Print(string(message.Data))
}
```
  
*nats.Conn.NewRespInbox는 회신용으로 사용할 수 있는 Subject를 생성해서 반환한다.  
이것을 *nats.Conn.PublishRequest 에 반환해서 회신을 대기한다.  
  
  
### Queueing
큐잉(Queueing) 그룹을 지정해서 서브 스크라이브 하면 같은 큐 그룹 내에서 메시지를 나누어서 배신한다.  
Pub/Sub와 다른 점은 Pub/Sub은 같은 서브젝트의 서브 스크라이버가 모두 동일한 메시지를 받고, 큐잉을 사용하면 같은 그룹의 서브 스크라이버는 나누어서 메시지를 받는다.  
  
- Queue-Sub 클라이언트 ([nats-qsub](https://github.com/nats-io/nats/blob/master/examples/nats-qsub.go))
  
Queue-Sub 클라이언트 2개(subject=example.one, queue-group=Q1)와 다른 큐 그룹의 Queue-Sub 클라이언트 2개(subject=example.one, queue-group=Q2)을 실행한다.  
  
<PRE>
$ ./nats-qsub example.one Q1                   # client-A1
$ ./nats-qsub example.one Q1                   # client-A2
$ ./nats-qsub example.one Q2                   # client-B1
$ ./nats-qsub example.one Q2                   # client-B2
</PRE>
  
Pub 클라이언트에서 메시지를 보내본다. 도중 client-B1의 접속을 끊고 이 때의 동작도 본다.  
    
<PRE>  
$ ./nats-pub example.one "Message 1"
$ # 여기에서 client-B1를 정지
$ ./nats-pub example.one "Message 2"
$ ./nats-pub example.one "Message 3"
$ ./nats-pub example.one "Message 4"
$ ./nats-pub example.one "Message 5"
$ ./nats-pub example.one "Message 6"
</PRE>
   
메시지를 보내었을 때 각각의 Queue-Sub 클라이언트의 출력은 아래에.  
  
client-A1  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q1] Pid[73099]: 'Message 1'
[#2] Received on [example.one] Queue[Q1] Pid[73099]: 'Message 6'
</PRE>
  
client-A2  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 2'
[#2] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 3'
[#3] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 4'
[#4] Received on [example.one] Queue[Q1] Pid[73137]: 'Message 5'
</PRE>
  
client-B1  
<PRE>
Listening on [example.one]
[#1] Received on [example.one] Queue[Q2] Pid[73173]: 'Message 1'
^C
</PRE>
  
client-B2  
<PRE>  
Listening on [example.one]
[#1] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 2'
[#2] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 3'
[#3] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 4'
[#4] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 5'
[#5] Received on [example.one] Queue[Q2] Pid[73212]: 'Message 6'
</PRE>
  
같은 큐 그룹 내에서는 어떤 하나의 서브 스크라이버에 메시지를 던지는 것을 알 수 있다.  
client-A1, A2의 동작을 보면 메시지를 분배는 간단한 라운드로빈은 아닌 것 같다.  
  
  
## 보안
  
### 패스워드 인증
gnatsd 에 `--user` 옵션, `--pass` 옵션 또는 `--auth` 옵션을 붙여서 시작하면 패스워드 인증 or 토큰 인증을 사용하게 된다.  
<PRE>  
$ gnatsd --auth hogetoken
</PRE>  
  
클라이언트를 접속해 본다  
<PRE>  
$ ./nats-sub subj
Can't connect: nats: authorization violation
gnatsd
[93190] 2016/07/15 21:16:48.334616 [ERR] ::1:56626 - cid:1 - Authorization Error
</PRE>  
  
접속 처에 토큰 정보를 붙이면 인증 성공으로 접속할 수 있다.  
<PRE>  
$ ./nats-sub -s "nats://hoge@127.0.0.1:4222" subj
Listening on [subj]
</PRE>  
  
패스워드 인증의 경우는 hoge 부분을 'user:pass' 처럼 하면 된다.  
  
  
### TLS
TLS를 사용하여 서버와 클라이언트의 인증과 통신 내용 자체를 암호화 할 수 있다.  
  
gnatsd의 설정 파일로 아래와 같이 tls.conf를 준비한다.  
CERT 파일 등은 nats-io/gnatsd의 test/configs/certs 디렉토리에 있는 것을 사용한다.  
  
tls.conf  
```
listen: 127.0.0.1:4443

tls {
  cert_file: "./certs/server-cert.pem"
  key_file:  "./certs/server-key.pem"
  ca_file:   "./certs/ca.pem"
  verify:    true
}
```
  
<PRE>
$ gnatsd -D -V -config tls.conf
Starting gnatsd on port 5000
[95447] 2016/07/15 21:48:55.717289 [INF] Starting nats-server version 0.9.0.beta
[95447] 2016/07/15 21:48:55.717450 [DBG] Go build version go1.6.2
[95447] 2016/07/15 21:48:55.717455 [INF] Listening for client connections on 127.0.0.1:4443
[95447] 2016/07/15 21:48:55.717660 [INF] TLS required for client connections
[95447] 2016/07/15 21:48:55.717674 [DBG] Server id is m4w0EmrXB1xPvE5eMxpyX1
[95447] 2016/07/15 21:48:55.717677 [INF] Server is ready
</PRE>
  
TLS를 사용하지 않는 클라이언트를 붙여 보면 실패한다.  
<PRE>
$ ./nats-sub -s "nats://127.0.0.1:4443" subj
Can't connect: nats: secure connection required
</PRE>
  
앞에 사용한 nats-sub.go에 아래의 변경을 더해서 TLS 접속을 해본다.  
  
nats-tls-sub.go.diff  
<PRE>
$ diff -u nats-sub.go nats-tls-sub.go
--- nats-sub.go 2016-07-15 16:09:03.000000000 +0900
+++ nats-tls-sub.go 2016-07-18 00:28:36.000000000 +0900
@@ -4,7 +4,10 @@
 package main

 import (
+   "crypto/tls"
+   "crypto/x509"
    "flag"
+   "io/ioutil"
    "log"
    "runtime"

@@ -33,7 +36,25 @@
        usage()
    }

-   nc, err := nats.Connect(*urls)
+   pool := x509.NewCertPool()
+   pemData, err := ioutil.ReadFile("./certs/ca.pem")
+   if err != nil {
+       log.Fatalf("read error for ca.pem: %v\n", err)
+   }
+   pool.AppendCertsFromPEM(pemData)
+
+   cert, err := tls.LoadX509KeyPair("./certs/client-cert.pem", "./certs/client-key.pem")
+   if err != nil {
+       log.Fatalf("tls.LoadX509KeyPair() error=%v", err)
+   }
+
+   config := &tls.Config{
+       ServerName:   "localhost",
+       Certificates: []tls.Certificate{cert},
+       RootCAs:      pool,
+       MinVersion:   tls.VersionTLS12,
+   }
+   nc, err := nats.Connect(*urls, nats.Secure(config))
    if err != nil {
        log.Fatalf("Can't connect: %v\n", err)
    }
</PRE>
  	
이것을 실행하면 접속할 수 있다.  
<PRE>
$ go build nats-tls-sub.go
$ ./nats-tls-sub -s "tls://127.0.0.1:4443" subj
Listening on [subj]
[99264] 2016/07/18 00:28:53.609262 [DBG] 127.0.0.1:62669 - cid:1 - Client connection created
[99264] 2016/07/18 00:28:53.609327 [DBG] 127.0.0.1:62669 - cid:1 - Starting TLS client connection handshake
[99264] 2016/07/18 00:28:53.670571 [DBG] 127.0.0.1:62669 - cid:1 - TLS handshake complete
[99264] 2016/07/18 00:28:53.670590 [DBG] 127.0.0.1:62669 - cid:1 - TLS version 1.2, cipher suite TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
[99264] 2016/07/18 00:28:53.670830 [TRC] 127.0.0.1:62669 - cid:1 - ->> [CONNECT {"verbose":false,"pedantic":false,"tls_required":true,"name":"","lang":"go","version":"1.2.2"}]
[99264] 2016/07/18 00:28:53.670919 [TRC] 127.0.0.1:62669 - cid:1 - ->> [PING]
[99264] 2016/07/18 00:28:53.670926 [TRC] 127.0.0.1:62669 - cid:1 - <<- [PONG]
[99264] 2016/07/18 00:28:53.671085 [TRC] 127.0.0.1:62669 - cid:1 - ->> [SUB subj  1]
[99264] 2016/07/18 00:28:53.671108 [TRC] 127.0.0.1:62669 - cid:1 - ->> [PING]
[99264] 2016/07/18 00:28:53.671114 [TRC] 127.0.0.1:62669 - cid:1 - <<- [PONG]
[99264] 2016/07/18 00:28:56.754385 [DBG] 127.0.0.1:62669 - cid:1 - Client connection closed
</PRE>
  
이외에 유저 마다 권한 설정이나 Bcrypt을 사용한 패스워드 해시화도 가능하다.  
  
  
## 모니터링
gnatsd 에 `-m` 포트 번호 옵션을 지정하면 모니터링을 유효화 할 수 있다.  
  
`top` 명령어와 비슷한 `nats-top` 명령어를 설치하면 일단 모니터링 할 수 있다.  
<PRE>
$ nats-top

NATS server version 0.9.0.beta (uptime: 8m25s)
Server:
  Load: CPU:  0.0%  Memory: 9.6M  Slow Consumers: 0
  In:   Msgs: 15  Bytes: 162  Msgs/Sec: 2.0  Bytes/Sec: 14
  Out:  Msgs: 6  Bytes: 54  Msgs/Sec: 1.0  Bytes/Sec: 2

Connections Polled: 2
  HOST                 CID      NAME            SUBS    PENDING     MSGS_TO     MSGS_FROM   BYTES_TO    BYTES_FROM  LANG     VERSION  UPTIME   LAST ACTIVITY
  ::1:61187            3                        0       0           0           6           0           63          go       1.2.2    5s       2016-07-14 12:21:07.254618678 +0900 JST
  ::1:61447            4                        0       0           0           3           0           36          go       1.2.2    2s       2016-07-14 12:21:08.129647296 +0900 JST
</PRE>
  
`nats-top` 이외에도 WebUI를 갖춘 툴 등도 있다.  
  
모니터링 기능 자체는 단순하므로 http://hostname:port/XXX 에 HTTP로 접근하면 JSON을 XXX에 따라서 매트릭스를 돌려준다.  
대상의 엔트리 포인트는 아래와 같다.  
- /vars : NATS 클라이언트의 접속 수와 CPU 사용률 등
- /connz : NATS 클라이언트 수나 이 각 클라이언트의 내용
- /subsz : 서브 스크라이버 수나 매치한 메미지 수, 파괴된 메시지 수 등
- /routez : 클라스터의 루트 관련
  
  
  
## 커넥터
다른 미들웨어와 연동 기능을 제공하는 커넥트 라는 기능이 있다.(단순한 미들웨어 간의 브릿지이므로 기능이라고 하기에 애매할 수도....)  

https://nats.io/download/  
   
공식에는 NATS Connector Framework 라는 Java로 구현된 기능이 있고, 이 프레임워크 상에서 동작하는 Redis와 NATS를 연결하는 플러그인도 동시에 제공하고 있다. 
- [fluent-plugin-nats](https://github.com/cloudfoundry-community/fluent-plugin-nats ) : Fluentd와 NATS을 연결하는 Fluentd 플러그인
- [Logstash](https://github.com/r3labs/nats_to_logstash ) : Logstash에 NATS 메시지를 던지는 커넥트
- [nats-proxy](https://github.com/sohlich/nats-proxy ) : HTTP/Websocket으로 NATS과 대화하는 커넥트
  

## NATS Streaming 실행
[여기](https://github.com/nats-io/nats-streaming-server/releases )에서 실행 파일을 다운로드 한다.  
실행 파일 이름은 **nats-streaming-server** 이다.  

클라이언트는 아래처럼 실행한다.  
<PRE>
$ git clone https://github.com/nats-io/go-nats-streaming.git
$ cd go-nats-streaming/examples
$ go build stan-pub.go
$ go build stan-sub.go
</PRE>
  
서버 실행  
<PRE>
$ ./nats-streaming-server -m 8222
[87442] 2016/07/15 20:14:50.605166 [INF] Starting nats-streaming-server[test-cluster] version 0.2.0
[87442] 2016/07/15 20:14:50.605457 [INF] Starting nats-server version 0.9.0.beta
[87442] 2016/07/15 20:14:50.605467 [INF] Listening for client connections on localhost:4222
[87442] 2016/07/15 20:14:50.607194 [INF] Server is ready
[87442] 2016/07/15 20:14:50.944737 [INF] STAN: Message store is MEMORY
[87442] 2016/07/15 20:14:50.944755 [INF] STAN: Maximum of 1000000 will be stored
</PRE>
  
nats-top을 확인하면 스트리밍 서버가 실행하는 시점에서 내부에서 클라스터용 클라이언트가 있다.  
gnatsd 상에서 돈다는 것이 여기서 알 수 있다.   
  
<PRE>  
NATS server version 0.9.0.beta (uptime: 4m49s)
Server:
  Load: CPU:  0.0%  Memory: 11.0M  Slow Consumers: 0
  In:   Msgs: 1  Bytes: 45  Msgs/Sec: 0.0  Bytes/Sec: 0
  Out:  Msgs: 0  Bytes: 0  Msgs/Sec: 0.0  Bytes/Sec: 0

Connections Polled: 1
  HOST                 CID      NAME            SUBS    PENDING     MSGS_TO     MSGS_FROM   BYTES_TO    BYTES_FROM  LANG     VERSION  UPTIME   LAST ACTIVITY
  127.0.0.1:65386      2        NATS-Streaming-Server-test-cluster 5       0           0           1           0           45          go       1.2.2    4m49s    2016-07-15 20:15:48.18
</PRE>
    
### Streaming Pub/Sub
Sub 클라이언트가 없는 상태에서 Pub 클라이언트에서 메시지를 보내 본다.  
<PRE>
$ ./stan-pub subject.hoge "Hello NATS Streaming"
Published [subject.hoge] : 'Hello NATS Streaming'
$ ./stan-pub subject.hoge "Hello NATS Streaming 2"
Published [subject.hoge] : 'Hello NATS Streaming 2'
</PRE>
  
여기에서 Sub 클라이언트를 실행한다.  
`--all` 옵션을 붙여서 모든 유효한 메시지를 받도록 한다.  
<PRE>  
$ ./stan-sub -id STANSUB1 --all subject.hoge
Connected to nats://localhost:4222 clusterID: [test-cluster] clientID: [STANSUB1]
subscribing with DeliverAllAvailable
Listening on [subject.hoge], clientID=[STANSUB1], qgroup=[] durable=[]
[#1] Received on [subject.hoge]: 'sequence:1 subject:"subject.hoge" data:"Hello NATS Streaming" timestamp:1468581922036801904 '
[#2] Received on [subject.hoge]: 'sequence:2 subject:"subject.hoge" data:"Hello NATS Streaming 2" timestamp:1468581932488709393 '
</PRE>  
  
NATS에서읭 Pub/Sub 에서는 퍼블리쉬한 시점에서 서브스크라이버 하지 않으면 메시지를 수신 할 수 없었지만 NATS Streaming에서는 수신할 수 있다.  
  
`--last` 옵션을 붙여서 최후의 메시지만을 받을 수도 있다.  
stan-sub.go의 옵션을 보면 메시지의 기한 설정이나 기한 체크해서 유효하지 않으면 수신하지 않도록 하는 것도 가능한 것 같다.  
  

### 메시지 저장
nats-streaming-server는 default로는 인메모리로 메시지 저장 하고 있지만 `-store FILE -dir ${DIRNAME}`을 지정하면 DIRNAME 디렉토리를 만들고 그 아래에 파일로 메시지를 저장 할 수 있다.  
  
<PRE>  
$ ./nats-streaming-server -m 8222 -store FILE -dir streaminglogs
            :
[91285] 2016/07/15 20:45:08.489025 [INF] STAN: Message store is FILE
            :
</PRE>
  			
<PRE>
$ ls -ltr streaminglogs/*
-rw-r--r--  1 hattori-h  staff  195  7 15 20:43 streaminglogs/server.dat
-rw-r--r--  1 hattori-h  staff   71  7 15 20:45 streaminglogs/clients.dat

streaminglogs/subject.hoge:
total 48
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 subs.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.5.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.4.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.3.dat
-rw-r--r--  1 hattori-h  staff   4  7 15 20:45 msgs.2.dat
-rw-r--r--  1 hattori-h  staff  62  7 15 20:45 msgs.1.dat
</PRE>
  
    

### 참고
- [NATS에 대해서 알아보고 테스트 해 보기](https://qiita.com/hhatto/items/81611940de547ce3bd08)  
  

## Receiving Messages 
https://docs.nats.io/using-nats/developer/receiving  
  
일반적으로 애플리케이션은 비동기 또는 동기식으로 메시지를 수신할 수 있다. NATS로 메시지를 수신하는 것은 라이브러리에 따라 달라질 수 있다.  
  
Go나 Java와 같은 일부 언어는 동기 및 비동기 API를 제공하는 반면, 다른 언어는 한 가지 유형의 구독만 지원할 수 있다.  
  
모든 경우의 구독 프로세스에는 클라이언트 라이브러리가 애플리케이션이 특정 주제에 관심이 있음을 NATS 시스템에 알리는 과정이 포함된다. 애플리케이션이 구독을 완료하면 구독을 취소하여 서버에 메시지 전송을 중단하도록 알린다.  
  
클라이언트는 일치하는 각 구독에 대해 메시지를 받게 되므로 연결에 동일하거나 겹치는 주제(예: foo 및 >)를 사용하는 여러 구독이 있는 경우 동일한 메시지가 클라이언트에 여러 번 전송된다.  
  
참고: 클라이언트 API(비동기) 구독 호출은 nats 서버에서 구독이 실제로 완전히 설정되기 전에 반환될 수 있다. 서버 수준에서 준비 중인 구독과 동기화해야 하는 경우 구독을 호출한 직후 연결에서 `Flush()`를 호출하자.
  
  
## Sending Messages 
https://docs.nats.io/using-nats/developer/sending   
  
NATS는 대상 제목, 선택적 회신 제목 및 바이트 배열을 포함하는 프로토콜을 사용하여 메시지를 주고받는다. 일부 라이브러리에서는 다른 데이터 형식을 바이트 단위로 변환하는 헬퍼를 제공할 수 있지만, NATS 서버는 모든 메시지를 불투명한 바이트 배열로 취급한다.  
  
모든 NATS 클라이언트는 메시지를 간단하게 전송할 수 있도록 설계되었다. 예를 들어 "updates" 제목에 "All is Well"이라는 문자열을 UTF-8 바이트 문자열로 보내려면 다음과 같이 하면 된다:  
   
```Go
nc, err := nats.Connect("demo.nats.io", nats.Name("API PublishBytes Example"))
if err != nil {
    log.Fatal(err)
}
defer nc.Close()

if err := nc.Publish("updates", []byte("All is Well")); err != nil {
    log.Fatal(err)
}
```


# JetStream
https://docs.nats.io/nats-concepts/jetstream   
  
NATS에는 `JetStream` 이라는 분산 지속성 시스템이 내장되어 있으며 기본 `핵심 NATS` 기능 및 서비스 품질 위에 새로운 기능과 더 높은 서비스 품질을 제공한다.  
  
JetStream은 nats 서버에 내장되어 있으며, 모든 클라이언트 애플리케이션에서 사용할 수 있도록 하기 위해 NATS 서버 중 1대(또는 1~2대의 동시 NATS 서버 장애에 대한 내결함성을 원하는 경우 3대 또는 5대)만 JetStream을 사용하도록 설정하면 된다.    

```
nats-server -js
```  
를 사용하여 JetStream을 활성화한다.  
    
JetStream은 오늘날 스트리밍 기술의 문제점으로 지적되는 복잡성, 취약성, 확장성 부족을 해결하기 위해 만들어졌다. 일부 기술은 다른 기술보다 이러한 문제를 더 잘 해결하지만, 현재 스트리밍 기술 중 진정한 멀티 테넌트, 수평적 확장성, 다양한 배포 모델을 지원하는 기술은 없다. 우리가 알고 있는 그 어떤 기술도 운영을 위한 완벽한 배포 가시성을 확보하면서 동일한 보안 컨텍스트에서 엣지에서 클라우드로 확장할 수 없다.    
  
## 목표 
- 시스템 설정이나 운용이 쉬우므로 관측 가능하다
- NATS2.0의 보안 모델로 안전하게 운용할 수 있다
- 시스템은 수평적으로 확장할 수 있어야 하며 높은 수집 속도에 적용할 수 있어야 한다.
- 시스템은 다양한 사용 사례를 지원해야 한다.
- 시스템이 자가 복구되고 항상 사용 가능해야 한다.
- 시스템에 핵심 NATS에 가까운 API가 있어야 한다.
- 시스템은 원하는 대로 NATS 메시지가 스트림의 일부가 될 수 있어야 한다.
- 시스템이 페이로드에 구애받지 않는 동작을 표시해야 한다.
- 시스템에 타사 종속성이 없어야 한다.
  
   
## 제한 사항
스트림은 아래의 제한이 있다
- 최대 메시지 연령
- 스트림 전체의 최대 크기(바이트 단위)
- 스트림 내의 최대 메시지 수
- 개개의 메시지의 최대 크기
- 폐기 정책 지정 가능. 제한에 도달해서 새로운 메시지가 스트림에 공개 되었을 때 이 새로운 메시지를 위해 공간을 확보하기 위해 현재 스트림에 있는 가장 오래된 메시지 또는 가장 새로운 메시지 중 어느쪽을 폐기할지 정할 수 있다.
- 또 임의의 시점에서 스트림에 정의할 수 있는 컨슈머의 수에 제한을 설정할 수도 있다
  

## 기능 리스트
- 윈도우 내에 1번만 배신하는 「At-Least-Once」
- 메시지를 보존하고, 시간 또는 시퀸스로 재생
- 와일드카드 대응
- 계정 인식
- 암호화
- 특정 메시지 클린화(GDPR)
- 수평방향 확장
- 스트림 영속화와 컨슈머에 의한 재생  
    
JetStream은 메시지 모음과 소비를 분산시키고, 같은 스트림에서 데이터를 소비하는 방법을 복수 제공하도록 설계 되어있다. 그래서 JetStream 기능은 서버 스트림과 서버 컨슈머로 구성하고 있다.    


## 키 값 저장소
JetStream은 지속성 계층이며, 스트리밍은 이 계층 위에 구축된 기능 중 하나에 불과하다.  
또 다른 기능(일반적으로 메시징 시스템에서 사용할 수 없거나 메시징 시스템과 연관되어 있지 않은)은 키와 연관된 값 메시지를 저장, 검색 및 삭제하고, 해당 키에서 발생하는 변경 사항을 감시(수신)하며, 특정 키에서 발생한 값(및 삭제)의 기록을 검색할 수 있는 기능인 JetStream 키 값 저장소이다.    
   
   
## 리플레이 정책
JetStream 컨슈머는 컨슈머 애플리케이션이 으느쪽을 수신하고 싶은지에 따라서 복수의 재생 정책을 지원하고 있다.  
  
- 스트림에 현재 저장되어 있는 모든 메시지, 즉 완전한 「재생」을 의미하고, 「재생 정책」(즉 재생 속도)를 아래 중에서 선택할 수 있다.
    - 인스턴드(컨슈머가 처리할 수 있는 속도로 메시지가 배신된다)
    - 오리지널(스트림에 공개된 속도로 컨슈머에 배신되는 것을 의미하고, 실제 트래픽의 스테이징 등에 아주 편리하다)
- 스트림에 저장된 마지막 메시지, 또는 각 서브젝트의 최대 메시지(스트림은 복수의 서브젝트를 묶을 수 있기 때문에)
- 특정 시퀸스 번호에서 시작한다
- 특정 개시 시각에서 시작한다  
  
   

## 리텐션 정책
각 스트림에 대응하여 어떤 리텐션을 할지를 선택할 수 있다.  
- 제한(default)
- interest(스트림 상 컨슈머가 존재하는 한 메시지를 스트림에 보존시킨다)
- 워크 큐(스트림을 공유 큐로서 사용하여 소비된 메시지는 큐에서 삭제)




# 샘플 코드 
  
## ChatApp
분산 서버 구조의 채팅 서비스이다.  
클라이언트와 서버 간에 네트워크 통신을 하는 서버는 GateWayServer 이며 `Golang`을 사용하였고, 그 이외의 백엔드 서버는 `C#`을 사용하였다.  







