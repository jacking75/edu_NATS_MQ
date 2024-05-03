# NATS 클라이언트 C#

[A C# Client for NATS](https://github.com/nats-io/nats.net )  
- Publish는 스레드 세이프 하다.
    - Connection.cs 파일의 `internal void PublishImpl(string subject, string reply, MsgHeader inHeaders, byte[] data, int offset, int? inCount, bool flushBuffer)` 함수를 보면 내부에서 lock을 걸고 있다.
- Publish를 호출하면 socket 쓰기를 한다. 동기 I/O
    
    
## 예제 코드  
다음 코드는 “foo” 라는 이름으로 Company 객체를 메시지 버스로 보내는 방법을 보여준다.  
```
using (var cnx = new ConnectionFactory().CreateEncodedConnection())
{
    cnx.Publish("foo", new Company
    {
        Name = "Apcera",
        Address = "140 New Montgomery St."
    });
}
```
  
이 메시지를 처리하는 쪽에서는 “foo” 라는 이름의 메시지들을 비동기적으로 수신하여 처리한다.  
```
using (var cnx = new ConnectionFactory().CreateEncodedConnection())
{
    using (cnx.SubscribeAsync(
        "foo",
        (sender, args) => {
            var company = (Company)args.ReceivedObject;
            Console.WriteLine($"Name: {company.Name}, Address: {company.Address}");
        }))
    {
        System.Console.WriteLine("Waiting for a message...");
        Thread.Sleep(5000);
    }
}
```  
  