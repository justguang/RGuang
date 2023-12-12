using System;

namespace RGuang
{
    /// <summary>
    /// 普通对象池，面向业务
    /// </summary>
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> _resetMethod;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="factoryMethod2CreateObject">自定义格式工厂调用方法，用于对象创建</param>
        /// <param name="resetMethod2RecycleObject">重置方法，用于回收对象时调用</param>
        /// <param name="initCount">初始对象个数</param>
        public SimpleObjectPool(Func<T> factoryMethod2CreateObject,
                                Action<T> resetMethod2RecycleObject = null,
                                int initCount = 0)
        {
            SetFactoryMethod(factoryMethod2CreateObject);

            _resetMethod = resetMethod2RecycleObject;
            for (int i = 0; i < initCount; i++)
            {
                _cacheStack.Push(_factory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (_cacheStack.Contains(obj))
            {
                throw new Exception("重复回收对象 => " + obj);
            }

            _resetMethod?.Invoke(obj);

            _cacheStack.Push(obj);

            return true;
        }
    }
}
