using System;
using System.Text;
using System.Security.Cryptography;

namespace JustGuang
{
    /// <summary>
    /// 加密 解密类
    /// 
    /// </summary>
    public sealed class Crypto
    {

        /// <summary>
        /// MD5加密 字符串
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>返回已加密的字符串</returns>
        public static string Md5(string str)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException();

            string cl = str;
            StringBuilder md5_builder = new System.Text.StringBuilder();
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                md5_builder.Append(s[i].ToString("X2"));
                //pwd = pwd + s[i].ToString("X");

            }
            return md5_builder.ToString();
        }
    }

}
