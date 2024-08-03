
namespace RGuang.Kit
{
    /// <summary>
    /// C# 单例类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        protected static T m_ins;

        /// <summary>
        /// 标签锁：确保线程安全
        /// </summary>
        static readonly object m_lock = new object();

        public static T Instance
        {
            get
            {
                if (m_ins == null)
                {
                    lock (m_lock)
                    {
                        if (m_ins == null)
                        {
                            m_ins = SingletonCreator.CreateSingleton<T>();
                        }
                    }
                }
                return m_ins;
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            m_ins = null;
        }

        /// <summary>
        /// 单例初始化方法
        /// </summary>
        public virtual void OnSingletonInit()
        {
        }
    }

}
