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

        void Dispose();

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
        /// 日志文件保存路径【默认当前运行程序的更目录下Logs文件夹下】
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
                        _savePath = type.GetProperty("streamingAssetsPath").GetValue(null).ToString();

#if UNITY_EDITOR
                        int subIndex = _savePath.LastIndexOf("/", _savePath.Length - 17);
                        _savePath = _savePath.Substring(0, subIndex) + "/Logs";
#endif

                        _savePath = _savePath + "/RGuangLog/";
                    }
                    else
                    {
                        _savePath = string.Format("{0}RGuangLog\\", AppDomain.CurrentDomain.BaseDirectory);
                    }
                }

                return _savePath;
            }

        }
        private string _savePath = string.Empty;

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
                    _saveName = "RGuangLog.log";
                }
                return _saveName;
            }
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
            sb.Append($"\n日志保存的文件名 [{DateTime.Now.ToString("yyyy-MM-dd.HH@") + SaveName}]");

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
        #region Unity Logger
        /// <summary>
        /// unity类型的输出日志
        /// </summary>
        class UnityLogger : ILogger
        {
            ~UnityLogger()
            {
                Dispose();
            }
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

            public void Dispose()
            {
                type = null;
                UnityEngine.Application.logMessageReceivedThreaded -= OnUnityLogReceivedThreaded;
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
        #endregion

        #region ConsoleLogger
        /// <summary>
        /// 控制台类型的输出日志
        /// </summary>
        class ConsoleLogger : ILogger
        {
            ~ConsoleLogger()
            {
                Dispose();
            }
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
            public void Dispose()
            {
                System.AppDomain.CurrentDomain.FirstChanceException -= FirstChanceException;
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
        #endregion


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

            if (Logger != null) Logger.Dispose();

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

        #region --- TEST PRINT ---
        /// <summary>
        /// 测试打印 【初音未来】
        /// <param name="colorLog">颜色</param>
        public static void TestPrint_Miku(ColorLog colorLog = ColorLog.Cyan)
        {
            /**
             *_______________#########_______________________
             *______________############_____________________
             *______________#############____________________
             *_____________##__###########___________________
             *____________###__######_#####__________________
             *____________###_#######___####_________________
             *___________###__##########_####________________
             *__________####__###########_####_______________
             *________#####___###########__#####_____________
             *_______######___###_########___#####___________
             *_______#####___###___########___######_________
             *______######___###__###########___######_______
             *_____######___####_##############__######______
             *____#######__#####################_#######_____
             *____#######__##############################____
             *___#######__######_#################_#######___
             *___#######__######_######_#########___######___
             *___#######____##__######___######_____######___
             *___#######________######____#####_____#####____
             *____######________#####_____#####_____####_____
             *_____#####________####______#####_____###______
             *______#####______;###________###______#________
             *________##_______####________####______________
             */


            {
                Log.Info(" ");
                Log.ColorInfo(colorLog, "_______________#########_______________________");
                Log.ColorInfo(colorLog, "______________############_____________________");
                Log.ColorInfo(colorLog, "______________#############____________________");
                Log.ColorInfo(colorLog, "_____________##__###########___________________");
                Log.ColorInfo(colorLog, "____________###__######_#####__________________");
                Log.ColorInfo(colorLog, "____________###_#######___####_________________");
                Log.ColorInfo(colorLog, "___________###__##########_####________________");
                Log.ColorInfo(colorLog, "__________####__###########_####_______________");
                Log.ColorInfo(colorLog, "________#####___###########__#####_____________");
                Log.ColorInfo(colorLog, "_______######___###_########___#####___________");
                Log.ColorInfo(colorLog, "_______#####___###___########___######_________");
                Log.ColorInfo(colorLog, "______######___###__###########___######_______");
                Log.ColorInfo(colorLog, "_____######___####_##############__######______");
                Log.ColorInfo(colorLog, "____#######__#####################_#######_____");
                Log.ColorInfo(colorLog, "____#######__##############################____");
                Log.ColorInfo(colorLog, "___#######__######_#################_#######___");
                Log.ColorInfo(colorLog, "___#######__######_######_#########___######___");
                Log.ColorInfo(colorLog, "___#######____##__######___######_____######___");
                Log.ColorInfo(colorLog, "___#######________######____#####_____#####____");
                Log.ColorInfo(colorLog, "____######________#####_____#####_____####_____");
                Log.ColorInfo(colorLog, "_____#####________####______#####_____###______");
                Log.ColorInfo(colorLog, "______#####______;###________###______#________");
                Log.ColorInfo(colorLog, "________##_______####________####______________");
                Log.Info(" ");
            }

        }

        /// <summary>
        /// 测试打印【Doge】
        /// </summary>
        public static void TestPrint_Doge(ColorLog colorLog = ColorLog.Yellow)
        {
            /**                                                                          
             *          .,:,,,                                        .::,,,::.          
             *        .::::,,;;,                                  .,;;:,,....:i:         
             *        :i,.::::,;i:.      ....,,:::::::::,....   .;i:,.  ......;i.        
             *        :;..:::;::::i;,,:::;:,,,,,,,,,,..,.,,:::iri:. .,:irsr:,.;i.        
             *        ;;..,::::;;;;ri,,,.                    ..,,:;s1s1ssrr;,.;r,        
             *        :;. ,::;ii;:,     . ...................     .;iirri;;;,,;i,        
             *        ,i. .;ri:.   ... ............................  .,,:;:,,,;i:        
             *        :s,.;r:... ....................................... .::;::s;        
             *        ,1r::. .............,,,.,,:,,........................,;iir;        
             *        ,s;...........     ..::.,;:,,.          ...............,;1s        
             *       :i,..,.              .,:,,::,.          .......... .......;1,       
             *      ir,....:rrssr;:,       ,,.,::.     .r5S9989398G95hr;. ....,.:s,      
             *     ;r,..,s9855513XHAG3i   .,,,,,,,.  ,S931,.,,.;s;s&BHHA8s.,..,..:r:     
             *    :r;..rGGh,  :SAG;;G@BS:.,,,,,,,,,.r83:      hHH1sXMBHHHM3..,,,,.ir.    
             *   ,si,.1GS,   sBMAAX&MBMB5,,,,,,:,,.:&8       3@HXHBMBHBBH#X,.,,,,,,rr    
             *   ;1:,,SH:   .A@&&B#&8H#BS,,,,,,,,,.,5XS,     3@MHABM&59M#As..,,,,:,is,   
             *  .rr,,,;9&1   hBHHBB&8AMGr,,,,,,,,,,,:h&&9s;   r9&BMHBHMB9:  . .,,,,;ri.  
             *  :1:....:5&XSi;r8BMBHHA9r:,......,,,,:ii19GG88899XHHH&GSr.      ...,:rs.  
             *  ;s.     .:sS8G8GG889hi.        ....,,:;:,.:irssrriii:,.        ...,,i1,  
             *  ;1,         ..,....,,isssi;,        .,,.                      ....,.i1,  
             *  ;h:               i9HHBMBBHAX9:         .                     ...,,,rs,  
             *  ,1i..            :A#MBBBBMHB##s                             ....,,,;si.  
             *  .r1,..        ,..;3BMBBBHBB#Bh.     ..                    ....,,,,,i1;   
             *   :h;..       .,..;,1XBMMMMBXs,.,, .. :: ,.               ....,,,,,,ss.   
             *    ih: ..    .;;;, ;;:s58A3i,..    ,. ,.:,,.             ...,,,,,:,s1,    
             *    .s1,....   .,;sh,  ,iSAXs;.    ,.  ,,.i85            ...,,,,,,:i1;     
             *     .rh: ...     rXG9XBBM#M#MHAX3hss13&&HHXr         .....,,,,,,,ih;      
             *      .s5: .....    i598X&&A&AAAAAA&XG851r:       ........,,,,:,,sh;       
             *      . ihr, ...  .         ..                    ........,,,,,;11:.       
             *         ,s1i. ...  ..,,,..,,,.,,.,,.,..       ........,,.,,.;s5i.         
             *          .:s1r,......................       ..............;shs,           
             *          . .:shr:.  ....                 ..............,ishs.             
             *              .,issr;,... ...........................,is1s;.               
             *                 .,is1si;:,....................,:;ir1sr;,                  
             *                    ..:isssssrrii;::::::;;iirsssssr;:..                    
             *                         .,::iiirsssssssssrri;;:.                      
             */

            {

                Log.Info(" ");
                Log.ColorInfo(colorLog, "          .,:,,,                                        .::,,,::.          ");
                Log.ColorInfo(colorLog, "        .::::,,;;,                                  .,;;:,,....:i:         ");
                Log.ColorInfo(colorLog, "        :i,.::::,;i:.      ....,,:::::::::,....   .;i:,.  ......;i.        ");
                Log.ColorInfo(colorLog, "        :;..:::;::::i;,,:::;:,,,,,,,,,,..,.,,:::iri:. .,:irsr:,.;i.        ");
                Log.ColorInfo(colorLog, "        ;;..,::::;;;;ri,,,.                    ..,,:;s1s1ssrr;,.;r,        ");
                Log.ColorInfo(colorLog, "        :;. ,::;ii;:,     . ...................     .;iirri;;;,,;i,        ");
                Log.ColorInfo(colorLog, "        ,i. .;ri:.   ... ............................  .,,:;:,,,;i:        ");
                Log.ColorInfo(colorLog, "        :s,.;r:... ....................................... .::;::s;        ");
                Log.ColorInfo(colorLog, "        ,1r::. .............,,,.,,:,,........................,;iir;        ");
                Log.ColorInfo(colorLog, "        ,s;...........     ..::.,;:,,.          ...............,;1s        ");
                Log.ColorInfo(colorLog, "       :i,..,.              .,:,,::,.          .......... .......;1,       ");
                Log.ColorInfo(colorLog, "      ir,....:rrssr;:,       ,,.,::.     .r5S9989398G95hr;. ....,.:s,      ");
                Log.ColorInfo(colorLog, "     ;r,..,s9855513XHAG3i   .,,,,,,,.  ,S931,.,,.;s;s&BHHA8s.,..,..:r:     ");
                Log.ColorInfo(colorLog, "    :r;..rGGh,  :SAG;;G@BS:.,,,,,,,,,.r83:      hHH1sXMBHHHM3..,,,,.ir.    ");
                Log.ColorInfo(colorLog, "   ,si,.1GS,   sBMAAX&MBMB5,,,,,,:,,.:&8       3@HXHBMBHBBH#X,.,,,,,,rr    ");
                Log.ColorInfo(colorLog, "   ;1:,,SH:   .A@&&B#&8H#BS,,,,,,,,,.,5XS,     3@MHABM&59M#As..,,,,:,is,   ");
                Log.ColorInfo(colorLog, "  .rr,,,;9&1   hBHHBB&8AMGr,,,,,,,,,,,:h&&9s;   r9&BMHBHMB9:  . .,,,,;ri.  ");
                Log.ColorInfo(colorLog, "  :1:....:5&XSi;r8BMBHHA9r:,......,,,,:ii19GG88899XHHH&GSr.      ...,:rs.  ");
                Log.ColorInfo(colorLog, "  ;s.     .:sS8G8GG889hi.        ....,,:;:,.:irssrriii:,.        ...,,i1,  ");
                Log.ColorInfo(colorLog, "  ;1,         ..,....,,isssi;,        .,,.                      ....,.i1,  ");
                Log.ColorInfo(colorLog, "  ;h:               i9HHBMBBHAX9:         .                     ...,,,rs,  ");
                Log.ColorInfo(colorLog, "  ,1i..            :A#MBBBBMHB##s                             ....,,,;si.  ");
                Log.ColorInfo(colorLog, "  .r1,..        ,..;3BMBBBHBB#Bh.     ..                    ....,,,,,i1;   ");
                Log.ColorInfo(colorLog, "   :h;..       .,..;,1XBMMMMBXs,.,, .. :: ,.               ....,,,,,,ss.   ");
                Log.ColorInfo(colorLog, "    ih: ..    .;;;, ;;:s58A3i,..    ,. ,.:,,.             ...,,,,,:,s1,    ");
                Log.ColorInfo(colorLog, "    .s1,....   .,;sh,  ,iSAXs;.    ,.  ,,.i85            ...,,,,,,:i1;     ");
                Log.ColorInfo(colorLog, "     .rh: ...     rXG9XBBM#M#MHAX3hss13&&HHXr         .....,,,,,,,ih;      ");
                Log.ColorInfo(colorLog, "      .s5: .....    i598X&&A&AAAAAA&XG851r:       ........,,,,:,,sh;       ");
                Log.ColorInfo(colorLog, "      . ihr, ...  .         ..                    ........,,,,,;11:.       ");
                Log.ColorInfo(colorLog, "         ,s1i. ...  ..,,,..,,,.,,.,,.,..       ........,,.,,.;s5i.         ");
                Log.ColorInfo(colorLog, "          .:s1r,......................       ..............;shs,           ");
                Log.ColorInfo(colorLog, "          . .:shr:.  ....                 ..............,ishs.             ");
                Log.ColorInfo(colorLog, "              .,issr;,... ...........................,is1s;.               ");
                Log.ColorInfo(colorLog, "                 .,is1si;:,....................,:;ir1sr;,                  ");
                Log.ColorInfo(colorLog, "                    ..:isssssrrii;::::::;;iirsssssr;:..                    ");
                Log.ColorInfo(colorLog, "                         .,::iiirsssssssssrri;;:.                      ");
                Log.Info(" ");
            }
        }

        /// <summary>
        /// 测试打印 【Doge2】
        /// </summary>
        /// <param name="colorLog"></param>
        public static void TestPrint_Doge2(ColorLog colorLog = ColorLog.DarkYellow)
        {

            /***
            * ░░░░░░░░░░░░░░░░░░░░░░░░▄░░
            * ░░░░░░░░░▐█░░░░░░░░░░░▄▀▒▌░
            * ░░░░░░░░▐▀▒█░░░░░░░░▄▀▒▒▒▐
            * ░░░░░░░▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐
            * ░░░░░▄▄▀▒░▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐
            * ░░░▄▀▒▒▒░░░▒▒▒░░░▒▒▒▀██▀▒▌
            * ░░▐▒▒▒▄▄▒▒▒▒░░░▒▒▒▒▒▒▒▀▄▒▒
            * ░░▌░░▌█▀▒▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐
            * ░▐░░░▒▒▒▒▒▒▒▒▌██▀▒▒░░░▒▒▒▀▄
            * ░▌░▒▄██▄▒▒▒▒▒▒▒▒▒░░░░░░▒▒▒▒
            * ▀▒▀▐▄█▄█▌▄░▀▒▒░░░░░░░░░░▒▒▒
            */

            {

                Log.Info(" ");
                Log.ColorInfo(colorLog, "  ░░░░░░░░░░░░░░░░░░░░░░░░▄░░  ");
                Log.ColorInfo(colorLog, "  ░░░░░░░░░▐█░░░░░░░░░░░▄▀▒▌░  ");
                Log.ColorInfo(colorLog, "  ░░░░░░░░▐▀▒█░░░░░░░░▄▀▒▒▒▐  ");
                Log.ColorInfo(colorLog, "  ░░░░░░░▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐  ");
                Log.ColorInfo(colorLog, "  ░░░░░▄▄▀▒░▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐  ");
                Log.ColorInfo(colorLog, "  ░░░▄▀▒▒▒░░░▒▒▒░░░▒▒▒▀██▀▒▌  ");
                Log.ColorInfo(colorLog, "  ░░▐▒▒▒▄▄▒▒▒▒░░░▒▒▒▒▒▒▒▀▄▒▒  ");
                Log.ColorInfo(colorLog, "  ░░▌░░▌█▀▒▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐  ");
                Log.ColorInfo(colorLog, "  ░▐░░░▒▒▒▒▒▒▒▒▌██▀▒▒░░░▒▒▒▀▄  ");
                Log.ColorInfo(colorLog, "  ░▌░▒▄██▄▒▒▒▒▒▒▒▒▒░░░░░░▒▒▒▒  ");
                Log.ColorInfo(colorLog, "  ▀▒▀▐▄█▄█▌▄░▀▒▒░░░░░░░░░░▒▒▒  ");
                Log.Info(" ");

            }

        }

        /// <summary>
        /// 测试打印 【FUCK BUG】
        /// </summary>
        /// <param name="colorLog"></param>
        public static void TestPrint_FuckBug(ColorLog colorLog = ColorLog.White)
        {
            /***
            *
            *   █████▒█    ██  ▄████▄   ██ ▄█▀       ██████╗ ██╗   ██╗ ██████╗
            * ▓██   ▒ ██  ▓██▒▒██▀ ▀█   ██▄█▒        ██╔══██╗██║   ██║██╔════╝
            * ▒████ ░▓██  ▒██░▒▓█    ▄ ▓███▄░        ██████╔╝██║   ██║██║  ███╗
            * ░▓█▒  ░▓▓█  ░██░▒▓▓▄ ▄██▒▓██ █▄        ██╔══██╗██║   ██║██║   ██║
            * ░▒█░   ▒▒█████▓ ▒ ▓███▀ ░▒██▒ █▄       ██████╔╝╚██████╔╝╚██████╔╝
            *  ▒ ░   ░▒▓▒ ▒ ▒ ░ ░▒ ▒  ░▒ ▒▒ ▓▒       ╚═════╝  ╚═════╝  ╚═════╝
            *  ░     ░░▒░ ░ ░   ░  ▒   ░ ░▒ ▒░
            *  ░ ░    ░░░ ░ ░ ░        ░ ░░ ░
            *           ░     ░ ░      ░  ░
            */

            {

                Log.Info(" ");
                Log.ColorInfo(colorLog, "   █████▒█    ██  ▄████▄   ██ ▄█▀       ██████╗ ██╗   ██╗ ██████╗");
                Log.ColorInfo(colorLog, " ▓██   ▒ ██  ▓██▒▒██▀ ▀█   ██▄█▒        ██╔══██╗██║   ██║██╔════╝");
                Log.ColorInfo(colorLog, " ▒████ ░▓██  ▒██░▒▓█    ▄ ▓███▄░        ██████╔╝██║   ██║██║  ███╗");
                Log.ColorInfo(colorLog, " ░▓█▒  ░▓▓█  ░██░▒▓▓▄ ▄██▒▓██ █▄        ██╔══██╗██║   ██║██║   ██║");
                Log.ColorInfo(colorLog, " ░▒█░   ▒▒█████▓ ▒ ▓███▀ ░▒██▒ █▄       ██████╔╝╚██████╔╝╚██████╔╝");
                Log.ColorInfo(colorLog, "  ▒ ░   ░▒▓▒ ▒ ▒ ░ ░▒ ▒  ░▒ ▒▒ ▓▒       ╚═════╝  ╚═════╝  ╚═════╝");
                Log.ColorInfo(colorLog, "  ░     ░░▒░ ░ ░   ░  ▒   ░ ░▒ ▒░");
                Log.ColorInfo(colorLog, "  ░ ░    ░░░ ░ ░ ░        ░ ░░ ░");
                Log.ColorInfo(colorLog, "           ░     ░ ░      ░  ░");
                Log.Info(" ");
            }

        }

        /// <summary>
        /// 测试打印 【DaBiDou】
        /// </summary>
        /// <param name="colorLog"></param>
        public static void TestPrint_DaBiDou(ColorLog colorLog = ColorLog.Blue)
        {
            /***
             *                                         ,s555SB@@&                          
             *                                      :9H####@@@@@Xi                        
             *                                     1@@@@@@@@@@@@@@8                       
             *                                   ,8@@@@@@@@@B@@@@@@8                      
             *                                  :B@@@@X3hi8Bs;B@@@@@Ah,                   
             *             ,8i                  r@@@B:     1S ,M@@@@@@#8;                 
             *            1AB35.i:               X@@8 .   SGhr ,A@@@@@@@@S                
             *            1@h31MX8                18Hhh3i .i3r ,A@@@@@@@@@5               
             *            ;@&i,58r5                 rGSS:     :B@@@@@@@@@@A               
             *             1#i  . 9i                 hX.  .: .5@@@@@@@@@@@1               
             *              sG1,  ,G53s.              9#Xi;hS5 3B@@@@@@@B1                
             *               .h8h.,A@@@MXSs,           #@H1:    3ssSSX@1                  
             *               s ,@@@@@@@@@@@@Xhi,       r#@@X1s9M8    .GA981               
             *               ,. rS8H#@@@@@@@@@@#HG51;.  .h31i;9@r    .8@@@@BS;i;          
             *                .19AXXXAB@@@@@@@@@@@@@@#MHXG893hrX#XGGXM@@@@@@@@@@MS        
             *                s@@MM@@@hsX#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&,      
             *              :GB@#3G@@Brs ,1GM@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@B,     
             *            .hM@@@#@@#MX 51  r;iSGAM@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@8     
             *          :3B@@@@@@@@@@@&9@h :Gs   .;sSXH@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:    
             *      s&HA#@@@@@@@@@@@@@@M89A;.8S.       ,r3@@@@@@@@@@@@@@@@@@@@@@@@@@@r    
             *   ,13B@@@@@@@@@@@@@@@@@@@5 5B3 ;.         ;@@@@@@@@@@@@@@@@@@@@@@@@@@@i    
             *  5#@@#&@@@@@@@@@@@@@@@@@@9  .39:          ;@@@@@@@@@@@@@@@@@@@@@@@@@@@;    
             *  9@@@X:MM@@@@@@@@@@@@@@@#;    ;31.         H@@@@@@@@@@@@@@@@@@@@@@@@@@:    
             *   SH#@B9.rM@@@@@@@@@@@@@B       :.         3@@@@@@@@@@@@@@@@@@@@@@@@@@5    
             *     ,:.   9@@@@@@@@@@@#HB5                 .M@@@@@@@@@@@@@@@@@@@@@@@@@B    
             *           ,ssirhSM@&1;i19911i,.             s@@@@@@@@@@@@@@@@@@@@@@@@@@S   
             *              ,,,rHAri1h1rh&@#353Sh:          8@@@@@@@@@@@@@@@@@@@@@@@@@#:  
             *            .A3hH@#5S553&@@#h   i:i9S          #@@@@@@@@@@@@@@@@@@@@@@@@@A.
             *
             *
             *    又看源码，看你妹妹呀！
             */

            {
                Log.Info(" ");
                Log.ColorInfo(colorLog, "                                         ,s555SB@@&                          ");
                Log.ColorInfo(colorLog, "                                      :9H####@@@@@Xi                        ");
                Log.ColorInfo(colorLog, "                                     1@@@@@@@@@@@@@@8                       ");
                Log.ColorInfo(colorLog, "                                   ,8@@@@@@@@@B@@@@@@8                      ");
                Log.ColorInfo(colorLog, "                                  :B@@@@X3hi8Bs;B@@@@@Ah,                   ");
                Log.ColorInfo(colorLog, "             ,8i                  r@@@B:     1S ,M@@@@@@#8;                 ");
                Log.ColorInfo(colorLog, "            1AB35.i:               X@@8 .   SGhr ,A@@@@@@@@S                ");
                Log.ColorInfo(colorLog, "            1@h31MX8                18Hhh3i .i3r ,A@@@@@@@@@5               ");
                Log.ColorInfo(colorLog, "            ;@&i,58r5                 rGSS:     :B@@@@@@@@@@A               ");
                Log.ColorInfo(colorLog, "             1#i  . 9i                 hX.  .: .5@@@@@@@@@@@1               ");
                Log.ColorInfo(colorLog, "              sG1,  ,G53s.              9#Xi;hS5 3B@@@@@@@B1                ");
                Log.ColorInfo(colorLog, "               .h8h.,A@@@MXSs,           #@H1:    3ssSSX@1                  ");
                Log.ColorInfo(colorLog, "               s ,@@@@@@@@@@@@Xhi,       r#@@X1s9M8    .GA981               ");
                Log.ColorInfo(colorLog, "               ,. rS8H#@@@@@@@@@@#HG51;.  .h31i;9@r    .8@@@@BS;i;          ");
                Log.ColorInfo(colorLog, "                .19AXXXAB@@@@@@@@@@@@@@#MHXG893hrX#XGGXM@@@@@@@@@@MS        ");
                Log.ColorInfo(colorLog, "                s@@MM@@@hsX#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&,      ");
                Log.ColorInfo(colorLog, "              :GB@#3G@@Brs ,1GM@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@B,     ");
                Log.ColorInfo(colorLog, "            .hM@@@#@@#MX 51  r;iSGAM@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@8     ");
                Log.ColorInfo(colorLog, "          :3B@@@@@@@@@@@&9@h :Gs   .;sSXH@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:    ");
                Log.ColorInfo(colorLog, "      s&HA#@@@@@@@@@@@@@@M89A;.8S.       ,r3@@@@@@@@@@@@@@@@@@@@@@@@@@@r    ");
                Log.ColorInfo(colorLog, "   ,13B@@@@@@@@@@@@@@@@@@@5 5B3 ;.         ;@@@@@@@@@@@@@@@@@@@@@@@@@@@i    ");
                Log.ColorInfo(colorLog, "  5#@@#&@@@@@@@@@@@@@@@@@@9  .39:          ;@@@@@@@@@@@@@@@@@@@@@@@@@@@;    ");
                Log.ColorInfo(colorLog, "  9@@@X:MM@@@@@@@@@@@@@@@#;    ;31.         H@@@@@@@@@@@@@@@@@@@@@@@@@@:    ");
                Log.ColorInfo(colorLog, "   SH#@B9.rM@@@@@@@@@@@@@B       :.         3@@@@@@@@@@@@@@@@@@@@@@@@@@5    ");
                Log.ColorInfo(colorLog, "     ,:.   9@@@@@@@@@@@#HB5                 .M@@@@@@@@@@@@@@@@@@@@@@@@@B    ");
                Log.ColorInfo(colorLog, "           ,ssirhSM@&1;i19911i,.             s@@@@@@@@@@@@@@@@@@@@@@@@@@S   ");
                Log.ColorInfo(colorLog, "              ,,,rHAri1h1rh&@#353Sh:          8@@@@@@@@@@@@@@@@@@@@@@@@@#:  ");
                Log.ColorInfo(colorLog, "            .A3hH@#5S553&@@#h   i:i9S          #@@@@@@@@@@@@@@@@@@@@@@@@@A.");
                Log.ColorInfo(colorLog, "");
                Log.ColorInfo(colorLog, "");
                Log.ColorInfo(colorLog, "    又看源码，看你妹妹呀！");
                Log.Info(" ");


            }
        }
        #endregion


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
            string prefix = DateTime.Now.ToString("yyyy-MM-dd.HH@");
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
