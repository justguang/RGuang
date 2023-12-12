
namespace RGuang
{
    /// <summary>
    /// 通过属性实现的 Singleton
    /// </summary>
    public static class SingletonProperty<T> where T:class,ISingleton
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        private static T _instance;
        /// <summary>
        /// 标签锁
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 静态属性
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = SingletonCreator.CreateSingleton<T>();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Dispose()
        {
            _instance = null;
        }

    }

}
