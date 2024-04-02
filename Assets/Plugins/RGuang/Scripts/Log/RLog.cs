/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  日志工具
///                  内有GC产生，适用于调试，如用在发布产品中建议优化
///********************************************/
/// <summary>
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace RGuang
{
    #region  enum //日志类型，日志输出颜色，日志等级
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum RLoggerType
    {
        /// <summary>
        /// unity类型的日志
        /// </summary>
        Unity,
        /// <summary>
        /// 控制台类型的日志
        /// </summary>
        Console
    }


    /// <summary>
    /// 日志输出颜色
    /// </summary>
    public enum RLogColor
    {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow
    }

    /// <summary>
    /// 日志等级
    /// </summary>
    [Flags]
    public enum RLoggerLevel
    {
        None = 0x1,
        Log = 0x2,
        Warn = 0x4,
        Trace = 0x8,
        Error = 0x10
    }
    #endregion


    #region ILogger 接口， log配置
    interface ILogger
    {
        void Log(string msg, RLogColor logColor = RLogColor.None);

        void Warn(string msg);
        void Error(string msg);
    }

    /// <summary>
    /// Log 配置
    /// </summary>
    public sealed class RLogConfig
    {
        /// <summary>
        /// 日志启用等级
        /// </summary>
        public RLoggerLevel logLevel = RLoggerLevel.Log | RLoggerLevel.Warn | RLoggerLevel.Error;
        /// <summary>
        /// 前缀标记【默认 #】
        /// </summary>
        public string logPrefix = "#";
        /// <summary>
        /// 标记与日志具体内容间隔符号【默认 >>】
        /// </summary>
        public string logSeparate = ">";
        /// <summary>
        /// 是否显示时间标记【默认true，显示】
        /// </summary>
        public bool enableTime = true;
        /// <summary>
        /// 是否显示线程ID【默认true，显示】
        /// </summary>
        public bool enableThreadID = true;
        /// <summary>
        /// 是否显示具体堆栈的消息【默认true，显示】
        /// </summary>
        public bool enableTrace = true;
        /// <summary>
        /// 是否将日志保存下来【默认true，保存】
        /// </summary>
        public bool enableSave = true;
        /// <summary>
        /// 是否覆盖原有保存的日志【默认true，覆盖】
        /// </summary>
        public bool enableCover = true;
        /// <summary>
        /// 日志类型【默认LoggerType.Console】
        /// </summary>
        public RLoggerType loggerType = RLoggerType.Console;
        /// <summary>
        /// 日志保存的路径【默认当前运行程序的更目录下Logs文件夹下】
        /// </summary>
        private string _savePath;
        /// <summary>
        /// 日志文件保存路径
        /// </summary>
        public string savePath
        {
            get
            {
                if (_savePath == null)
                {
                    if (loggerType == RLoggerType.Unity)
                    {
                        Type type = Type.GetType("UnityEngine.Application, UnityEngine");
                        _savePath = type.GetProperty("persistentDataPath").GetValue(null).ToString() + "/RLog/";
                    }
                    else
                    {
                        _savePath = string.Format("{0}RLog\\", AppDomain.CurrentDomain.BaseDirectory);
                    }
                }
                return _savePath;
            }
            set
            {
                _savePath = value;
            }
        }

        /// <summary>
        /// 日志文件保存的名字
        /// </summary>
        public string saveName = "RLog.txt";

    }
    #endregion


    #region Log方法扩展
    /// <summary>
    /// Log扩展
    /// </summary>
    public static class RLogExtensionMethods
    {
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Log(this object obj, string log, params object[] args)
        {
            RLog.Log(string.Format(log, args));
        }
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Log(this object obj, object log)
        {
            RLog.Log(log);
        }

        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容显示的颜色</param>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void ColorLog(this object obj, RLogColor color, string log, params object[] args)
        {
            RLog.ColorLog(color, string.Format(log, args));
        }
        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容显示的颜色</param>
        /// <param name="log">要打印的内容</param>
        public static void ColorLog(this object obj, RLogColor color, object log)
        {
            RLog.ColorLog(color, log);
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Warn(this object obj, string log, params object[] args)
        {
            RLog.Warn(string.Format(log, args));
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Warn(this object obj, object log)
        {
            RLog.Warn(log);
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Error(this object obj, string log, params object[] args)
        {
            RLog.Error(string.Format(log, args));
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Error(this object obj, object log)
        {
            RLog.Error(log);
        }

        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="log">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Trace(this object obj, string log, params object[] args)
        {
            RLog.Trace(string.Format(log, args));
        }
        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="log">要打印的内容</param>
        public static void Trace(this object obj, object log)
        {
            RLog.Trace(log);
        }
    }
    #endregion


    #region RLog 日志核心
    /// <summary>
    /// 日志工具核心类
    /// </summary>
    public sealed class RLog
    {
        /// <summary>
        /// unity类型的输出日志
        /// </summary>
        class UnityLogger : ILogger
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            public void Log(string msg, RLogColor logColor)
            {
                if (logColor != RLogColor.None)
                {
                    msg = UnityLogColor(msg, logColor);
                }
                type.GetMethod("Log", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            public void Warn(string msg)
            {
                msg = UnityLogColor(msg, RLogColor.Yellow);
                type.GetMethod("LogWarning", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            public void Error(string msg)
            {
                msg = UnityLogColor(msg, RLogColor.Red);
                type.GetMethod("LogError", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            private string UnityLogColor(string msg, RLogColor color)
            {
                switch (color)
                {
                    case RLogColor.Red:
                        msg = string.Format("<color=#FF0000>{0}</color>", msg);
                        break;
                    case RLogColor.Green:
                        msg = string.Format("<color=#00FF00>{0}</color>", msg);
                        break;
                    case RLogColor.Blue:
                        msg = string.Format("<color=#0000FF>{0}</color>", msg);
                        break;
                    case RLogColor.Cyan:
                        msg = string.Format("<color=#00FFFF>{0}</color>", msg);
                        break;
                    case RLogColor.Magenta:
                        msg = string.Format("<color=#FF00FF>{0}</color>", msg);
                        break;
                    case RLogColor.Yellow:
                        msg = string.Format("<color=#FFFF00>{0}</color>", msg);
                        break;
                    case RLogColor.None:
                    default:
                        msg = string.Format("<color=#FF0000>{0}</color>", msg);
                        break;
                }
                return msg;
            }
        }

        /// <summary>
        /// 控制台类型的输出日志
        /// </summary>
        class ConsoleLogger : ILogger
        {
            public void Log(string msg, RLogColor logColor)
            {
                WriteConsoleLog(msg, logColor);
            }
            public void Warn(string msg)
            {
                WriteConsoleLog(msg, RLogColor.Yellow);
            }
            public void Error(string msg)
            {
                WriteConsoleLog(msg, RLogColor.Red);
            }
            private void WriteConsoleLog(string msg, RLogColor color)
            {
                switch (color)
                {
                    case RLogColor.Red:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.Cyan:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.Magenta:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case RLogColor.None:
                    default:
                        Console.WriteLine(msg);
                        break;
                }
            }
        }



        /// <summary>
        /// 日志配置
        /// </summary>
        public static RLogConfig cfg;
        /// <summary>
        /// 日志输出
        /// </summary>
        private static ILogger logger;
        /// <summary>
        /// 日志写入文件
        /// </summary>
        private static StreamWriter logFileWriter = null;


        /// <summary>
        /// 日志初始化
        /// </summary>
        /// <param name="logConfig">日志配置【默认null，自动配置】</param>
        public static void InitSetting(RLogConfig logConfig = null)
        {
            if (logConfig == null)
            {
                logConfig = new RLogConfig();
            }
            RLog.cfg = logConfig;


            //unity或控制台类型的日志
            if (cfg.loggerType == RLoggerType.Console)
            {
                logger = new ConsoleLogger();
            }
            else
            {
                logger = new UnityLogger();
            }

        }


        #region public static【 Log、Warn、Error、StackTrace】

        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Log(string msg, params object[] args)
        {
            if ((cfg.logLevel & RLoggerLevel.Log) == 0) return;

            msg = DecorateLog(string.Format(msg, args));
            logger.Log(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Log] {0}", msg));
            }
        }
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Log(object obj)
        {
            if ((cfg.logLevel & RLoggerLevel.Log) == 0) return;

            string msg = DecorateLog(obj.ToString());
            logger.Log(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Log] {0}", msg));
            }
        }

        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容颜色</param>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void ColorLog(RLogColor color, string msg, params object[] args)
        {
            if ((cfg.logLevel & RLoggerLevel.Log) == 0) return;

            msg = DecorateLog(string.Format(msg, args));
            logger.Log(msg, color);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Log] {0}", msg));
            }
        }
        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容颜色</param>
        /// <param name="obj">要打印的内容</param>
        public static void ColorLog(RLogColor color, object obj)
        {
            if ((cfg.logLevel & RLoggerLevel.Log) == 0) return;

            string msg = DecorateLog(obj.ToString());
            logger.Log(msg, color);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Log] {0}", msg));
            }
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Warn(string msg, params object[] args)
        {
            if ((cfg.logLevel & RLoggerLevel.Warn) == 0) return;

            msg = DecorateLog(string.Format(msg, args));
            logger.Warn(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Warning] {0}", msg));
            }
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Warn(object obj)
        {
            if ((cfg.logLevel & RLoggerLevel.Warn) == 0) return;

            string msg = DecorateLog(obj.ToString());
            logger.Warn(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Warning] {0}", msg));
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Error(string msg, params object[] args)
        {
            if ((cfg.logLevel & RLoggerLevel.Error) == 0) return;

            msg = DecorateLog(string.Format(msg, args), true);
            logger.Error(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Error] {0}", msg));
            }
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Error(object obj)
        {
            if ((cfg.logLevel & RLoggerLevel.Error) == 0) return;

            string msg = DecorateLog(obj.ToString(), true);
            logger.Error(msg);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Error] {0}", msg));
            }
        }

        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Trace(string msg, params object[] args)
        {
            if ((cfg.logLevel & RLoggerLevel.Trace) == 0) return;

            msg = DecorateLog(string.Format(msg, args), true);
            logger.Log(msg, RLogColor.Magenta);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Stack Trace] {0}", msg));
            }
        }
        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Trace(object obj)
        {
            if ((cfg.logLevel & RLoggerLevel.Trace) == 0) return;

            string msg = DecorateLog(obj.ToString(), true);
            logger.Log(msg, RLogColor.Magenta);
            if (cfg.enableSave)
            {
                WriteToFile(string.Format("[Stack Trace] {0}", msg));
            }
        }
        #endregion


        #region private static Tool

        /// <summary>
        /// 修饰日志
        /// </summary>
        /// <param name="msg">要修饰的内容</param>
        /// <param name="isTrace">是否显示堆栈【默认false不显示】</param>
        private static string DecorateLog(string msg, bool isTrace = false)
        {
            StringBuilder sb = new StringBuilder(cfg.logPrefix, 100);
            if (cfg.enableTime)
            {
                //时间格式   时：分：秒.毫秒
                sb.AppendFormat(" {0}", DateTime.Now.ToString("HH:mm:ss.ffff"));
            }
            if (cfg.enableThreadID)
            {
                sb.AppendFormat(" {0}", GetThreadID());
            }

            sb.AppendFormat(" {0} {1}", cfg.logSeparate, msg);
            if (isTrace)
            {
                sb.AppendFormat(" \nStackTrace:{0}", GetLogTrace());
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取线程ID
        /// </summary>
        /// <returns>返回当前线程ID</returns>
        private static string GetThreadID()
        {
            return string.Format(" ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// 获取堆栈信息
        /// </summary>
        /// <returns>返回堆栈信息</returns>
        private static string GetLogTrace()
        {
            StackTrace st = new StackTrace(3, true);
            string traceInfo = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame tempSF = st.GetFrame(i);
                traceInfo += string.Format("\n {0}::{1} line:{2}", tempSF.GetFileName(), tempSF.GetMethod(), tempSF.GetFileLineNumber());
            }
            return traceInfo;
        }

        /// <summary>
        /// 写日志内容到文件
        /// </summary>
        /// <param name="msg">要写的内容</param>
        private static void WriteToFile(string msg)
        {
            if (!cfg.enableSave) return;

            //覆盖原有日志
            if (cfg.enableCover)
            {
                string coverPath = cfg.savePath + cfg.saveName;
                try
                {
                    if (Directory.Exists(cfg.savePath))
                    {
                        if (File.Exists(coverPath))
                        {
                            File.Delete(coverPath);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(cfg.savePath);
                    }

                    File.AppendAllText(coverPath, msg + "\n", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    //
                }
            }
            else
            {
                //不覆盖原有日志内容

                string prefix = DateTime.Now.ToString("yyyy-MM-dd@");
                string fileName = prefix + cfg.saveName;
                try
                {
                    if (!Directory.Exists(cfg.savePath))
                    {
                        Directory.CreateDirectory(cfg.savePath);
                    }

                    File.AppendAllText(cfg.savePath + fileName, msg + "\n", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    //
                }
            }

        }
        #endregion

    }
    #endregion


}
