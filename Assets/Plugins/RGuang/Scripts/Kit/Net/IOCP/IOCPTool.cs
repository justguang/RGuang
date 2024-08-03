/// <summary>
///********************************************
/// ClassName    ：  IOCPTool
/// Description  ：  工具函数
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace RGuang.Kit.Net.IOCPNet
{
    /// <summary>
    /// 工具函数
    /// </summary>
    public static class IOCPTool
    {
        /// <summary>
        /// 处理数据粘包分包
        /// </summary>
        /// <param name="byteList">所有接收的数据</param>
        /// <returns>返回一个完整的数据包，如果不完整返回null</returns>
        public static byte[] SplitLogicBytes(ref List<byte> byteList)
        {
            byte[] buff = null;
            if (byteList.Count > 4)
            {
                byte[] data = byteList.ToArray();
                int len = BitConverter.ToInt32(data, 0);
                if (byteList.Count >= len + 4)
                {
                    buff = new byte[len];
                    Buffer.BlockCopy(data, 4, buff, 0, len);
                    byteList.RemoveRange(0, len + 4);
                }
            }
            return buff;
        }

        /// <summary>
        /// 打包网络数据
        /// </summary>
        /// <param name="body">数据体</param>
        /// <returns>返回带有指示包体长度的包头的数据包，【数据包头占4个字节】</returns>
        public static byte[] PackLenInfo(byte[] body)
        {
            int len = body.Length;
            byte[] pkg = new byte[len + 4];
            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pkg, 0);
            body.CopyTo(pkg, 4);
            return pkg;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="msg">要序列化的对象</param>
        public static byte[] Serialize<T>(T msg) where T : IOCPMsg
        {
            byte[] data = null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                bf.Serialize(ms, msg);
                ms.Seek(0, SeekOrigin.Begin);
                data = ms.ToArray();
            }
            catch (SerializationException e)
            {
                Error("Failed to serialie. Reson:{0}", e.Message);
            }
            finally
            {
                ms.Close();
            }
            return data;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes">要反序列化的字节数组</param>
        public static T DesSerialize<T>(byte[] bytes) where T : IOCPMsg
        {
            T msg = null;
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                msg = (T)bf.Deserialize(ms);
            }
            catch (SerializationException e)
            {
                Error("Failed to desSerialie. Reson:{0} , bytesLen:{1}", e.Message, bytes.Length);
            }
            finally
            {
                ms.Close();
            }
            return msg;
        }

        #region LOG日志
        public static Action<string> LogFunc;
        public static Action<IOCPLogColor, string> ColorLogFunc;
        public static Action<string> WarnFunc;
        public static Action<string> ErrorFunc;

        public static void Log(string msg, params object[] args)
        {
            msg = string.Format(msg, args);
            msg = Decorate(msg);
            if (LogFunc != null)
            {
                LogFunc(msg);
            }
            else
            {
                ConsoleLog(msg, IOCPLogColor.None);
            }

            WriteToFile(msg);
        }
        public static void ColorLog(IOCPLogColor color, string msg, params object[] args)
        {
            msg = string.Format(msg, args);
            msg = "【ColorLog】" + Decorate(msg);
            if (ColorLogFunc != null)
            {
                ColorLogFunc(color, msg);
            }
            else
            {
                ConsoleLog(msg, color);
            }

            WriteToFile(msg);
        }
        public static void Warn(string msg, params object[] args)
        {
            msg = string.Format(msg, args);
            msg = "【Warn】" + Decorate(msg) + "\nStackTrace：" + GetLogTrace();
            if (WarnFunc != null)
            {
                WarnFunc(msg);
            }
            else
            {
                ConsoleLog(msg, IOCPLogColor.Yellow);
            }
            WriteToFile(msg);
        }
        public static void Error(string msg, params object[] args)
        {
            msg = string.Format(msg, args);
            msg = "【Error】" + Decorate(msg) + "\nStackTrace：" + GetLogTrace();
            if (ErrorFunc != null)
            {
                ErrorFunc(msg);
            }
            else
            {
                ConsoleLog(msg, IOCPLogColor.Red);
            }
            WriteToFile(msg);
        }
        static void ConsoleLog(string msg, IOCPLogColor color)
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            msg = string.Format("ThreadID:{0} >> {1}", threadID, msg);
            switch (color)
            {
                case IOCPLogColor.Red:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.Cyan:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.Magenta:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.Yellow:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case IOCPLogColor.None:
                default:
                    Console.WriteLine(msg);
                    break;
            }
        }


        //修饰内容
        static string Decorate(string msg)
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff");
            msg = string.Format("[{0}] ThreadID:{1} >> {2}", time, threadID, msg);
            return msg;
        }


        //获取堆栈信息
        static string GetLogTrace()
        {
            StackTrace st = new StackTrace(2, true);
            string traceInfo = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame tempSF = st.GetFrame(i);
                traceInfo += string.Format("\n {0}::{1} line:{2}", tempSF.GetFileName(), tempSF.GetMethod(), tempSF.GetFileLineNumber());
            }
            return traceInfo;
        }
        #endregion

        #region 保存日志到文件
        static bool enableSaveToFile;//true【开启保存日志到本地文件】，false不开启
        static string saveFileName = "IOCPLog.txt";//保存的文件名
        static string savePath;//日志文件保存路径
        public static void EnableLogSaveToFile(string fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Trim();
                string tmp = fileName.Substring(fileName.Length - 4, 4);
                if (!tmp.Contains(".txt"))
                {
                    fileName += ".txt";
                }
                saveFileName = fileName;


                enableSaveToFile = true;
                savePath = string.Format("{0}Log\\", AppDomain.CurrentDomain.BaseDirectory);

                try
                {
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                }
                catch
                {
                    //
                }
            }
        }

        //写入文件
        static void WriteToFile(string msg)
        {
            if (!enableSaveToFile) return;

            string prefix = DateTime.Now.ToString("yyyy-MM-dd@");
            string fileName = prefix + saveFileName;
            try
            {
                File.AppendAllText(savePath + fileName, msg + "\n", Encoding.UTF8);
            }
            catch (Exception e)
            {
                //
            }
        }
        #endregion
    }


    /// <summary>
    /// 日志颜色
    /// </summary>
    public enum IOCPLogColor
    {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow,
    }
}
