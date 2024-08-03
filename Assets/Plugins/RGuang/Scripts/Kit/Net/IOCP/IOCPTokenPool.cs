///********************************************
/// ClassName    ：  IOCPTokenPool
/// Description  ：  IOCP会话连接token缓存池
///********************************************/
/// </summary>
using System.Collections.Generic;

namespace RGuang.Kit.Net.IOCPNet
{
    /// <summary>
    /// IOCP会话连接token缓存池【使用的Stack缓存池的方式】
    /// </summary>
    public class IOCPTokenPool<T, K>
        where T : IOCPToken<K>, new()
        where K : IOCPMsg, new()
    {
        Stack<T> stk;
        /// <summary>
        /// 池子中剩余token的数量
        /// </summary>
        public int Size => stk.Count;


        /// <summary>
        /// 实例化一个会话缓存池
        /// </summary>
        /// <param name="capacity">设置池子初始容量</param>
        public IOCPTokenPool(int capacity)
        {
            this.stk = new Stack<T>(capacity);
        }


        /// <summary>
        /// 取用一个token
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            lock (stk)
            {
                return stk.Pop();
            }
        }

        /// <summary>
        /// 回收一个token
        /// </summary>
        /// <param name="token">要回收的token</param>
        public void Push(T token)
        {
            if (token == null)
            {
                IOCPTool.Warn("Push token to pool cannot be null.");
                return;
            }
            lock (stk)
            {
                stk.Push(token);
            }
        }

    }
}
