/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  日志工具
///                  内有GC产生，适用于调试，如用在发布产品中建议优化
///********************************************/
/// <summary>

namespace RGuang.LogKit
{

    /// <summary>
    /// Log扩展
    /// </summary>
    public static class LogExtensionMethods
    {
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Info(this object obj, string log, params object[] args)
        {
            RGuang.LogKit.Log.Info(string.Format(log, args));
        }
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Info(this object obj, object log)
        {
            RGuang.LogKit.Log.Info(log);
        }

        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容显示的颜色</param>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void ColorInfo(this object obj, ColorLog color, string log, params object[] args)
        {
            RGuang.LogKit.Log.ColorInfo(color, string.Format(log, args));
        }
        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容显示的颜色</param>
        /// <param name="log">要打印的内容</param>
        public static void ColorInfo(this object obj, ColorLog color, object log)
        {
            RGuang.LogKit.Log.ColorInfo(color, log);
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Warn(this object obj, string log, params object[] args)
        {
            RGuang.LogKit.Log.Warn(string.Format(log, args));
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Warn(this object obj, object log)
        {
            RGuang.LogKit.Log.Warn(log);
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Error(this object obj, string log, params object[] args)
        {
            RGuang.LogKit.Log.Error(string.Format(log, args));
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Error(this object obj, object log)
        {
            RGuang.LogKit.Log.Error(log);
        }

        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Trace(this object obj, string log, params object[] args)
        {
            RGuang.LogKit.Log.Trace(string.Format(log, args));
        }
        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Trace(this object obj, object log)
        {
            RGuang.LogKit.Log.Trace(log);
        }
    }


}

