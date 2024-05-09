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
        /// <param name="addError"></param>
        public static void AddHandlerIfNotExists<T>(ref T handlers, T addHandler, Action<string> addError = null) where T : Delegate
        {
            if (addHandler == null)
            {
                addError?.Invoke($"添加错误，要添加项不可为空：{typeof(T)}");

                return;
            }

            if (handlers == null)
            {
                handlers = addHandler;
                return;
            }

            var arr = handlers.GetInvocationList();
            if (!arr.Contains(addHandler))
            {
                try
                {
                    handlers = (T)Delegate.Combine(handlers, addHandler);
                }
                catch (Exception e)
                {
                    addError?.Invoke(e.Message);
                }
            }
        }

        /// <summary>
        /// 移除一个如果存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="removeHandler"></param>
        /// <param name="removeError"></param>
        public static void RemoveHandlerIfExists<T>(ref T handlers, T removeHandler, Action<string> removeError = null) where T : Delegate
        {
            if (handlers == null)
            {
                removeError?.Invoke($"移除错误，移除源为空：{typeof(T)}");

                return;
            }


            var arr = handlers.GetInvocationList();

            if (arr.Contains(removeHandler))
            {
                try
                {
                    handlers = (T)Delegate.Remove(handlers, removeHandler);
                }
                catch (Exception e)
                {
                    removeError?.Invoke(e.Message);
                }
            }
        }

        /// <summary>
        /// 移除所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="removeAllError"></param>
        public static void RemoveAllHandlerFromDelegate<T>(ref T handlers, Action<string> removeAllError = null) where T : Delegate
        {
            if (handlers == null)
            {
                removeAllError?.Invoke($"移除错误，移除源为空：{typeof(T)}");

                return;
            }


            var arr = handlers.GetInvocationList();

            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    handlers = (T)Delegate.Remove(handlers, arr[i]);
                }
                catch (Exception e)
                {
                    removeAllError?.Invoke(e.Message);
                }
            }
        }



    }




}
