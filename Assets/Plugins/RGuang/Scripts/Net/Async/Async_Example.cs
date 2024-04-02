/// <summary>
///********************************************
/// ClassName    ：  Async_Example
/// Description  ：  AsyncNet 使用示例
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;

namespace RGuang.Net.Async
{
    [Serializable]
    public class NetMsg : AsyncMsg
    {
        public string msg;
    }

    public class Async_Example
    {
        static void Main(string[] args)
        {

        }


        public static void StartClient()
        {
            AsyncNet<Client, NetMsg> client = new AsyncNet<Client, NetMsg>();
            client.StartAsClient("192.168.1.100", 17666);
            AsyncTool.ColorLog(AsyncLogColor.Yellow, "Input 'quit' to stop client.");

            while (true)
            {
                string ipt = Console.ReadLine();
                if (ipt == "quit")
                {
                    client.CloseClient();
                    break;
                }
                else
                {
                    NetMsg msg = new NetMsg
                    {
                        msg = ipt
                    };
                    client.session.SendMsg(msg);
                }
            }
        }



        public static void StartServer()
        {
            AsyncNet<Sever, NetMsg> server = new AsyncNet<Sever, NetMsg>();
            server.StartAsServer("192.168.1.100", 17666);

            AsyncTool.ColorLog(AsyncLogColor.Yellow, "Input 'quit' to stop server.");

            while (true)
            {
                string ipt = Console.ReadLine();
                if (ipt == "quit")
                {
                    server.CloseServer();
                    break;
                }
                else
                {
                    NetMsg msg = new NetMsg
                    {
                        msg = ipt
                    };
                    byte[] data = AsyncTool.PackLenInfo(AsyncTool.Serialize(msg));
                    List<Sever> sessionLst = server.GetSessionLst();
                    for (int i = 0; i < sessionLst.Count; i++)
                    {
                        sessionLst[i].SendMsg(data);
                    }
                }
            }

        }

    }


    #region 客户端

    public class Client : AsyncSession<NetMsg>
    {
        protected override void OnConnected(bool result)
        {
            AsyncTool.ColorLog(AsyncLogColor.Green, "Connect Server:" + result);
        }

        protected override void OnDisConnected()
        {
            AsyncTool.Warn("DisConnect to Server.");
        }

        protected override void OnReceiveMsg(NetMsg msg)
        {
            AsyncTool.Log("RcvServerMsg:" + msg.msg);
        }

    }

    #endregion


    #region 服务端
    public class Sever : AsyncSession<NetMsg>
    {
        protected override void OnConnected(bool result)
        {
            AsyncTool.ColorLog(AsyncLogColor.Green, "Client Online:" + result);
        }

        protected override void OnDisConnected()
        {
            AsyncTool.Warn("Client Offline.");
        }

        protected override void OnReceiveMsg(NetMsg msg)
        {
            AsyncTool.Log("RcvClientMsg:" + msg.msg);
        }
    }


    #endregion


}
