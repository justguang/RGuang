using System;
using System.Collections.Generic;

namespace RGuang.Kit
{
    /// <summary>
    /// 引用接口
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 清理引用
        /// </summary>
        void ClearReference();
    }


    /// <summary>
    /// 引用池
    /// </summary>
    public static class ReferencePool
    {
        /**
         * 管理所有引用池
         * 
         * key => 类型
         * 
         * value => 引用池
         * 
         */
        private static readonly Dictionary<Type, ReferenceCollection> s_ReferenceCollection = new Dictionary<Type, ReferenceCollection>();
        private static bool s_EnableStrictCheck = false;//true启用严格筛查，回收对象时进行检查是否重复回收


        /// <summary>
        /// 严格检查对象启用状态【true在回收对象时检查是否重复回收】
        /// 默认false
        /// </summary>
        public static bool EnableStrickCheck
        {
            get => s_EnableStrictCheck;
            set => s_EnableStrictCheck = value;
        }

        /// <summary>
        /// 获取引用池数量
        /// </summary>
        public static int Count => s_ReferenceCollection.Count;
        /// <summary>
        /// 清理所有引用池
        /// </summary>
        public static void ClearAllPool()
        {
            lock (s_ReferenceCollection)
            {
                var iterator = s_ReferenceCollection.GetEnumerator();
                while (iterator.MoveNext())
                {
                    iterator.Current.Value.RemoveAll();
                }
                s_ReferenceCollection.Clear();
            }
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns></returns>
        public static T Allocate<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Allocate<T>();
        }
        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns></returns>
        public static IReference Allocate(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Allocate();
        }
        /// <summary>
        /// 回收引用
        /// </summary>
        /// <param name="reference"></param>
        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw new Exception("无效的回收对象");
            }

            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);

        }
        /// <summary>
        /// 向引用池追加指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="addCount">追加数量</param>
        public static void AddCount<T>(int addCount) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).AddCount<T>(addCount);
        }
        /// <summary>
        /// 向引用池追加指定数量的引用
        /// </summary>
        /// <param name="referenceType">类型</param>
        /// <param name="addCount">追加数量</param>
        public static void AddCount(Type referenceType, int addCount)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).AddCount(addCount);
        }
        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="removeCount">要移除的数量</param>
        public static void RemoveCount<T>(int removeCount) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveCount(removeCount);
        }
        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="removeCount">要移除的数量</param>
        public static void RemoveCount(Type referenceType,int removeCount)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveCount(removeCount);
        }
        /// <summary>
        /// 从引用池中移除所有引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>()where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }
        /// <summary>
        /// 从引用池中移除所有引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }


        /// <summary>
        /// 检查引用类型
        /// </summary>
        /// <param name="referenceType"></param>
        private static void InternalCheckReferenceType(Type referenceType)
        {
            if (!s_EnableStrictCheck)
            {
                return;
            }

            if (referenceType == null)
            {
                throw new Exception("引用类型是无效的。");
            }

            if (!referenceType.IsClass)
            {
                throw new Exception("引用类型必须是Class。");
            }

            if (referenceType.IsAbstract)
            {
                throw new Exception("引用类型不能是一个抽象类。");
            }

            if (!typeof(IReference).IsAssignableFrom(referenceType))
            {
                throw new Exception("引用类型必须继承RGuang.Kit.IReference接口。");
            }
        }
        /// <summary>
        /// 获取某类型的引用池
        /// </summary>
        /// <param name="referenceType"></param>
        /// <returns></returns>
        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null) throw new Exception($"无效的类型");

            ReferenceCollection referenceCollection = null;
            lock (s_ReferenceCollection)
            {
                if (!s_ReferenceCollection.TryGetValue(referenceType, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);
                    s_ReferenceCollection.Add(referenceType, referenceCollection);
                }
            }

            return referenceCollection;
        }

        #region ReferenceCollection
        private sealed class ReferenceCollection
        {
            private readonly Queue<IReference> m_referenceQue;
            private readonly Type m_referenceType;

            private int m_amountReferenceCount;
            private int m_usingReferenceCount;
            private int m_addedReferenceCount;
            private int m_removedReferenceCount;
            private int m_releasedReferenceCount;


            public ReferenceCollection(Type referenceType)
            {
                m_referenceQue = new Queue<IReference>();
                m_referenceType = referenceType;
                m_usingReferenceCount = 0;
                m_addedReferenceCount = 0;
                m_removedReferenceCount = 0;
                m_releasedReferenceCount = 0;
            }


            public Type ReferenceType => m_referenceType;
            /// <summary>
            /// 总数
            /// </summary>
            public int AmountReferenceCount
            {
                get => m_amountReferenceCount;
                private set => m_amountReferenceCount = value;
            }
            /// <summary>
            /// 正在使用的数量
            /// </summary>
            public int UsingReferenceCount
            {
                get => m_usingReferenceCount;
                private set => m_usingReferenceCount = value;
            }
            /// <summary>
            /// 闲置的数量
            /// </summary>
            public int IdelReferenceCount
            {
                get => m_referenceQue.Count;
            }
            /// <summary>
            /// 已添加过的数量
            /// </summary>
            public int AddedReferenceCount
            {
                get => m_addedReferenceCount;
                private set => m_addedReferenceCount = value;
            }
            /// <summary>
            /// 已移除过的数量
            /// </summary>
            public int RemovedReferenceCount
            {
                get => m_removedReferenceCount;
                private set => m_removedReferenceCount = value;
            }
            /// <summary>
            /// 已回收过的数量
            /// </summary>
            public int ReleasedReferenceCount
            {
                get => m_releasedReferenceCount;
                private set => m_releasedReferenceCount = value;
            }


            public T Allocate<T>() where T : class, IReference, new()
            {
                var type = typeof(T);
                if (type != ReferenceType)
                {
                    throw new Exception($"无效的类型[{type}]");
                }

                lock (m_referenceQue)
                {
                    UsingReferenceCount++;
                    if (m_referenceQue.Count > 0)
                    {
                        return (T)m_referenceQue.Dequeue();
                    }

                    AddedReferenceCount++;
                    AmountReferenceCount++;
                }

                return new T();
            }

            public IReference Allocate()
            {
                lock (m_referenceQue)
                {
                    UsingReferenceCount++;
                    if (m_referenceQue.Count > 0)
                    {
                        return m_referenceQue.Dequeue();
                    }

                    AddedReferenceCount++;
                    AmountReferenceCount++;
                }

                return (IReference)Activator.CreateInstance(ReferenceType);
            }


            public void Release(IReference reference)
            {
                if (reference == null)
                {
                    //throw new Exception($"无效的回收对象");
                    return;
                }

                reference.ClearReference();
                lock (m_referenceQue)
                {
                    if (s_EnableStrictCheck && m_referenceQue.Contains(reference))
                    {
                        throw new Exception($"重复回收对象[{reference}]");
                    }

                    m_referenceQue.Enqueue(reference);
                    UsingReferenceCount--;
                    ReleasedReferenceCount++;
                }

            }

            public void AddCount<T>(int addCount) where T : class, IReference, new()
            {
                if (addCount < 0)
                {
                    throw new Exception($"无效的添加数[{addCount}]");
                }

                var type = typeof(T);
                if (type != ReferenceType)
                {
                    throw new Exception($"无效的类型[{type}]");
                }


                lock (m_referenceQue)
                {
                    while (addCount-- > 0)
                    {
                        m_referenceQue.Enqueue((IReference)Activator.CreateInstance(m_referenceType));
                    }

                    AddedReferenceCount += addCount;
                    AmountReferenceCount += addCount;
                }

            }

            public void AddCount(int addCount)
            {
                if (addCount < 0)
                {
                    throw new Exception($"无效的添加数[{addCount}]");
                }

                lock (m_referenceQue)
                {
                    while (addCount-- > 0)
                    {
                        m_referenceQue.Enqueue((IReference)Activator.CreateInstance(m_referenceType));
                    }

                    AddedReferenceCount += addCount;
                    AmountReferenceCount += addCount;
                }

            }

            public void RemoveCount(int removeCount)
            {
                if (removeCount < 0)
                {
                    throw new Exception($"无效的移除数[{removeCount}]");
                }

                lock (m_referenceQue)
                {
                    if (removeCount > m_referenceQue.Count) removeCount = m_referenceQue.Count;

                    while (removeCount-- > 0)
                    {
                        m_referenceQue.Dequeue();
                    }

                    RemovedReferenceCount += removeCount;
                    AmountReferenceCount -= removeCount;
                }

            }


            public void RemoveAll()
            {
                lock (m_referenceQue)
                {
                    RemovedReferenceCount += m_referenceQue.Count;
                    AmountReferenceCount = 0;
                    m_referenceQue.Clear();
                }

            }


        }
        #endregion





    }
}
