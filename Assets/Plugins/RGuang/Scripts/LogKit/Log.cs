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
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace RGuang.LogKit
{
    #region  enum //日志类型，日志输出颜色，日志等级
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LoggerType
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
    public enum ColorLog
    {
        /// <summary>
        /// 白
        /// </summary>
        White,
        /// <summary>
        /// 灰
        /// </summary>
        Gray,
        /// <summary>
        /// 黑
        /// </summary>
        Black,
        /// <summary>
        /// 红
        /// </summary>
        Red,
        /// <summary>
        /// 绿
        /// </summary>
        Green,
        /// <summary>
        /// 蓝
        /// </summary>
        Blue,
        /// <summary>
        /// 黄
        /// </summary>
        Yellow,
        /// <summary>
        /// 青
        /// </summary>
        Cyan,
        /// <summary>
        /// 洋红
        /// </summary>
        Magenta,


        /// <summary>
        /// 深灰
        /// </summary>
        DarkGray,
        /// <summary>
        /// 深红
        /// </summary>
        DarkRed,
        /// <summary>
        /// 墨绿
        /// </summary>
        DarkGreen,
        /// <summary>
        /// 深蓝
        /// </summary>
        DarkBlue,
        /// <summary>
        /// 暗黄
        /// </summary>
        DarkYellow,
        /// <summary>
        /// 暗青
        /// </summary>
        DarkCyan,
        /// <summary>
        /// 暗洋红(紫)
        /// </summary>
        DarkMagenta,

    }

    /// <summary>
    /// 日志等级
    /// </summary>
    [Flags]
    public enum LoggerLevel
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0x1,
        /// <summary>
        /// 普通信息
        /// </summary>
        Info = 0x2,
        /// <summary>
        /// 警告信息
        /// </summary>
        Warn = 0x4,
        /// <summary>
        /// 堆栈信息
        /// </summary>
        Trace = 0x8,
        /// <summary>
        /// 错误信息
        /// </summary>
        Error = 0x10
    }
    #endregion


    #region ILogger 接口， log配置
    interface ILogger
    {
        /// <summary>
        /// 普通日志信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logColor"></param>
        void Info(string msg, ColorLog logColor = ColorLog.Black);
        /// <summary>
        /// 警告日志信息
        /// </summary>
        /// <param name="msg"></param>
        void Warn(string msg);
        /// <summary>
        /// 错误日志信息
        /// </summary>
        /// <param name="msg"></param>
        void Error(string msg);

    }


    /// <summary>
    /// Info 配置
    /// </summary>
    public sealed class LogConfig
    {
        /// <summary>
        /// 日志启用等级
        /// </summary>
        public LoggerLevel LogLevel = LoggerLevel.Info | LoggerLevel.Warn | LoggerLevel.Error;
        /// <summary>
        /// 日志类型【默认LoggerType.Console】
        /// </summary>
        public LoggerType LoggerType = LoggerType.Console;
        /// <summary>
        /// 前缀标记【默认 #】
        /// </summary>
        public string LogPrefix = "#";
        /// <summary>
        /// 是否显示时间标记【默认true，显示】
        /// </summary>
        public bool EnableTime = true;
        /// <summary>
        /// 是否显示线程ID【默认true，显示】
        /// </summary>
        public bool EnableThreadID = true;
        /// <summary>
        /// 标记与日志具体内容间隔符号【默认 >>】
        /// </summary>
        public string LogSeparate = ">";
        /// <summary>
        /// 是否显示具体堆栈的消息【默认true，显示】
        /// </summary>
        public bool EnableTrace = false;
        /// <summary>
        /// 是否将日志保存下来【默认true，保存】
        /// </summary>
        public bool EnableSave = true;
        /// <summary>
        /// 日志保存的路径【默认当前运行程序的更目录下Logs文件夹下】
        /// </summary>
        private string _savePath = string.Empty;
        /// <summary>
        /// 日志文件保存路径
        /// </summary>
        public string SavePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_savePath))
                {
                    if (LoggerType == LoggerType.Unity)
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

            set => _savePath = value;
        }

        /// <summary>
        /// 日志文件保存的名字
        /// 新日志数据覆盖文件内原数据 =》日志文件名【年-月-日@文件名】
        /// 新日志数据不覆盖文件内原数据 =》日志文件名【年-月-日[时]@文件名】
        /// </summary>
        public string SaveName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_saveName))
                {
                    _saveName = "Rlog.txt";
                }
                return _saveName;
            }

            set => _saveName = value;
        }
        private string _saveName = string.Empty;



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"日志类型 [{LoggerType}]");
            sb.Append($"\n日志启用等级：");
            if ((LogLevel & LoggerLevel.Info) != 0)
                sb.Append("Info、");
            if ((LogLevel & LoggerLevel.Warn) != 0)
                sb.Append("Warn、");
            if ((LogLevel & LoggerLevel.Error) != 0)
                sb.Append("Error、");
            if ((LogLevel & LoggerLevel.Trace) != 0)
                sb.Append("Trace、");
            if ((LogLevel & LoggerLevel.None) != 0)
                sb.Append("Black");

            sb.Append($"\n日志显示时间 [{EnableTime}]");
            sb.Append($"\n日志显示显示线程ID [{EnableThreadID}]");
            sb.Append($"\n日志显示具体堆栈信息 [{EnableTrace}]");
            sb.Append($"\n日志内容前缀标记 [{LogPrefix}]");
            sb.Append($"\n日志前缀与具体内容分割符号 [{LogSeparate}]");
            sb.Append($"\n日志是否保存到日志文件 [{EnableSave}]");
            sb.Append($"\n日志文件保存路径 [{SavePath}]");
            sb.Append($"\n日志保存的文件名 [{DateTime.Now.ToString("yyyy-MM-dd[HH]@") + SaveName}]");

            return sb.ToString();
        }
    }
    #endregion



    #region RLog 日志核心
    /// <summary>
    /// 日志工具核心类
    /// </summary>
    public sealed class Log
    {
        /// <summary>
        /// unity类型的输出日志
        /// </summary>
        class UnityLogger : ILogger
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            public UnityLogger() => UnityEngine.Application.logMessageReceivedThreaded += OnUnityLogReceivedThreaded;

            /**
             * Unity全局异常事件监听：
             *      UnityEngine.Application.logMessageReceivedThreaded += OnUnityLogReceivedThreaded;
             */
            private static void OnUnityLogReceivedThreaded(string msg, string stackTrace, UnityEngine.LogType logType)
            {
                if (LogKit.Log.Cfg.LoggerType.Equals(LoggerType.Unity) && LogKit.Log.Cfg.EnableSave)
                {
                    switch (logType)
                    {
                        case LogType.Log: msg = string.Format("[信息]{0}", msg); break;
                        case LogType.Warning: msg = string.Format("[警告]{0}", msg); break;
                        case LogType.Error: msg = string.Format("[错误]{0}", msg); break;

                        //【断言】+前缀符号+时间+线程ID+分割符号+信息内容
                        case LogType.Assert:
                            msg = string.Format("【断言】{0}{1}{2}{3}{4}\n\t堆栈:\n\t\t{5}",
                            LogKit.Log.Cfg.LogPrefix,
                            LogKit.Log.Cfg.EnableTime ? DateTime.Now.ToString("HH:mm:ss.ffff") + " " : null,
                            LogKit.Log.Cfg.EnableThreadID ? LogKit.Log.GetThreadID() + " " : null,
                            LogKit.Log.Cfg.LogSeparate + " ",
                            msg,
                            stackTrace);

                            break;
                        case LogType.Exception:
                            msg = string.Format("【异常】{0}{1}{2}{3}{4}\n\t堆栈:\n\t\t{5}",
                            LogKit.Log.Cfg.LogPrefix,
                            LogKit.Log.Cfg.EnableTime ? DateTime.Now.ToString("HH:mm:ss.ffff") + " " : null,
                            LogKit.Log.Cfg.EnableThreadID ? LogKit.Log.GetThreadID() + " " : null,
                            LogKit.Log.Cfg.LogSeparate + " ",
                            msg,
                            stackTrace);

                            break;
                        default:
                            break;
                    }

                    LogKit.Log.WriteToFile(msg);
                }
            }


            public void Info(string msg, ColorLog logColor)
            {
                if (logColor != LogKit.ColorLog.Black)
                {
                    msg = UnityInfoColor(msg, logColor);
                }
                type.GetMethod("Log", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            public void Warn(string msg)
            {
                msg = UnityInfoColor(msg, LogKit.ColorLog.Yellow);
                type.GetMethod("LogWarning", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            public void Error(string msg)
            {
                msg = UnityInfoColor(msg, LogKit.ColorLog.Red);
                type.GetMethod("LogError", new Type[] { typeof(object) }).Invoke(null, new object[] { msg });
            }

            private string UnityInfoColor(string msg, ColorLog color)
            {
                switch (color)
                {
                    //十六进制代码 #RR GG BB
                    //Color : #00 00 00
                    case LogKit.ColorLog.White: msg = string.Format("<color=#FFFFFF>{0}</color>", msg); break;
                    case LogKit.ColorLog.Gray: msg = string.Format("<color=#C0C0C0>{0}</color>", msg); break;
                    case LogKit.ColorLog.Black: msg = string.Format("<color=#000000>{0}</color>", msg); break;
                    case LogKit.ColorLog.Red: msg = string.Format("<color=#FF0000>{0}</color>", msg); break;
                    case LogKit.ColorLog.Green: msg = string.Format("<color=#00FF00>{0}</color>", msg); break;
                    case LogKit.ColorLog.Blue: msg = string.Format("<color=#4070FF>{0}</color>", msg); break;
                    case LogKit.ColorLog.Yellow: msg = string.Format("<color=#FFFF00>{0}</color>", msg); break;
                    case LogKit.ColorLog.Cyan: msg = string.Format("<color=#00FFFF>{0}</color>", msg); break;
                    case LogKit.ColorLog.Magenta: msg = string.Format("<color=#FF00FF>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkGray: msg = string.Format("<color=#909090>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkRed: msg = string.Format("<color=#900000>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkGreen: msg = string.Format("<color=#009000>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkBlue: msg = string.Format("<color=#000090>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkYellow: msg = string.Format("<color=#909000>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkCyan: msg = string.Format("<color=#009090>{0}</color>", msg); break;
                    case LogKit.ColorLog.DarkMagenta: msg = string.Format("<color=#900090>{0}</color>", msg); break;
                    default: msg = string.Format("<color=#000000>{0}</color>", msg); break;
                }

                return msg;
            }
        }

        /// <summary>
        /// 控制台类型的输出日志
        /// </summary>
        class ConsoleLogger : ILogger
        {
            public ConsoleLogger() => System.AppDomain.CurrentDomain.FirstChanceException += FirstChanceException;

            /**
             * Console全局异常事件监听：
             *      System.AppDomain.CurrentDomain.FirstChanceException += FirstChanceException;
             */
            private static void FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
            {
                if (LogKit.Log.Cfg.LoggerType.Equals(LoggerType.Console) && LogKit.Log.Cfg.EnableSave)
                {
                    StringBuilder sb = new StringBuilder(DecorateLog(e.Exception.Message, false));
                    sb.Insert(0, "【异常】 ");
                    sb.AppendFormat("\n\t堆栈:\n\t\t{0}", e.Exception.StackTrace);
                    Console.WriteLine(sb.ToString());
                    LogKit.Log.WriteToFile(sb.ToString());
                }
            }

            public void Info(string msg, ColorLog logColor)
            {
                WriteConsoleInfo(msg, logColor);
            }
            public void Warn(string msg)
            {
                WriteConsoleInfo(msg, LogKit.ColorLog.Yellow);
            }
            public void Error(string msg)
            {
                WriteConsoleInfo(msg, LogKit.ColorLog.Red);
            }
            private void WriteConsoleInfo(string msg, ColorLog color)
            {
                switch (color)
                {
                    case LogKit.ColorLog.White:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Gray:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(msg);
                        break;
                    case LogKit.ColorLog.Black:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Cyan:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.Magenta:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkGray:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkRed:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkGreen:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkBlue:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkYellow:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkCyan:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogKit.ColorLog.DarkMagenta:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(msg);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(msg);
                        break;
                }

            }

        }



        /// <summary>
        /// 日志配置
        /// </summary>
        public static LogConfig Cfg
        {
            get
            {
                if (_cfg == null) throw new Exception("日志 [RGuang.Kit.Info] 未初始化. 请使用[RGuang.Kit.Info.InitSetting]进行初始化.");
                return _cfg;
            }
            private set => _cfg = value;
        }
        private static LogConfig _cfg = null;
        /// <summary>
        /// 日志输出
        /// </summary>
        private static ILogger Logger;
        /// <summary>
        /// 日志写入文件 StreamWriter
        /// </summarmsgy>
        //private static StreamWriter logFileWriter = null;
        /// <summary>
        /// 日志写入文件 FileStream
        /// </summary>
        //private static FileStream logFileStream = null;



        /// <summary>
        /// 日志初始化
        /// </summary>
        /// <param name="logConfig">日志配置【默认null，自动配置】</param>
        public static void InitSetting(LogConfig logConfig = null)
        {
            if (logConfig == null) logConfig = new LogConfig();
            RGuang.LogKit.Log.Cfg = logConfig;


            if (Cfg.LoggerType == LoggerType.Console)
            {
                //Console
                Logger = new ConsoleLogger();
            }
            else if (Cfg.LoggerType == LoggerType.Unity)
            {
                //Unity
                Logger = new UnityLogger();
            }

            if (Directory.Exists(Cfg.SavePath) == false) Directory.CreateDirectory(Cfg.SavePath);

        }





        #region public static【 Log、Warn、Error、StackTrace】

        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Info(string msg, params object[] args)
        {
            if ((Cfg.LogLevel & LoggerLevel.Info) == 0) return;

            msg = DecorateLog(string.Format(msg, args), Cfg.EnableTrace);
            Logger.Info(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[信息] {0}", msg));
            }
        }
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Info(object obj)
        {
            if ((Cfg.LogLevel & LoggerLevel.Info) == 0) return;

            string msg = DecorateLog(obj.ToString(), Cfg.EnableTrace);
            Logger.Info(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[信息] {0}", msg));
            }
        }

        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容颜色</param>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void ColorInfo(ColorLog color, string msg, params object[] args)
        {
            if ((Cfg.LogLevel & LoggerLevel.Info) == 0) return;

            msg = DecorateLog(string.Format(msg, args), Cfg.EnableTrace);
            Logger.Info(msg, color);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[信息] {0}", msg));
            }
        }
        /// <summary>
        /// 打印带颜色的日志
        /// </summary>
        /// <param name="color">设置内容颜色</param>
        /// <param name="obj">要打印的内容</param>
        public static void ColorInfo(ColorLog color, object obj)
        {
            if ((Cfg.LogLevel & LoggerLevel.Info) == 0) return;

            string msg = DecorateLog(obj.ToString(), Cfg.EnableTrace);
            Logger.Info(msg, color);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[信息] {0}", msg));
            }
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Warn(string msg, params object[] args)
        {
            if ((Cfg.LogLevel & LoggerLevel.Warn) == 0) return;

            msg = DecorateLog(string.Format(msg, args), Cfg.EnableTrace);
            Logger.Warn(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[警告] {0}", msg));
            }
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Warn(object obj)
        {
            if ((Cfg.LogLevel & LoggerLevel.Warn) == 0) return;

            string msg = DecorateLog(obj.ToString(), Cfg.EnableTrace);
            Logger.Warn(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[警告] {0}", msg));
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Error(string msg, params object[] args)
        {
            if ((Cfg.LogLevel & LoggerLevel.Error) == 0) return;

            msg = DecorateLog(string.Format(msg, args), true);
            Logger.Error(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[错误] {0}", msg));
            }
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Error(object obj)
        {
            if ((Cfg.LogLevel & LoggerLevel.Error) == 0) return;

            string msg = DecorateLog(obj.ToString(), true);
            Logger.Error(msg);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[错误] {0}", msg));
            }
        }

        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="msg">要打印的内容</param>
        /// <param name="args">格式化的参数</param>
        public static void Trace(string msg, params object[] args)
        {
            if ((Cfg.LogLevel & LoggerLevel.Trace) == 0) return;

            msg = DecorateLog(string.Format(msg, args), true);
            LogExtensionMethods.Info(Logger, msg, LogKit.ColorLog.Magenta);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[堆栈] {0}", msg));
            }
        }
        /// <summary>
        /// 打印堆栈
        /// </summary>
        /// <param name="obj">要打印的内容</param>
        public static void Trace(object obj)
        {
            if ((Cfg.LogLevel & LoggerLevel.Trace) == 0) return;

            string msg = DecorateLog(obj.ToString(), true);
            LogExtensionMethods.Info(Logger, msg, LogKit.ColorLog.Magenta);
            if (Cfg.LoggerType.Equals(LoggerType.Console) && Cfg.EnableSave)
            {
                WriteToFile(string.Format("[堆栈] {0}", msg));
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
            StringBuilder sb = new StringBuilder(Cfg.LogPrefix, 100);
            if (Cfg.EnableTime)
            {
                //时间格式   时：分：秒.毫秒
                sb.AppendFormat("{0} ", DateTime.Now.ToString("HH:mm:ss.ffff"));
            }
            if (Cfg.EnableThreadID)
            {
                sb.AppendFormat("{0} ", GetThreadID());
            }
            sb.AppendFormat("{0} {1} ", Cfg.LogSeparate, msg);

            if (isTrace)
            {
                sb.AppendFormat(" \n\t堆栈:\n\t\t{0}", GetLogTrace());
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取线程ID
        /// </summary>
        /// <returns>返回当前线程ID</returns>
        private static string GetThreadID()
        {
            return string.Format("线程ID:{0}", Thread.CurrentThread.ManagedThreadId);
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
                traceInfo += string.Format("脚本:{0} | 函数:{1} | 行数:{2}", tempSF.GetFileName(), tempSF.GetMethod(), tempSF.GetFileLineNumber());
            }
            return traceInfo;
        }

        /// <summary>
        /// 写日志内容到文件
        /// </summary>
        /// <param name="msg">要写的内容</param>
        private static void WriteToFile(string msg)
        {
            string prefix = DateTime.Now.ToString("yyyy-MM-dd[HH]@");
            string fileName = prefix + Cfg.SaveName;
            string path = Cfg.SavePath + fileName;

            WriteFileByFileWriter(path, msg + "\n", Encoding.UTF8, true);
        }

        /// <summary>
        /// 写入数据自动创建文件【用StreamWriter类写入】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="context">数据</param>
        /// <param name="encoding"><编码/param>
        /// <param name="append">true追加写入数据，false覆盖原有数据</param>
        /// <param name="errorCallback">异常回调</param>
        static void WriteFileByFileWriter(string path, string context, Encoding encoding, bool append)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new Exception("写入日志文件失败！没有指定文件路径");
            }

            if (File.Exists(path) == false)
            {
                var fs = File.Create(path);
                fs.Close();
                fs.Dispose();
            }

            try
            {
                //context += "\n";
                using (StreamWriter sr = new StreamWriter(path, append, encoding))
                {
                    sr.Write(context);
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion




    }
    #endregion


}