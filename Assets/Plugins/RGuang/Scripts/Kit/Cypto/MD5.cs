using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace RGuang.Kit
{
    public class MD5Kit
    {
        /// <summary>
        /// MD5加密指定文件，并保存md5加密码到指定路径
        /// </summary>
        /// <param name="filePath">要加密的文件</param>
        /// <param name="md5SavePath">加密码保存的路径</param>
        /// <param name="errorLog">错误输出</param>
        public static void SaveMD5(string filePath, string md5SavePath, Action<string> errorLog = null)
        {
            string md5 = BuildFileMD5(filePath, errorLog);
            string name = md5SavePath + "_md5.dat";
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            StreamWriter sw = new StreamWriter(name, false, Encoding.UTF8);
            if (sw != null)
            {
                sw.Write(md5);
                sw.Flush();
                sw.Close();
            }
        }


        /// <summary>
        /// MD5加密文件，并将加密码文件保存同级目录下
        /// </summary>
        /// <param name="filePath">要加密的文件</param>
        /// <param name="errorLog">错误输出</param>
        public static void SaveMD5(string filePath, Action<string> errorLog = null)
        {
            string md5 = BuildFileMD5(filePath, errorLog);
            string name = filePath + "_md5.dat";
            if (File.Exists(name))
            {
                File.Delete(name);
            }

            StreamWriter sw = new StreamWriter(name, false, Encoding.UTF8);
            if (sw != null)
            {
                sw.Write(md5);
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// 获取MD5加密码
        /// </summary>
        /// <param name="path">加密码文件路径</param>
        /// <param name="errorLog">错误输出</param>
        /// <returns></returns>
        public static string GetMD5(string path, Action<string> errorLog = null)
        {
            string name = path + "_md5.dat";
            try
            {
                StreamReader sr = new StreamReader(name, Encoding.UTF8);
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
            catch (Exception e)
            {
                errorLog?.Invoke(e.Message);
                return "";
            }
        }

        /// <summary>
        /// MD5加密指定文件
        /// </summary>
        /// <param name="filePath">文件</param>
        /// <param name="errorLog">错误输出</param>
        /// <returns>返回加密码</returns>
        public static string BuildFileMD5(string filePath, Action<string> errorLog = null)
        {
            string fileMd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定stream 对象的哈希值
                    fileMd5 = FormatMD5(fileMD5Bytes);
                }
            }
            catch (Exception e)
            {
                errorLog?.Invoke(e.Message);
            }
            return fileMd5;
        }


        public static string FormatMD5(Byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();//将byte[]转成字符串
        }

    }


}


