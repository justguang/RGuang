using System.Collections.Generic;

namespace RGuang.Kit
{
    /// <summary>
    /// 可池化对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolableObject<T> where T : PoolableObject<T>, new()
    {
        /// <summary>
        /// 池子
        /// </summary>
        private static Stack<T> _pool = new Stack<T>(8);
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static T Allocate()
        {
            var node = _pool.Count == 0 ? new T() : _pool.Pop();
            node._inPool = false;
            return node;
        }

        /// <summary>
        /// 该对象是否进入池子
        /// true：在对象池中
        /// false：不在池子里
        /// </summary>
        protected bool _inPool = false;

        /// <summary>
        /// 回收到对象池中
        /// </summary>
        public void Recycle2Cache()
        {
            OnRecycle();
            _inPool = true;
            _pool.Push(this as T);
        }

        /// <summary>
        /// 对象回收进池子时调用
        /// 用于数据清理，资源释放
        /// </summary>
        protected abstract void OnRecycle();

    }
}
