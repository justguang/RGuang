using System;

namespace RGuang.Kit
{
    /// <summary>
    /// 更安全的对象池，带有一定的约束
    /// </summary>
    public class SafeObjectPool<T> : Pool<T>, ISingleton where T : IPoolable, new()
    {
        #region Singleton
        void ISingleton.OnSingletonInit()
        {
        }
        protected SafeObjectPool() => _factory = new DefaultObjectFactory<T>();

        public static SafeObjectPool<T> Instance
        {
            get => SingletonProperty<SafeObjectPool<T>>.Instance;
        }
        public void Dispose()
        {
            SingletonProperty<SafeObjectPool<T>>.Dispose();
        }
        #endregion


        /// <summary>
        /// 获取、设置池内对象的数量
        /// </summary>
        /// <value>The max cache count.</value>
        public int MaxCacheCount
        {
            get { return _maxCount; }
            set
            {
                _maxCount = value;

                if (_cacheStack != null)
                {
                    if (_maxCount > 0)
                    {
                        if (_maxCount < _cacheStack.Count)
                        {
                            int removeCount = _cacheStack.Count - _maxCount;
                            while (removeCount > 0)
                            {
                                _cacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始配置最大和初始数量
        /// </summary>
        /// <param name="maxCount"></param>
        /// <param name="initCount"></param>
        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (int i = CurCount; i < initCount; i++)
                {
                    Recycle(new T());
                }
            }

        }

        /// <summary>
        /// 分配(获取)一个对象
        /// </summary>
        /// <returns></returns>
        public override T Allocate()
        {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>回收成功返回true</returns>
        public override bool Recycle(T obj)
        {
            if (obj == null || obj.IsRecycled)
            {
                return false;
            }

            if (_maxCount > 0)
            {
                if(_cacheStack.Count>= _maxCount)
                {
                    obj.OnRecycled();
                    return false;
                }
            }

            obj.IsRecycled = true;
            obj.OnRecycled();
            _cacheStack.Push(obj);
            return true;
        }


    }

}

