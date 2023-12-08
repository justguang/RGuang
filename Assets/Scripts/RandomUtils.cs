using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using RGuang.Utils;

public class RandomUtils
{

    /// <summary>
    ///  随机打乱一组数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lst"></param>
    public static void RandomArray<T>(List<T> lst)
    {
        System.Random rand = new System.Random();
        for (int i = lst.Count - 1; i >= 0; i--)
        {
            int j = rand.Next(i + 1);
            T tmp = lst[i];
            lst[i] = lst[j];
            lst[j] = tmp;
            //if (i % 100 == 0) GC.Collect();
        }

        //int count = lst.Count;
        //while (count > 1)
        //{
        //    count--;
        //    int tmpIndex = rand.Next(count + 1);
        //    T tmpValue = lst[tmpIndex];
        //    lst[tmpIndex] = lst[count];
        //    lst[count] = tmpValue;
        //}
    }


    /// <summary>
    /// 获得一串随机字符串【字符串包含大小写字母和数字】
    /// </summary>
    /// <param name="str_len">参数str_len指定获取的长度</param>
    public static string Rand_Str(int str_len)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        System.Random r = new System.Random(System.BitConverter.ToInt32(b, 0));

        string str = null;
        str += "0123456789";
        str += "abcdefghijklmnopqrstuvwxyz";
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string s = null;

        for (int i = 0; i < str_len; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }


    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="str">要加密的字符串</param>
    /// <returns>返回已加密的字符串</returns>
    public static string Md5(string str)
    {
        string cl = str;
        StringBuilder md5_builder = new StringBuilder();
        MD5 md5 = MD5.Create();//实例化一个md5对像
                               // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        for (int i = 0; i < s.Length; i++)
        {
            // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
            md5_builder.Append(s[i].ToString("X2"));
            //pwd = pwd + s[i].ToString("X");

        }
        return md5_builder.ToString();
    }

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
            ULog.Error(e);
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
            ULog.Error(e);
            return -1;
        }
    }



}


