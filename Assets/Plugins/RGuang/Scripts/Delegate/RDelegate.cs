using System;
using System.Linq;

namespace RGuang
{
    public static class RDelegate
    {
        /// <summary>
        /// 如果不存在添加一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="addHandler"></param>
        public static void AddHandlerIfNotExists<T>(ref T handlers, T addHandler) where T : Delegate
        {
            if (handlers == null) throw new ArgumentNullException(nameof(handlers));
            if (addHandler == null) throw new ArgumentNullException(nameof(addHandler));

            var arr = handlers.GetInvocationList();

            if (arr.Length > 0 && !arr.Contains(addHandler))
            {
                try
                {
                    handlers = (T)Delegate.Combine(handlers, addHandler);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"添加错误：{e.Message}");
                }
            }
        }

        /// <summary>
        /// 移除一个如果存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="removeHandler"></param>
        public static void RemoveHandlerIfExists<T>(ref T handlers, T removeHandler) where T : Delegate
        {
            if (handlers == null) throw new ArgumentNullException(nameof(handlers));

            var arr = handlers.GetInvocationList();

            if (arr.Length > 0 && arr.Contains(removeHandler))
            {
                try
                {
                    handlers = (T)Delegate.Remove(handlers, removeHandler);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"移除错误：{e.Message}");
                }
            }
        }

        /// <summary>
        /// 移除所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        public static void RemoveAllHandlerFromDelegate<T>(ref T handlers) where T : Delegate
        {
            if (handlers == null) throw new ArgumentNullException(nameof(handlers));

            var arr = handlers.GetInvocationList();

            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    handlers = (T)Delegate.Remove(handlers, arr[i]);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"移除错误：{e.Message}");
                }
            }
        }



    }




}
