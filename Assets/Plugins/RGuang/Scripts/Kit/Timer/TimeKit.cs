using System;
using UnityEngine;

namespace RGuang.Kit
{
    /// <summary>
    /// DateTime
    /// 
    /// </summary>
    public static class TimeKit
    {


        /// <summary>
        /// 获取当前时间戳【获取失败返回-1】
        /// ps：单位：秒
        /// </summary>
        public static long GetTimeStampSecond(Action<string> errorCallback = null)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
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
        /// 获取当前时间戳【获取失败返回-1】
        /// ps：单位：毫秒
        /// </summary>
        public static long GetTimeStampMilliSecond(Action<string> errorCallback = null)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
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
