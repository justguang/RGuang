/// <summary>
///********************************************
/// ClassName    ：  IOCPServer
/// Author       ：  LCG
/// CreateTime   ：  2022/6/20 星期一 
/// Description  ：  基于IOCP封装的异步套接字通信
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RGuang.Net.IOCPNet
{
    /// <summary>
    /// 网络消息数据协议
    /// </summary>
    [Serializable]
    public abstract class IOCPMsg { }


    /// <summary>
    /// 基于IOCP封装的异步套接字通信【服务端】；
    /// 套接字类型 => Stream；
    /// 协议 => Tcp；
    /// 接收连接方式 => Socket.AcceptAsync。
    /// </summary>
    public class IOCPNet<T, K>
        where T : IOCPToken<K>, new()
        where K : IOCPMsg, new()
    {
        Socket skt;
        SocketAsyncEventArgs saea;//事件
        public IOCPNet()
        {
            saea = new SocketAsyncEventArgs();
            saea.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
        }


        #region Server
        public int backlog = 100;
        int curConnCount = 0;//当前连接数量
        IOCPTokenPool<T, K> tokenPool;//会话token缓存池
        List<T> tokenList;//管理已连接的token
        Semaphore acceptSemaphore;//连接信号量限制

        /// <summary>
        /// 启动server
        /// </summary>
        /// <param name="ip">绑定的ip</param>
        /// <param name="port">端口</param>
        /// <param name="maxConnectCount">根据物理设备不同设置最大连接数</param>
        public void StartAsServer(string ip, int port, int maxConnectCount)
        {
            curConnCount = 0;
            acceptSemaphore = new Semaphore(maxConnectCount, maxConnectCount);
            tokenPool = new IOCPTokenPool<T, K>(maxConnectCount);
            tokenList = new List<T>();
            for (int i = 0; i < maxConnectCount; i++)
            {
                T token = new T { tokenID = i + 1 };
                tokenPool.Push(token);
            }


            IPEndPoint pt = new IPEndPoint(IPAddress.Parse(ip), port);
            skt = new Socket(pt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            bool IsStart = false;
            try
            {
                skt.Bind(pt);
                skt.Listen(backlog);
                IsStart = true;
            }
            catch (Exception e)
            {
                IOCPTool.Error(e.ToString());
                //
            }

            if (IsStart)
            {
                IOCPTool.ColorLog(IOCPLogColor.Green, "Server Start...   port[{0}]", port);
                StartAccept();
            }
        }
        //开始异步接收连接
        void StartAccept()
        {
            saea.AcceptSocket = null;
            acceptSemaphore.WaitOne();
            bool suspend = skt.AcceptAsync(saea);
            if (suspend == false)
            {
                ProcessAccept();
            }
        }

        //有个连接进来
        void ProcessAccept()
        {
            Interlocked.Increment(ref curConnCount);
            T token = tokenPool.Pop();

            lock (tokenList)
            {
                tokenList.Add(token);
            }

            token.InitToken(saea.AcceptSocket);
            token.onTokenClose = OnTokenClose;
            IOCPTool.ColorLog(IOCPLogColor.Green, "Client Online, Allocate TokenID:{0}", token.tokenID);
            StartAccept();
        }
        //有个连接断开
        void OnTokenClose(int tokenID)
        {
            //根据tokenID找出对应索引
            int index = -1;
            int len = tokenList.Count;
            for (int i = 0; i < len; i++)
            {
                if (tokenList[i].tokenID == tokenID)
                {
                    index = i;
                    break;
                }
            }

            //回收token
            if (index != -1)
            {
                tokenPool.Push(tokenList[index]);
                lock (tokenList)
                {
                    tokenList.RemoveAt(index);
                }
                Interlocked.Decrement(ref curConnCount);
                acceptSemaphore.Release();
            }
            else
            {
                IOCPTool.Error("TokenID:{0} cannot find in server tokenList.", tokenID);
            }
        }
        /// <summary>
        /// 获取当前所有连接
        /// </summary>
        public List<T> GetTokenList()
        {
            return tokenList;
        }
        /// <summary>
        /// 关闭server
        /// </summary>
        public void CloseServer()
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                tokenList[i].CloseToken();
            }
            tokenList = null;

            if (skt != null)
            {
                skt.Close();
                skt = null;
            }
        }
        #endregion


        #region Client
        public T token;

        /// <summary>
        /// 启动client
        /// </summary>
        /// <param name="ip">连接的ip</param>
        /// <param name="port">端口</param>
        public void StartAsClient(string ip, int port)
        {
            IPEndPoint pt = new IPEndPoint(IPAddress.Parse(ip), port);
            skt = new Socket(pt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            saea.RemoteEndPoint = pt;

            IOCPTool.ColorLog(IOCPLogColor.Green, "Client Start...   port[{0}]", port);
            StartConnect();
        }
        /// <summary>
        /// 关闭client
        /// </summary>
        public void CloseClient()
        {
            if (token != null)
            {
                token.CloseToken();
                token = null;
            }
            if (skt != null)
            {
                skt = null;
            }
        }
        //开始异步连接
        void StartConnect()
        {
            bool suspend = skt.ConnectAsync(saea);
            if (suspend == false)
            {
                ProcessConnect();
            }
            else
            {
                IOCPTool.Log("连接挂起");
            }
        }
        void ProcessConnect()
        {
            //IOCPTool.Log("连接成功");
            token = new T();
            token.InitToken(skt);
        }
        #endregion


        //事件
        void IO_Completed(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept();
                    break;
                case SocketAsyncOperation.Connect:
                    ProcessConnect();
                    break;
                default:
                    IOCPTool.Warn("The last operation completed on the socket was not a Accept or Connect op.");
                    break;
            }
        }
    }
}
