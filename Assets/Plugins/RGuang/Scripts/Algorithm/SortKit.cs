using System;


namespace RGuang
{
    /// <summary>
    /// 排序
    /// </summary>
    public static class SortKit
    {
        static void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        #region 冒泡
        /// <summary>
        /// 冒泡
        /// </summary>
        /// <param name="arr"></param>
        public static void BubbleSort(int[] arr)
        {
            int n = arr.Length - 1;
            bool swapped;
            for (int i = 0; i < n; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i; i++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                        swapped = true;
                    }
                }

                if (swapped == false)
                {
                    //如果一轮都没有发生交换，则说明数组已是有序的，无需继续排序
                    break;
                }
            }
        }

        /// <summary>
        /// 冒泡2
        /// </summary>
        /// <param name="arr"></param>
        public static void OptimizedBubbleSort(int[] arr)
        {
            int n = arr.Length - 1;
            bool swapped;
            for (int i = 0; i < n; i++)
            {
                swapped = false;
                //前向冒泡，找到最大值移动到末尾
                for (int j = 0; j < n - i; i++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                        swapped = true;
                    }
                }

                if (swapped == false)
                {
                    //如果一轮都没有发生交换，则说明数组已是有序的，无需继续排序
                    break;
                }

                swapped = false;
                //后向冒泡，找到最小值移动到前端
                for (int j = n - 1; j > i; j--)
                {
                    if (arr[j] < arr[j - 1])
                    {
                        Swap(ref arr[j], ref arr[j - 1]);
                        swapped = true;
                    }
                }
            }
        }
        #endregion

        #region 选择
        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="arr"></param>
        public static void SelectionSort(int[] arr)
        {
            int n = arr.Length - 1;
            for (int i = 0; i < n; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    Swap(ref arr[i], ref arr[minIndex]);
                }
            }
        }


        #endregion

        #region 插入
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="arr"></param>
        public static void InsertionSort(int[] arr)
        {
            int n = arr.Length - 1;
            for (int i = 0; i < n; i++)
            {
                int key = arr[i];
                int j = i - 1;
                while (j > 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }
        #endregion

        #region 快排
        /// <summary>
        /// 快排
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public static void QuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(arr, low, high);
                QuickSort(arr, low, pivotIndex - 1);
                QuickSort(arr, pivotIndex + 1, high);
            }
        }
        /// <summary>
        /// 快排 T:IComparable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public static void QuickSort<T>(T[] arr, int low, int high) where T : IComparable<T>
        {
            if (low < high)
            {
                int pivotIndex = Partition(arr, low, high);
                QuickSort(arr, low, pivotIndex - 1);
                QuickSort(arr, pivotIndex + 1, high);
            }
        }
        static int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (arr[j] <= pivot)
                {
                    i++;
                    Swap(ref arr[i], ref arr[j]);
                }
            }
            Swap(ref arr[i + 1], ref arr[high]);
            return i + 1;
        }
        static int Partition<T>(T[] arr, int low, int high) where T : IComparable<T>
        {
            T pivotValue = arr[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (arr[j].CompareTo(pivotValue) <= 0)
                {
                    i++;
                    Swap(ref arr[i], ref arr[j]);
                }
            }
            Swap(ref arr[i + 1], ref arr[high]);
            return i + 1;
        }
        #endregion

        #region 归并
        /// <summary>
        /// 归并
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void MergeSort(int[] arr, int left, int right)
        {
            if (right <= left)
            {
                return;
            }
            int mid = left + (right - left) / 2;
            MergeSort(arr, left, mid);
            MergeSort(arr, mid + 1, right);
            Merge(arr, left, mid, right);
        }
        static void Merge(int[] arr, int left, int mid, int right)
        {
            int[] tmp = new int[right - left + 1];
            int i = left;
            int j = mid + 1;
            int k = 0;
            while (i <= mid && j <= right)
            {
                if (arr[i] <= arr[j])
                {
                    tmp[k++] = arr[i++];
                }
                else
                {
                    tmp[k++] = arr[j++];

                }
            }

            while (i <= mid)
            {
                tmp[k++] = arr[i++];
            }

            while (j <= right)
            {
                tmp[k++] = arr[j++];
            }

            Array.Copy(tmp, 0, arr, left, tmp.Length);
        }
        #endregion

    }


}

