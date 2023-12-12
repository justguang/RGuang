using System;
using System.Collections.Generic;

namespace RGuang
{
    public abstract class Pool<T> : IPool<T>
    {

        /// <summary>
        /// 存储相关数据的栈
        /// </summary>
        protected readonly Stack<T> _cacheStack = new Stack<T>();
        /// <summary>
        /// default MaxCount
        /// </summary>
        protected int _maxCount = 8;
        /// <summary>
        /// 对象工厂
        /// </summary>
        protected IObjectFactory<T> _factory;
        /// <summary>
        /// 当前数量
        /// </summary>
        public int CurCount
        {
            get => _cacheStack.Count;
        }
        /// <summary>
        /// 设置对象创建工厂
        /// </summary>
        /// <param name="factory"></param>
        public void SetObjectFactory(IObjectFactory<T> factory)
        {
            _factory = factory;
        }
        /// <summary>
        /// 设置自定义创建方法的对象工厂
        /// </summary>
        /// <param name="factoryMethod"></param>
        public void SetFactoryMethod(Func<T> factoryMethod)
        {
            _factory = new CustomObjectFactory<T>(factoryMethod);
        }
        public void Clear(Action<T> onClearItem = null)
        {
            if (onClearItem != null)
            {
                var iterator = _cacheStack.GetEnumerator();
                while (iterator.MoveNext())
                {
                    onClearItem(iterator.Current);
                }
            }
            _cacheStack.Clear();
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public virtual T Allocate()
        {
            return _cacheStack.Count == 0 ? _factory.Create() : _cacheStack.Pop();
        }
        /// <summary>
        /// 回收一个对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>回收成功返回true</returns>
        public abstract bool Recycle(T obj);
    }

}
