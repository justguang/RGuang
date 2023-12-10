using System;
using UnityEngine;

namespace RGuang
{
    /// <summary>
    /// 时间类
    /// 
    /// </summary>
    public sealed class Time
    {
        /// <summary>
        /// 获取单位为秒的 时间戳【获取失败返回-1】
        /// </summary>
        public static long GetTimeStampSecond()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                return Convert.ToInt64(ts.TotalSeconds);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }
        /// <summary>
        /// 获取单位为毫秒的 时间戳【获取失败返回-1】
        /// </summary>
        public static long GetTimeStampMilliSecond()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                return Convert.ToInt64(ts.TotalMilliseconds);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }


    }
}
