/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  使用外部Update帧循环驱动，并在帧循环中回调
///********************************************/
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;

namespace RGuang.Kit
{
    public sealed class FrameTimer : RTimer
    {
        private ulong currentFrame = 0;//当前处于多少帧
        private readonly Dictionary<int, FrameTask> taskDic;
        private const string tidLock = "FrameTimer_tidLock";

        private List<int> tidCompleteList;

        /// <summary>
        /// 实例一个定时器（需要使用者在外部用update驱动）
        /// </summary>
        /// <param name="frameID">默认帧id=0，任务循环中计算需要的帧id</param>
        public FrameTimer(ulong frameID = 0)
        {
            currentFrame = frameID;
            taskDic = new Dictionary<int, FrameTask>();
            tidCompleteList = new List<int>();

        }

        /// <summary>
        /// 添加一个定时任务
        /// </summary>
        /// <param name="delay">定时任务（每次循环)执行前延迟时间【毫秒】</param>
        /// <param name="taskCallBack">任务执行时回调</param>
        /// <param name="cancelCallBack">任务取消时回调</param>
        /// <param name="count">任务循环执行次数【默认1次，-1表示无限次数】</param>
        /// <returns>添加成功返回任务的唯一id</returns>
        public override int AddTask(uint delay, Action<int> taskCallBack, Action<int> cancelCallBack, int count = 1)
        {
            int tid = GenerateId();
            ulong destFrame = currentFrame + delay;
            FrameTask task = new FrameTask(tid, delay, count, destFrame, taskCallBack, cancelCallBack);

            if (taskDic.ContainsKey(tid))
            {
                WarnFunc.Invoke($"[FrameTimer] 定时任务 id: {tid} 已存在.");
                return -1;
            }
            else
            {
                taskDic.Add(tid, task);
                return tid;
            }

        }


        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="tid">要删除的任务的id</param>
        /// <returns>删除成功返回true</returns>
        public override bool DelTask(int tid)
        {
            if (taskDic.TryGetValue(tid, out FrameTask task))
            {
                if (taskDic.Remove(tid))
                {
                    task?.cancelCallback?.Invoke(tid);
                    return true;
                }
                else
                {
                    ErrorFunc.Invoke($"[FrameTimer] Remove tid:{tid} in taskDic failed.");
                    return false;
                }
            }
            else
            {
                WarnFunc.Invoke($"[FrameTimer] tid:{tid} is not exist in taskDic.");
                return false;
            }

        }

        /// <summary>
        /// 重置清理定时任务
        /// </summary>
        public override void Reset()
        {
            taskDic.Clear();
            tidCompleteList.Clear();
            currentFrame = 0;
            tid = 0;
        }

        /// <summary>
        /// 实例化FrameTimer后，使用者需在外部update调用此函数来驱动定时器
        /// </summary>
        public void UpdateTask()
        {
            ++currentFrame;//每执行一次，代表已过一帧，当前帧id+1
            tidCompleteList.Clear();

            List<int> keyList = taskDic.Keys.ToList();
            int keyCount = keyList.Count;
            for (int i = 0; i < keyCount; i++)
            {
                int key = keyList[i];
                FrameTask task = taskDic[key];
                if (task.destFrame <= currentFrame)
                {
                    task?.taskCallback?.Invoke(task.tid);
                    task.destFrame += task.delay;
                    --task.count;
                    if (task.count == 0)
                    {
                        tidCompleteList.Add(task.tid);
                    }
                }
            }


            for (int i = 0; i < tidCompleteList.Count; i++)
            {
                if (taskDic.Remove(tidCompleteList[i]))
                {
                    LogFunc.Invoke($"[FrameTimer] 定时任务 id:{tidCompleteList[i]} 执行完成.");
                }
                else
                {
                    ErrorFunc.Invoke($"[FrameTimer] Remove tid:{tidCompleteList[i]}  task in taskDic failed.");
                }
            }
        }

        protected override int GenerateId()
        {
            lock (tidLock)
            {
                while (true)
                {
                    ++tid;
                    if (tid == int.MaxValue)
                    {
                        tid = 1;
                    }
                    if (!taskDic.ContainsKey(tid))
                    {
                        return tid;
                    }
                }
            }
        }



        class FrameTask
        {
            public int tid;
            public uint delay;
            public int count;
            public ulong destFrame;

            public Action<int> taskCallback;
            public Action<int> cancelCallback;

            public FrameTask(int tid, uint delay, int count, ulong destFrame, Action<int> taskCallback, Action<int> cancelCallback)
            {
                this.tid = tid;
                this.delay = delay;
                this.count = count;
                this.destFrame = destFrame;
                this.taskCallback = taskCallback;
                this.cancelCallback = cancelCallback;
            }

        }


    }
}
