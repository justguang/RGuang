using System;
using System.Collections.Generic;

namespace RGuang.Kit
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
        static Stack<Dictionary<TKey, TValue>> s_DicStack = new Stack<Dictionary<TKey, TValue>>(8);

        /// <summary>
        /// 出栈：从栈中获取某个字典数据
        /// </summary>
        /// <param name="capacity">初始容量【默认8】</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Get(int capacity = 8)
        {
            if (s_DicStack.Count == 0)
            {
                return new Dictionary<TKey, TValue>(capacity);
            }
            return s_DicStack.Pop();
        }
        public static Dictionary<TKey, TValue> Get(KeyValuePair<TKey, TValue> obj, int capacity = 8)
        {
            var dic = Get(capacity);
            dic.Add(obj.Key, obj.Value);
            return dic;
        }
        public static Dictionary<TKey, TValue> Get(ICollection<KeyValuePair<TKey, TValue>> collection, int capacity = 8)
        {
            var dic = Get(capacity);
            var iterator = collection.GetEnumerator();
            while (iterator.MoveNext())
            {
                dic.Add(iterator.Current.Key, iterator.Current.Value);
            }
            return dic;
        }

        /// <summary>
        /// 入栈：将字典Clear并存储到栈中 
        /// 并检查是否重复回收
        /// </summary>
        /// <param name="toRelease">回收对象</param>
        public static void Release(Dictionary<TKey, TValue> toRelease)
        {
            if (toRelease == null) return;
            toRelease.Clear();
            if (s_DicStack.Contains(toRelease))
            {
                throw new Exception("重复回收Dictionary");
            }
            s_DicStack.Push(toRelease);
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
        /// <param name="self"></param>
        public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> self)
        {
            DictionaryPool<TKey, TValue>.Release(self);
        }
    }

}
