/// <summary>
///********************************************
/// Description  ：  毫秒级的精确定时器，单线程轮询任务列表
///********************************************/
/// </summary>
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RGuang
{
    public sealed class TickTimer : RTimer
    {
        class TickTaskPack
        {
            public int tid;
            public Action<int> cb;
            public TickTaskPack(int tid, Action<int> cb)
            {
                this.tid = tid;
                this.cb = cb;
            }
        }

        //1970-01-01 00：00：00.00
        private readonly DateTime metaStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private readonly bool setHandle;
        private readonly ConcurrentDictionary<int, TickTask> taskDic;
        private readonly ConcurrentQueue<TickTaskPack> packQueue;
        private const string tidLock = "TickTimer_tidLock";


        private Thread timerThread;

        /// <summary>
        /// 实例化timer，单线程轮询任务列表执行任务
        /// </summary>
        /// <param name="interval">任务驱动器每次驱动(任务列表轮询)的间隔时间【默认20，单位毫秒】</param>
        /// <param name="setHandle">默认false，如果为true则需要使用者在外部update中调用
        /// [HandleTask]驱动；如果为false，则tiemr内部update驱动</param>
        public TickTimer(int interval = 20, bool setHandle = false)
        {
            this.setHandle = setHandle;
            taskDic = new ConcurrentDictionary<int, TickTask>();

            if (setHandle)
            {
                packQueue = new ConcurrentQueue<TickTaskPack>();
            }

            if (interval > 0)
            {
                void StartTick()
                {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                UpdateTask();
                                Thread.Sleep(interval);
                            }
                            catch (Exception e)
                            {
                                if (e is ThreadAbortException || e is ThreadInterruptedException) throw e;
                                ErrorFunc.Invoke($"[TickTimer] Error:{e}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WarnFunc.Invoke($"[TickTiemr] Tick Thread Abort:{e}.");
                    }
                }

                timerThread = new Thread(new ThreadStart(StartTick));
                timerThread.IsBackground = true;
                timerThread.Start();
            }
        }

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="delay">每次(循环)任务开始执行时的延时时间【单位毫秒】</param>
        /// <param name="taskCallBack">任务执行时的回调</param>
        /// <param name="cancelCallBack">任务取消时的回调</param>
        /// <param name="count">指定该任务循环多少次【默认1次,-1无限次】</param>
        /// <returns>返回该任务的id</returns>
        public override int AddTask(uint delay, Action<int> taskCallBack, Action<int> cancelCallBack, int count = 1)
        {
            int tid = GenerateId();
            double startTime = GetUTCMilliseconds();
            double destTime = startTime + delay;
            TickTask task = new TickTask(tid, delay, count, destTime, taskCallBack, cancelCallBack, startTime);
            if (taskDic.TryAdd(tid, task))
            {
                return tid;
            }
            else
            {
                WarnFunc.Invoke($"[TickTimer] 定时任务 id: {tid} 已存在~!");
                return -1;
            }

        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="tid">要删除的任务的id</param>
        /// <returns>返回true删除成功</returns>
        public override bool DelTask(int tid)
        {
            if (taskDic.TryRemove(tid, out TickTask task))
            {
                if (setHandle && task.cancelCallBack != null)
                {
                    packQueue.Enqueue(new TickTaskPack(tid, task.cancelCallBack));
                }
                else
                {
                    task.cancelCallBack?.Invoke(tid);
                }
                return true;
            }
            else
            {
                WarnFunc.Invoke($"[TickTimer] tid:{tid} remove failed.");
                return false;
            }
        }

        /// <summary>
        /// 重置，停止并清理所有任务
        /// </summary>
        public override void Reset()
        {
            if (packQueue != null && !packQueue.IsEmpty)
            {
                WarnFunc.Invoke($"[TickTimer] callback queue is not empty.");
            }

            taskDic.Clear();
            if (timerThread != null)
            {
                timerThread.Interrupt();
                timerThread.Abort();
            }
            tid = 0;
        }

        /// <summary>
        /// 如果实例TickTimer定时器时，interval参数为0，则需要使用者外部update调用此函数来驱动定时器
        /// </summary>
        public void UpdateTask()
        {
            double nowTime = GetUTCMilliseconds();
            foreach (var item in taskDic)
            {
                TickTask task = item.Value;
                if (nowTime < task.destTime) continue;

                ++task.loopIndex;
                if (task.count > 0)
                {
                    --task.count;
                    if (task.count == 0)
                    {
                        //taskDic属于线程安全字典， 遍历中删除元素无影响
                        FinishTask(task.tid);
                    }
                    else
                    {
                        //为避免循环次数增加而累积产生的误差，每次计算延时执行时，都是以最开始指定的时间计算
                        task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                        CallTaskCB(task.tid, task.taskCallBack);
                    }
                }
                else
                {
                    task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                    CallTaskCB(task.tid, task.taskCallBack);
                }
            }
        }

        /// <summary>
        /// 如果实例TickTimer定时器时，setHandle参数为true，则需要使用者外部update调用此函数来驱动定时任务执行回调
        /// </summary>
        public void HandleTask()
        {
            while (packQueue != null && packQueue.Count > 0)
            {
                if (packQueue.TryDequeue(out TickTaskPack pack))
                {
                    pack.cb.Invoke(pack.tid);
                }
                else
                {
                    ErrorFunc.Invoke($"[TickTimer] packQueue dequeue data Error!!!");
                }
            }
        }

        void FinishTask(int tid)
        {
            if (taskDic.TryRemove(tid, out TickTask task))
            {
                CallTaskCB(task.tid, task.taskCallBack);
                task.taskCallBack = null;
            }
            else
            {
                WarnFunc.Invoke($"[TickTimer] Remove tid:{tid} task in Dic failed.");
            }
        }
        void CallTaskCB(int tid, Action<int> taskCB)
        {
            if (setHandle)
            {
                packQueue.Enqueue(new TickTaskPack(tid, taskCB));
            }
            else
            {
                taskCB.Invoke(tid);
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
        private double GetUTCMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - metaStartDateTime;
            return ts.TotalMilliseconds;
        }


        class TickTask
        {
            public int tid;
            public uint delay;//任务(循环)开始执行时的延迟【毫秒】
            public int count;//任务循环次数(默认1次)
            public double destTime;
            public Action<int> taskCallBack;
            public Action<int> cancelCallBack;

            public double startTime;
            public ulong loopIndex;//任务已循环次数

            public TickTask(
                int tid,
                uint delya,
                int count,
                double destTime,
                Action<int> taskCallBack,
                Action<int> cancelCallBack,
                double startTime)
            {
                this.tid = tid;
                this.delay = delya;
                this.count = count;
                this.destTime = destTime;
                this.taskCallBack = taskCallBack;
                this.cancelCallBack = cancelCallBack;
                this.startTime = startTime;

                this.loopIndex = 0;
            }
        }

    }
}
