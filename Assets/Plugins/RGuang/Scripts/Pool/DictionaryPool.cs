using System;
using System.Collections.Generic;

namespace RGuang
{
    /// <summary>
    /// 存储 Dictionary 对象池，用于优化减少 new 调用次数。
    /// 
    /// </summary>
    public static class DictionaryPool<TKey, TValue>
    {
        /// <summary>
        /// 栈对象：存储多个Dictionary
        /// </summary>
        static Stack<Dictionary<TKey, TValue>> _listStack = new Stack<Dictionary<TKey, TValue>>(8);

        /// <summary>
        /// 出栈：从栈中获取某个字典数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Get()
        {
            if (_listStack.Count == 0)
            {
                return new Dictionary<TKey, TValue>(8);
            }
            return _listStack.Pop();
        }

        /// <summary>
        /// 入栈：将字典Clear并存储到栈中 
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(Dictionary<TKey, TValue> toRelease)
        {
            if (toRelease == null) return;
            toRelease.Clear();
            if (_listStack.Contains(toRelease))
            {
                throw new Exception("重复回收Dictionary");
            }
            _listStack.Push(toRelease);
        }

    }

    /// <summary>
    /// 对象池字典拓展方法
    /// </summary>
    public static class DictionaryPoolExtensions
    {
        /// <summary>
        /// 对字典拓展 自身入池的方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="toRelease"></param>
        public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> toRelease)
        {
            DictionaryPool<TKey, TValue>.Release(toRelease);
        }
    }

}
