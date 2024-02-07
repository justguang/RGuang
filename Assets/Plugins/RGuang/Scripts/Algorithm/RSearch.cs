using System;
using System.Collections.Generic;

namespace RGuang
{

    /// <summary>
    /// 查找
    /// 
    /// </summary>
    public static class RSearch
    {
        #region 线性搜索
        /// <summary>
        /// 线性搜索
        /// 如果查找到，返回元素所在索引，如果没有找到返回-1
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int LinearSearch(int[] arr, int target)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 线性搜索
        /// 如果查找到，返回元素所在索引，如果没有找到返回-1
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int LinearSearch<T>(T[] arr, T target)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(target))
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #region 二分查找
        /// <summary>
        /// 二分查找
        /// 如果查找到，返回元素所在索引，如果没有找到返回-1
        /// </summary>
        /// <param name="sortedArr"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int BinarySearch(int[] sortedArr, int target)
        {
            int left = 0;
            int right = sortedArr.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (sortedArr[mid] == target)
                {
                    return mid;
                }
                else if (sortedArr[mid] < target)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }
        /// <summary>
        /// 二分查找
        /// 如果查找到，返回元素所在索引，如果没有找到返回-1
        /// </summary>
        /// <param name="sortedArr"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int BinarySearch<T>(T[] sortedArr, T target) where T : IComparable<T>
        {
            int left = 0;
            int right = sortedArr.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (sortedArr[mid].CompareTo(target) == 0)
                {
                    return mid;
                }
                else if (sortedArr[mid].CompareTo(target) < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }
        #endregion


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

    }

}
