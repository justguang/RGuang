/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  优先级队列【以完全二叉树 小顶堆实现】
///********************************************/
using System;
using System.Collections.Generic;

namespace RGuang
{
    public sealed class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> lst = null;
        public int Count { get => lst.Count; }
        public PriorityQueue(int capacity = 4)
        {
            lst = new List<T>(capacity);
        }

        /// <summary>
        /// 进队列
        /// </summary>
        public void Enqueue(T item)
        {
            lst.Add(item);
            HeapfiyUp(lst.Count - 1);
        }

        /// <summary>
        /// 进队列
        /// </summary>
        public void Enqueue(T[] itemArray)
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                Enqueue(itemArray[i]);
            }
        }

        /// <summary>
        /// 进队列
        /// </summary>
        public void Enqueue(List<T> itemList)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                Enqueue(itemList[i]);
            }
        }

        /// <summary>
        /// 移除并返回队列头部元素
        /// </summary>
        public T Dequeue()
        {
            if (lst.Count == 0)
            {
                return default;
            }
            T value = lst[0];
            int endIndex = lst.Count - 1;
            lst[0] = lst[endIndex];
            lst.RemoveAt(endIndex);
            --endIndex;
            HeapfiyDown(0, endIndex);

            return value;
        }

        /// <summary>
        /// 不移除并返回队列头部元素
        /// </summary>
        public T Peek()
        {
            return lst.Count > 0 ? lst[0] : default;
        }

        /// <summary>
        /// 获取元素的索引
        /// </summary>
        public int IndexOf(T item)
        {
            return lst.IndexOf(item);
        }

        /// <summary>
        /// 移除指定索引的元素
        /// </summary>
        public T RemoveAt(int RmvIndex)
        {
            if (lst.Count <= RmvIndex)
            {
                return default;
            }
            T item = lst[RmvIndex];
            int endIndex = lst.Count - 1;
            lst[RmvIndex] = lst[endIndex];
            lst.RemoveAt(endIndex);
            --endIndex;

            if (RmvIndex < endIndex)
            {
                int parentIndex = (RmvIndex - 1) / 2;
                if (parentIndex > 0 && lst[RmvIndex].CompareTo(lst[parentIndex]) < 0)
                {
                    HeapfiyUp(RmvIndex);
                }
                else
                {
                    HeapfiyDown(RmvIndex, endIndex);
                }
            }

            return item;
        }

        /// <summary>
        /// 移除指定元素
        /// </summary>
        public T RemoveItem(T item)
        {
            int index = IndexOf(item);
            return index == -1 ? default : RemoveAt(index);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            lst.Clear();
        }

        /// <summary>
        /// 是否包含指定元素
        /// </summary>
        /// <param name="item">指定元素</param>
        /// <returns>true包含，false不包含</returns>
        public bool Contains(T item)
        {
            return lst.Contains(item);
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty()
        {
            return lst.Count == 0;
        }

        public List<T> ToList()
        {
            return lst;
        }

        public T[] ToArray()
        {
            return lst.ToArray();
        }


        //基于ChildIndex节点向树顶堆化
        void HeapfiyUp(int childIndex)
        {
            int parentIndex = (childIndex - 1) / 2;
            while (childIndex > 0 && lst[childIndex].CompareTo(lst[parentIndex]) < 0)
            {
                Swap(childIndex, parentIndex);
                childIndex = parentIndex;
                parentIndex = (childIndex - 1) / 2;
            }
        }

        //基于topIndex节点，向endIndex节点堆化
        void HeapfiyDown(int topIndex, int endIndex)
        {
            while (true)
            {
                int minIndex = topIndex;

                int childIndex = topIndex * 2 + 1;
                if (childIndex <= endIndex && lst[childIndex].CompareTo(lst[topIndex]) < 0)
                    minIndex = childIndex;

                childIndex = topIndex * 2 + 2;
                if (childIndex <= endIndex && lst[childIndex].CompareTo(lst[minIndex]) < 0)
                    minIndex = childIndex;

                if (topIndex == minIndex) break;

                Swap(topIndex, minIndex);
                topIndex = minIndex;
            }
        }

        void Swap(int a, int b)
        {
            T tmp = lst[a];
            lst[a] = lst[b];
            lst[b] = tmp;
        }
    }
}
