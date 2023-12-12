using System;
using System.Collections.Generic;

namespace RGuang
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
        static Stack<List<T>> _listStack = new Stack<List<T>>(8);

        /// <summary>
        /// 出栈：获取一个List对象
        /// </summary>
        /// <returns></returns>
        public static List<T> Get()
        {
            if (_listStack.Count == 0)
            {
                return new List<T>(8);
            }
            return _listStack.Pop();
        }

        /// <summary>
        /// 入栈：将List对象Clear并回收到栈中
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            if (_listStack.Contains(toRelease))
            {
                throw new Exception("重复回收List");
            }
            _listStack.Push(toRelease);
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
