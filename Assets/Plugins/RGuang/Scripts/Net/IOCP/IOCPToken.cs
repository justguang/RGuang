/// <summary>
///********************************************
/// ClassName    ：  IOCPToken
/// Author       ：  LCG
/// CreateTime   ：  2022/6/20 星期一
/// Description  ：  IOCP连接会话Token(session)
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RGuang.Net.IOCPNet
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum TokenState
    {
        None,
        /// <summary>
        /// 连接
        /// </summary>
        Connected,
        /// <summary>
        /// 断开连接
        /// </summary>
        DisConnected,
    }

    /// <summary>
    /// IOCP连接会话Token(session)；
    /// 网络消息接收方式：Socket.ReceiveAsync；
    /// 网络消息发送方式：Socket.SendAsync；
    /// 消息接收缓冲2048个字节。
    /// </summary>
    public abstract class IOCPToken<T> where T : IOCPMsg, new()
    {
        /// <summary>
        /// token ID
        /// </summary>
        public int tokenID;
        /// <summary>
        /// token的连接状态
        /// </summary>
        public TokenState tokenState = TokenState.None;

        /// <summary>
        /// 当token关闭并回收时回调
        /// </summary>
        public Action<int> onTokenClose;

        Socket skt;
        SocketAsyncEventArgs rcvSAEA;//接收数据事件
        SocketAsyncEventArgs sndSAEA;//发送数据事件
        Queue<byte[]> cacheQue = new Queue<byte[]>();//发送时缓存的队列
        List<byte> readList = new List<byte>();//接收到的网络消息
        bool isWrite = false;//是否在写入(发送)数据


        public IOCPToken()
        {
            rcvSAEA = new SocketAsyncEventArgs();
            sndSAEA = new SocketAsyncEventArgs();
            rcvSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            sndSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            rcvSAEA.SetBuffer(new byte[2048], 0, 2048);
        }

        public void InitToken(Socket skt)
        {
            this.skt = skt;
            this.tokenState = TokenState.Connected;
            OnConnected();
            StartAsyncRcv();
        }

        #region 处理接收
        //开始异步接收数据
        void StartAsyncRcv()
        {
            bool suspend = skt.ReceiveAsync(rcvSAEA);
            if (suspend == false)
            {
                ProcessRcv();
            }
        }

        //接收到数据
        void ProcessRcv()
        {
            if (rcvSAEA.BytesTransferred > 0 && rcvSAEA.SocketError == SocketError.Success)
            {
                byte[] bytes = new byte[rcvSAEA.BytesTransferred];
                Buffer.BlockCopy(rcvSAEA.Buffer, 0, bytes, 0, rcvSAEA.BytesTransferred);
                readList.AddRange(bytes);

                //处理接收的数据
                ProcessByteList();
                //继续开始异步接收数据
                StartAsyncRcv();
            }
            else
            {
                IOCPTool.Warn("Token:{0} Close:{1}", tokenID, rcvSAEA.SocketError.ToString());
                CloseToken();
            }
        }

        //使用递归处理接收的数据
        void ProcessByteList()
        {
            byte[] buff = IOCPTool.SplitLogicBytes(ref readList);
            if (buff != null)
            {
                T msg = IOCPTool.DesSerialize<T>(buff);
                OnReceiveMsg(msg);
                ProcessByteList();
            }
        }
        #endregion

        #region 处理发送
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="msg">消息体</param>
        /// <returns>返回true发送完毕，反之false发送有误</returns>
        public bool SendMsg(T msg)
        {
            if (msg != null)
            {
                byte[] data = IOCPTool.PackLenInfo(IOCPTool.Serialize(msg));
                return SendMsg(data);
            }
            return false;
        }

        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="bytes">序列化后的消息体</param>
        /// <returns>返回true发送结束，false发送有误</returns>
        public bool SendMsg(byte[] bytes)
        {
            if (tokenState != TokenState.Connected)
            {
                IOCPTool.Warn("Connection is break, cannot send net msg.");
                return false;
            }
            if (isWrite)
            {
                //如果上一个发送还没结束，就将这一次的发送缓存下来
                cacheQue.Enqueue(bytes);
                return true;
            }

            isWrite = true;
            sndSAEA.SetBuffer(bytes, 0, bytes.Length);
            bool suspend = skt.SendAsync(sndSAEA);
            if (suspend == false)
            {
                ProcessSend();
            }
            return true;
        }

        //发送网络消息结束
        void ProcessSend()
        {
            if (sndSAEA.SocketError == SocketError.Success)
            {
                //发送成功
                isWrite = false;
                if (cacheQue.Count > 0)
                {
                    //发送成功，如果缓存中还有待发送的数据则继续发送
                    byte[] item = cacheQue.Dequeue();
                    SendMsg(item);
                }
            }
            else
            {
                //发送错误
                IOCPTool.Error("Process Send Error:{0}", sndSAEA.SocketError.ToString());
                CloseToken();
            }
        }
        #endregion


        //事件，处理接收和发送
        void IO_Completed(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessRcv();
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend();
                    break;
                default:
                    IOCPTool.Warn("The last operation completed on the socket was not a receive or send op.");
                    break;
            }
        }

        //关闭会话
        public void CloseToken()
        {
            if (skt != null)
            {
                tokenState = TokenState.DisConnected;
                OnDisConnected();
                onTokenClose?.Invoke(tokenID);

                readList.Clear();
                cacheQue.Clear();
                isWrite = false;

                try
                {
                    skt.Shutdown(SocketShutdown.Send);
                }
                catch (Exception e)
                {
                    IOCPTool.Error("Shutdown Socket Error:{0}", e.ToString());
                }
                finally
                {
                    skt.Close();
                    skt = null;
                }
            }
        }


        //收到并解析后的数据
        protected abstract void OnReceiveMsg(T msg);

        //套接字断开连接
        protected abstract void OnDisConnected();

        //套接字连接成功
        protected abstract void OnConnected();



    }
}
