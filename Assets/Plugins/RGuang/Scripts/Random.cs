using System;
using System.Collections.Generic;

namespace JustGuang
{
    /// <summary>
    /// 随机类
    /// 
    /// </summary>
    public sealed class Random
    {

        /// <summary>
        ///  随机打乱一组数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        public static void RandomArray<T>(List<T> lst)
        {
            if (lst == null) throw new ArgumentNullException();

            System.Random rand = new System.Random();
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                int j = rand.Next(i + 1);
                T tmp = lst[i];
                lst[i] = lst[j];
                lst[j] = tmp;
                //if (i % 100 == 0) GC.Collect();
            }

        }

        /// <summary>
        ///  随机打乱一组数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        public static void RandomArray<T>(T[] arr)
        {
            if (arr == null) throw new ArgumentNullException();

            System.Random rand = new System.Random();
            for (int i = arr.Length- 1; i >= 0; i--)
            {
                int j = rand.Next(i + 1);
                T tmp = arr[i];
                arr[i] = arr[j];
                arr[j] = tmp;
                //if (i % 100 == 0) GC.Collect();
            }

        }


        /// <summary>
        /// 获得一串随机字符串【字符串包含大小写字母和数字】
        /// </summary>
        /// <param name="str_len">参数str_len指定获取的长度</param>
        public static string Rand_Str(uint str_len)
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




    }

}

