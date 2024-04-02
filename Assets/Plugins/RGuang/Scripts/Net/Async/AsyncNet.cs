/// <summary>
///********************************************
/// ClassName    ：  AsyncNet
/// Description  ：  异步Socket核心类
///********************************************/
/// </summary>
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace RGuang.Net.Async
{
    [Serializable]
    public abstract class AsyncMsg { }

    public class AsyncNet<T, K>
        where T : AsyncSession<K>, new()
        where K : AsyncMsg, new()
    {
        public T session;
        private Socket skt = null;
        public int backlog = 10;
        List<T> sessionLst = null;

        #region Client
        public void StartAsClient(string ip, int port)
        {
            try
            {
                skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                AsyncTool.ColorLog(AsyncLogColor.Green, "Client Start...");
                EndPoint pt = new IPEndPoint(IPAddress.Parse(ip), port);
                skt.BeginConnect(pt, new AsyncCallback(ServerConnectCB), null);

            }
            catch (Exception e)
            {
                AsyncTool.Error(e.Message);
            }
        }
        private void ServerConnectCB(IAsyncResult ar)
        {
            session = new T();
            try
            {
                skt.EndConnect(ar);
                if (skt.Connected)
                {
                    session.InitSession(skt, null);
                }
            }
            catch (Exception e)
            {
                AsyncTool.Error(e.ToString());
            }
        }
        public void CloseClient()
        {
            if (session != null)
            {
                session.CloseSession();
                session = null;
            }
            if (skt != null)
            {
                skt = null;
            }
        }
        #endregion

        #region Server
        public void StartAsServer(string ip, int port)
        {
            sessionLst = new List<T>();
            try
            {
                skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                skt.Listen(backlog);
                AsyncTool.ColorLog(AsyncLogColor.Green, "Server Start...");
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), null);
            }
            catch (Exception e)
            {
                AsyncTool.Error(e.Message);
            }
        }
        private void ClientConnectCB(IAsyncResult ar)
        {
            T session = new T();
            try
            {
                Socket clientSkt = skt.EndAccept(ar);
                if (clientSkt.Connected)
                {
                    lock (sessionLst)
                    {
                        sessionLst.Add(session);
                    }
                    session.InitSession(clientSkt, () => {
                        if (sessionLst.Contains(session))
                        {
                            lock (sessionLst)
                            {
                                if (sessionLst.Remove(session))
                                {
                                    AsyncTool.ColorLog(AsyncLogColor.Yellow, "Clear ServerSession Success.");
                                }
                                else
                                {
                                    AsyncTool.ColorLog(AsyncLogColor.Yellow, "Clear ServerSession Fail.");
                                }
                            }
                        }
                    });
                }
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), null);
            }
            catch (Exception e)
            {
                AsyncTool.Error("ClientConnectCB:{0}", e.Message);
            }

        }
        public List<T> GetSessionLst()
        {
            return sessionLst;
        }
        public void CloseServer()
        {
            for (int i = 0; i < sessionLst.Count; i++)
            {
                sessionLst[i].CloseSession();
            }
            sessionLst = null;
            if (skt != null)
            {
                skt.Close();
                skt = null;
            }
        }
        #endregion
    }
}
