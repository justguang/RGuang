using System;
using System.Collections.Generic;

namespace RGuang
{

    /// <summary>
    /// 查找类
    /// 
    /// </summary>
    public sealed class RSearch
    {
        /// <summary>
        /// 从一组中随机选一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static T ChooseOne<T>(IList<T> lst)
        {
            if (lst == null) throw new ArgumentNullException();

            return lst[UnityEngine.Random.Range(0, lst.Count)];
        }
        /// <summary>
        /// 从一组中随机选一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T ChooseOne<T>(T[] arr)
        {
            if (arr == null) throw new ArgumentNullException();

            return arr[UnityEngine.Random.Range(0, arr.Length)];
        }

        /// <summary>
        /// 从已排序的集合中查找
        /// 
        ///     可能出现的异常：
        ///         ArgumentNullException：lst参数为null
        ///         InvalidOperationException：集合中的元素未实现IComparer<T>接口
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="search"></param>
        /// <returns>如果存在返回对象，如果不存在返回默认</returns>
        public static T BinarySearchIfExists<T>(List<T> lst, T search)
        {
            if (lst == null) throw new ArgumentNullException();

            int index = lst.BinarySearch(search);
            if (index >= 0)
            {
                return lst[index];
            }
            return default(T);

        }
        /// <summary>
        /// 从已排序的集合中查找。
        ///     可能出现的异常：
        ///         ArgumentNullException：arr参数为null
        ///         RankException：arr是多维的
        ///         ArgumentException：arr中的元素与要查找的内容不兼容
        ///         InvalidOperationException：集合中的元素未实现IComparer<T>接口
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="search"></param>
        /// <returns>如果存在返回对象，如果不存在返回默认</returns>
        public static T BinarySearchIfExists<T>(T[] arr, T search)
        {
            int index = Array.BinarySearch(arr, search);
            if (index >= 0)
            {
                return arr[index];
            }
            return default(T);

        }

        /// <summary>
        /// 从已排序好的集合中插入新元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="insert"></param>
        /// <returns>如果元素已存在返回存在的元素，如果不存在则插入元素并返回插入后的元素</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T InsertIfNotExists<T>(List<T> lst, T insert)
        {
            if (lst == null) throw new ArgumentNullException();

            int index = lst.BinarySearch(insert);
            if (index >= 0 && lst[index].Equals(insert))
            {
                return lst[index];
            }
            lst.Insert(~index, insert);
            return insert;
        }

        /// <summary>
        ///  简单二分查
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static string BinarySearch(List<string> lst, string search)
        {
            if (lst == null) throw new ArgumentNullException();

            int leftIndex = 0;
            int rightIndex = lst.Count - 1;
            while (leftIndex <= rightIndex)
            {
                int midIndex = (leftIndex + rightIndex) / 2;
                string tmpData = lst[midIndex];
                if (tmpData.Equals(search))
                {
                    return lst[midIndex];
                }
                else if (tmpData.CompareTo(search) < 0)
                {
                    leftIndex = midIndex + 1;
                }
                else
                {
                    rightIndex = midIndex - 1;
                }
            }

            return null;
        }



    }

    #region 比较器 string实现示例
    /// <summary>
    /// IComparer 简单示例
    /// </summary>
    public class DinoComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal.
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater.
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the
                    // lengths of the two strings.
                    //
                    int retval = x.Length.CompareTo(y.Length);

                    if (retval != 0)
                    {
                        // If the strings are not of equal length,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        //
                        return x.CompareTo(y);
                    }
                }
            }
        }
    }
    #endregion

}
