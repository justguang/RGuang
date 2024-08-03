/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  计时任务抽象
///********************************************/
/// </summary>
using System;

namespace RGuang.Kit
{
    public abstract class RTimer
    {
        public Action<string> LogFunc = delegate { };
        public Action<string> WarnFunc = delegate { };
        public Action<string> ErrorFunc = delegate { };

        /// <summary>
        /// 添加、创建定时任务
        /// </summary>
        /// <param name="delay">定时任务</param>
        /// <param name="taskCallBack">定时任务回调</param>
        /// <param name="cancelCallBack">取消任务回调</param>
        /// <param name="count">任务重复计数</param>
        /// <returns>当前计时器唯一ID</returns>
        public abstract int AddTask(uint delay, Action<int> taskCallBack, Action<int> cancelCallBack, int count = 1);

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="tid">定时任务ID</param>
        /// <returns>返回是否成功删除的结果</returns>
        public abstract bool DelTask(int tid);

        /// <summary>
        /// 重置定时器
        /// </summary>
        public abstract void Reset();

        protected int tid = 0;
        protected abstract int GenerateId();

    }
}
