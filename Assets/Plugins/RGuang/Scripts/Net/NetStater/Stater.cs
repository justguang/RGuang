/// <summary>
///********************************************
/// ClassName    ：  Stater
/// Description  ：  可扩展状态机
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;

namespace RGuang.Net.NetState
{
    public class Stater<T> where T : struct
    {
        public Action<T, T> TransCallBack;
        public T curState;

        /// <summary>
        /// 用于一对多状态映射关系
        /// </summary>
        private Dictionary<T, List<StaterItem<T>>> transDic;
        /// <summary>
        /// 用于多对一状态映射关系
        /// </summary>
        private List<StaterItem<T>> transLst;

        private bool isProcessing = false;
        private List<StaterInput> cacheIptLst;

        public Stater(T startState)
        {
            curState = startState;
            transDic = new Dictionary<T, List<StaterItem<T>>>();
            transLst = new List<StaterItem<T>>();
            cacheIptLst = new List<StaterInput>();
        }

        public void AddLocalTransition(T curState, StaterInput ipt, T dstState, Func<T, StaterInput, T, bool> cb)
        {
            var item = new StaterItem<T>(ipt, dstState, cb);
            if (transDic.TryGetValue(curState, out List<StaterItem<T>> itemLst))
            {
                itemLst.Add(item);
            }
            else
            {
                itemLst = new List<StaterItem<T>> {
                    item
                };
                transDic.Add(curState, itemLst);
            }
        }

        public void AddGlobalTransition(StaterInput ipt, T dstState, Func<T, StaterInput, T, bool> cb)
        {
            transLst.Add(new StaterItem<T>(ipt, dstState, cb));
        }

        public void Input(StaterInput ipt)
        {
            if (isProcessing)
            {
                cacheIptLst.Add(ipt);
                return;
            }
            isProcessing = true;

            bool result = false;
            //检测常规转换中当前状态所属的可转换List里是否有满足条件转换项
            if (transDic.TryGetValue(curState, out List<StaterItem<T>> itemLst))
            {
                for (int i = 0; i < itemLst.Count; i++)
                {
                    result = TransWork(itemLst[i], ipt);
                    if (result)
                    {
                        break;
                    }
                }
            }

            //常规转换不存在时，检测全局转换中是否有满足条件的
            if (result == false)
            {
                for (int i = 0; i < transLst.Count; i++)
                {
                    result = TransWork(transLst[i], ipt);
                    if (result)
                    {
                        break;
                    }
                }
            }

            isProcessing = false;
            if (cacheIptLst.Count > 0)
            {
                var item = cacheIptLst[0];
                cacheIptLst.RemoveAt(0);
                Input(item);
            }
        }

        bool TransWork(StaterItem<T> item, StaterInput ipt)
        {
            bool result = false;
            if (item.ipt.Equals(ipt))
            {
                if (item.cb != null)
                {
                    result = item.cb(curState, ipt, item.dstState);
                }
                else
                {
                    result = true;
                }

                if (result)
                {
                    T preState = curState;
                    curState = item.dstState;
                    TransCallBack?.Invoke(preState, curState);
                }
            }
            return result;
        }

        public void ClearInputCache()
        {
            cacheIptLst.Clear();
        }
    }
}
