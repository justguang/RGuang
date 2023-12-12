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
        /// <param name="eventHandler"></param>
        /// <param name="addNewHandler"></param>
        public static void AddHandlerIfNotExists<T>(ref T eventHandler, T addNewHandler) where T : Delegate
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
            if (addNewHandler == null) throw new ArgumentNullException(nameof(addNewHandler));

            var handlers = eventHandler.GetInvocationList();

            if (handlers.Length > 0 && !handlers.Contains(addNewHandler))
            {
                try
                {
                    eventHandler = (T)Delegate.Combine(eventHandler, addNewHandler);
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
        /// <param name="eventHandler"></param>
        /// <param name="removeHandler"></param>
        public static void RemoveHandlerIfExists<T>(ref T eventHandler, T removeHandler) where T : Delegate
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            var handlers = eventHandler.GetInvocationList();

            if (handlers.Length > 0 && handlers.Contains(removeHandler))
            {
                try
                {
                    eventHandler = (T)Delegate.Remove(eventHandler, removeHandler);
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
        /// <param name="eventHandler"></param>
        public static void RemoveAllHandlerFromEvent<T>(ref T eventHandler) where T : Delegate
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));

            var handlers = eventHandler.GetInvocationList();

            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    eventHandler = (T)Delegate.Remove(eventHandler, handlers[i]);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"移除错误：{e.Message}");
                }
            }
        }



    }




}
