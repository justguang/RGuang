using System;
using System.Collections.Generic;

namespace RGuang.Kit
{
    /// <summary>
    /// 存储 List 对象池，用于优化减少new调用次数
    /// 
    /// </summary>
    public static class ListPool<T>
    {
        /// <summary>
        /// 栈对象：存储对象List
        /// </summary>
        static Stack<List<T>> s_ListStack = new Stack<List<T>>(8);

        /// <summary>
        /// 出栈：获取一个List对象
        /// </summary>
        /// <param name="capacity">初始容量【默认8】</param>
        /// <returns></returns>
        public static List<T> Get(int capacity = 8)
        {
            if (s_ListStack.Count == 0)
            {
                return new List<T>(capacity);
            }
            return s_ListStack.Pop();
        }
        public static List<T> Get(T obj, int capacity = 8)
        {
            var lst = Get(capacity);
            lst.Add(obj);
            return lst;
        }
        public static List<T> Get(IEnumerable<T> collection, int capacity = 8)
        {
            var lst = Get(capacity);
            lst.AddRange(collection);
            return lst;
        }

        /// <summary>
        /// 入栈：将List对象Clear并回收到栈中
        /// 并检查是否重复回收
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(List<T> toRelease)
        {
            if (toRelease == null) return;
            toRelease.Clear();
            if (s_ListStack.Contains(toRelease))
            {
                throw new Exception("重复回收List");
            }
            s_ListStack.Push(toRelease);
        }
    }

    /// <summary>
    /// 对象池List 拓展方法
    /// </summary>
    public static class ListPoolExtensions
    {
        /// <summary>
        /// 对List拓展 自身入池的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        public static void Release2Pool<T>(this List<T> self)
        {
            ListPool<T>.Release(self);
        }


    }

}
