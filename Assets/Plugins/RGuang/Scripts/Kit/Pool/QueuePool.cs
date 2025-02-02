using System;
using System.Collections.Generic;

namespace RGuang.Kit
{
    /// <summary>
    /// 存储 Queue 对象池，用于优化减少new调用次数
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class QueuePool<T>
    {
        /// <summary>
        /// 栈对象：存储对象Queue
        /// 
        /// </summary>
        static Stack<Queue<T>> m_queStack = new Stack<Queue<T>>(8);

        /// <summary>
        /// 出栈：获取一个Queue对象
        /// </summary>
        /// <param name="capacity">初始容量【默认8】</param>
        /// <returns></returns>
        public static Queue<T> Get(int capacity = 8)
        {
            if (m_queStack.Count == 0)
            {
                return new Queue<T>(capacity);
            }
            return m_queStack.Pop();
        }
        public static Queue<T> Get(T obj, int capacity = 8)
        {
            var que = Get(capacity);
            que.Enqueue(obj);
            return que;
        }
        public static Queue<T> Get(IEnumerable<T> collection, int capacity = 8)
        {
            var que = Get(capacity);
            var iterator = collection.GetEnumerator();
            while (iterator.MoveNext())
            {
                que.Enqueue(iterator.Current);
            }
            return que;
        }

        /// <summary>
        /// 入栈：将Queue对象Clear并回收到栈中
        /// </summary>
        /// <param name="toRelease">回收对象</param>
        public static void Release(Queue<T> toRelease)
        {
            if (toRelease == null) return;
            toRelease.Clear();
            if (m_queStack.Contains(toRelease))
            {
                throw new Exception("重复回收Queue");
            }

            m_queStack.Push(toRelease);
        }

    }

    /// <summary>
    /// 对象池Queue 拓展方法
    /// </summary>
    public static class QueuePoolExtensions
    {
        /// <summary>
        /// 对Queue拓展 自身入池的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static void Release2Pool<T>(this Queue<T> self)
        {
            QueuePool<T>.Release(self);
        }
    }
}
