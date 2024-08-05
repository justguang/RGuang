using System;
using UnityEngine;

namespace RGuang.Kit
{
    /// <summary>
    /// 时间类
    /// 
    /// </summary>
    public static class TimeKit
    {


        /// <summary>
        /// 获取单位为秒的 时间戳【获取失败返回-1】
        /// </summary>
        public static long GetTimeStampSecond(Action<string> errorCallback = null)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                return Convert.ToInt64(ts.TotalSeconds);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message);
                return -1;
            }
        }
        /// <summary>
        /// 获取单位为毫秒的 时间戳【获取失败返回-1】
        /// </summary>
        public static long GetTimeStampMilliSecond(Action<string> errorCallback = null)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                return Convert.ToInt64(ts.TotalMilliseconds);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message);
                return -1;
            }
        }


    }
}
