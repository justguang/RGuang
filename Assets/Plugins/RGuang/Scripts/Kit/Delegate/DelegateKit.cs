using System;
using System.Linq;

namespace RGuang.Kit
{
    public static class DelegateKit
    {
        /// <summary>
        /// 添加一项【可重复添加相同项】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="addHandler"></param>
        public static void AddHandler<T>(ref T handlers, T addHandler, Action<string> errorCallback = null) where T : Delegate
        {
            try
            {
                handlers = (T)Delegate.Combine(handlers, addHandler);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// 添加一项【唯一，避免重复添加】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="addHandler"></param>
        /// <param name="addError"></param>
        public static void AddHandlerIfNotExists<T>(ref T handlers, T addHandler, Action<string> addError = null) where T : Delegate
        {
            if (addHandler == null)
            {
                addError?.Invoke($"添加错误，要添加项为空：{typeof(T)}");

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
            else
            {
                addError?.Invoke($"重复添加[{addHandler}]");
            }
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="removeHandler"></param>
        /// <param name="removeError"></param>
        public static void RemoveHandler<T>(ref T handlers, T removeHandler, Action<string> removeError = null) where T : Delegate
        {
            if (handlers == null)
            {
                removeError?.Invoke($"移除错误，移除源为空：{typeof(T)}");

                return;
            }

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




}
