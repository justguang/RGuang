# UIOCPNet


### 命名空间：namespace RGuang.Kit.Net.IOCPNet



### 描述：基于IOCP封装的异步套接字通信,TokenPool(连接池)管理所有连接会话
网络消息收发使用的是：Socket.ReceiveAsync、Socket.SendAsync；
SocketAsyncEventArgs处理socket各种状态；
使用MemoryStream、BinaryFormatter序列化和反序列化；





### 【Client】：
using UIOCPNet;
IOCPNet<UIOCPNet_Example,NetMsg> client = new IOCPNet<UIOCPNet_Example, NetMsg>();
client.StartAsClient("192.168.1.100", 6688);





### 【Server】：
using UIOCPNet;
IOCPNet<UIOCPNet_Example, NetMsg> server = new IOCPNet<UIOCPNet_Example, NetMsg>();
server.StartAsServer("192.168.1.100", 6688, 1000);


### Example：
UIOCPNet_Example使用者自己定义的通信连接和网络消息接收处理类，需要继承IOCPToken，并实现抽象函数；
NetMsg使用者自己定义的网络消息协议类，需要继承IOCPMsg。



【Client】发送网络消息：client.token.SendMsg(xxx);
【Server】发送网络消息： server.GetTokenList().ForEach((c)=>{ c.SendMsg(xxx) });


