using System;
using System.IO;
using System.Text;

namespace RGuang.Kit
{
    public static class FileKit
    {
        #region 文件读取
        #region 通过File类读取文件
        /// <summary>
        /// 读取文件【通过File类的ReadAllText读取文件】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="errorCallback">异常回调</param>
        /// <returns>返回读取结果</returns>
        public static string ReadAllText(string path, Encoding encoding, Action<string> errorCallback = null)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke($"读取文件失败！无效文件路径");
                return result;
            }

            if (File.Exists(path) == false)
            {
                errorCallback?.Invoke($"读取文件失败！不存在的文件路径=[{path}]");
                return result;
            }

            try
            {
                result = File.ReadAllText(path, encoding);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"读取文件失败！{e.Message}");
            }

            return result;

        }

        /// <summary>
        /// 读取文件【通过File类的ReadAllLine读取文件】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="errorCallback">异常回调</param>
        /// <returns>返回读取结果</returns>
        public static string[] ReadAllLines(string path, Encoding encoding, Action<string> errorCallback = null)
        {

            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke($"读取文件失败！无效文件路径");
                return null;
            }

            if (File.Exists(path) == false)
            {
                errorCallback?.Invoke($"读取文件失败！不存在的文件路径=[{path}]");
                return null;
            }

            try
            {
                string[] result = File.ReadAllLines(path, encoding);
                return result;
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"读取文件失败！{e.Message}");

                return null;
            }

        }
        #endregion

        #region 以文件流的形式读取文件
        /// <summary>
        /// 读取文件【通过IO命名空间下的FileStream类读取文件，默认utf8编码】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="errorCallback">异常回调</param>
        /// <returns>返回读取结果</returns>
        public static string ReadFileByFileStream(string path, Action<string> errorCallback = null)
        {

            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke($"读取文件失败！无效文件路径");
                return result;
            }

            if (File.Exists(path) == false)
            {
                errorCallback?.Invoke($"读取文件失败！不存在的文件路径=[{path}]");
                return result;
            }

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    result = Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"读取文件失败！{e.Message}");
            }

            return result;
        }

        /// <summary>
        /// 读取文件【通过File类的OpenRead读取文件，默认utf8编码】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="errorCallback">异常回调</param>
        /// <returns>返回读取结果</returns>
        public static string ReadFileByFileOpenRead(string path, Action<string> errorCallback = null)
        {

            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke($"读取文件失败！无效文件路径");
                return result;
            }

            if (File.Exists(path) == false)
            {
                errorCallback?.Invoke($"读取文件失败！不存在的文件路径=[{path}]");
                return result;
            }

            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    result = Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"读取文件失败！{e.Message}");
            }

            return result;
        }

        /// <summary>
        /// 读取文件【通过IO命名空间下的StreamReader类读取文件】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="errorCallback">异常回调</param>
        /// <returns>返回读取结果</returns>
        public static string ReadFileByStreamReader(string path, Action<string> errorCallback = null)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke($"读取文件失败！无效文件路径");
                return result;
            }

            if (File.Exists(path) == false)
            {
                errorCallback?.Invoke($"读取文件失败！不存在的文件路径=[{path}]");
                return result;
            }


            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"读取文件失败！{e.Message}");
            }


            return result;
        }

        #endregion
        #endregion








        #region 文件写入
        #region 通过File类写入数据
        /// <summary>
        /// 写数据到文件中【用File类的WriteAllText方式写入（文件不存在默认创建）】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="context">数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="append">如果文件存在该参数有效，true追加数据，false覆盖原有数据</param>
        /// <param name="errorCallback">异常回调</param>
        public static void WriteFileByFileClass(string path, string context, Encoding encoding, bool append, Action<string> errorCallback = null)
        {

            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke("写入文件失败！无效路径");
                return;
            }


            try
            {
                if (append)
                {
                    File.AppendAllText(path, context, encoding);
                }
                else
                {
                    File.WriteAllText(path, context, encoding);
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"写入文件失败！{e.Message}");
            }
        }
        #endregion

        #region 通过文件流写入数据
        /// <summary>
        /// 写入数据自动创建文件【用FileStream类字节流形式写入，默认utf8编码】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="context">数据</param>
        /// <param name="append">true覆盖文件内原数据</param>
        /// <param name="errorCallback">异常回调</param>
        public static void WriteFileByFileStream(string path, string context, bool append, Action<string> errorCallback = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke("写入文件失败！无效路径");
                return;
            }

            bool fileExist = File.Exists(path);

            try
            {
                FileMode fm = fileExist ? (append ? FileMode.Append : FileMode.Open) : FileMode.Create;

                using (FileStream fs = new FileStream(path, fm, FileAccess.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(context);

                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }

            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"写入文件失败！{e.Message}");
            }

        }
        /// <summary>
        /// 写入数据自动创建文件【用StreamWriter类写入】
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="context">数据</param>
        /// <param name="encoding"><编码/param>
        /// <param name="append">true追加写入数据，false覆盖原有数据</param>
        /// <param name="errorCallback">异常回调</param>
        public static void WriteFileByFileWriter(string path, string context, Encoding encoding, bool append, Action<string> errorCallback = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                errorCallback?.Invoke("写入文件失败！无效路径");
                return;
            }

            if (File.Exists(path) == false)
            {
                var fs = File.Create(path);
                fs.Close();
                fs.Dispose();
            }

            try
            {
                using (StreamWriter sr = new StreamWriter(path, append, encoding))
                {
                    sr.Write(context);
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke($"写入文件失败！{e.Message}");
            }

        }
        #endregion
        #endregion



    }

}
