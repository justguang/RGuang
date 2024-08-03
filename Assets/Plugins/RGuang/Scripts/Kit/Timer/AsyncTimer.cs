/// <summary>
///********************************************
/// Author       ：  justguang
/// Description  ：  使用Async await语法驱动，运行在线程池中
///********************************************/
/// </summary>
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RGuang.Kit
{
    public sealed class AsyncTimer : RTimer
    {
        class AsyncTaskPack
        {
            public int tid;
            public Action<int> cb;

            public AsyncTaskPack(int tid, Action<int> cb)
            {
                this.tid = tid;
                this.cb = cb;
            }
        }

        private readonly ConcurrentDictionary<int, CancellationTokenSource> taskCancelDic;
        private const string tidLock = "AsyncTimer_tidLock";

        /// <summary>
        /// 实例化 AsyncTimer，运行在线程池
        /// </summary>
        public AsyncTimer()
        {
            this.taskCancelDic = new ConcurrentDictionary<int, CancellationTokenSource>();
        }

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="delay">每次任务循环开始执行的延迟时间【单位毫秒】</param>
        /// <param name="taskCallBack">任务执行时回调</param>
        /// <param name="cancelCallBack">任务取消时回调</param>
        /// <param name="count">指定任务循环多少次【默认1次, -1无限循环】</param>
        /// <returns>返回该任务的id</returns>
        public override int AddTask(
            uint delay,
            Action<int> taskCallBack,
            Action<int> cancelCallBack,
            int count = 1)
        {
            int tid = GenerateId();
            AsyncTask task = new AsyncTask(tid, delay, count, taskCallBack, cancelCallBack);
            CancellationTokenSource cts = new CancellationTokenSource();

            if (this.taskCancelDic.TryAdd(tid, cts))
            {
                RunTaskInPool(task, cts.Token);
                return tid;
            }
            else
            {
                WarnFunc.Invoke($"[AsyncTimer] 定时任务id:{tid} 已存在~!");
                return -1;
            }

        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="tid">要删除的任务的id</param>
        /// <returns>删除成功返回true</returns>
        public override bool DelTask(int tid)
        {
            if (this.taskCancelDic.TryRemove(tid, out CancellationTokenSource cts))
            {
                LogFunc.Invoke($"[AsyncTimer] 删除定时任务 id:{tid} 成功.");
                cts.Cancel();

                return true;
            }
            else
            {
                ErrorFunc.Invoke($"[AsyncTimer] Remove tid:{tid} task in taskDic failed.");
                return false;
            }

        }


        /// <summary>
        /// 重置，停止并清理所有任务
        /// </summary>
        public override void Reset()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                foreach (var item in taskCancelDic)
                {
                    item.Value.Cancel();
                }
                taskCancelDic.Clear();
                tid = 0;
            }));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }


        void RunTaskInPool(AsyncTask task, CancellationToken cancelTokan)
        {
            Task.Run(async () =>
            {
                if (task.count > 0)
                {
                    #region 限定次数的循环任务
                    do
                    {
                        --task.count;//可循环次数-1
                        ++task.loopIndex;//已循环次数+1
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0)
                        {
                            await Task.Delay(delay, task.ct);
                        }
                        TimeSpan ts = DateTime.UtcNow - task.startTime;
                        task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);

                        if (cancelTokan.IsCancellationRequested)
                        {
                            //任务取消
                            WarnFunc.Invoke($"[AsyncTimer] 定时任务 id={task.tid} 取消");
                            task.cancelCallBack?.Invoke(task.tid);
                            return;
                        }

                        try
                        {
                            CallBackTaskCB(task);
                        }
                        catch (Exception e)
                        {
                            ErrorFunc.Invoke("[AsyncTimer Error]");
                            ErrorFunc.Invoke(e.ToString());
                            //task.cancelCallBack?.Invoke(task.tid);
                            return;
                        }

                    } while (task.count > 0);
                    #endregion

                }
                else
                {
                    #region 无限次数循环任务
                    while (true)
                    {
                        ++task.loopIndex;//已循环次数+1
                        int delay = (int)(task.delay + task.fixDelta);
                        if (delay > 0)
                        {
                            await Task.Delay(delay, task.ct);
                        }
                        TimeSpan ts = DateTime.UtcNow - task.startTime;
                        task.fixDelta = (int)(task.delay * task.loopIndex - ts.TotalMilliseconds);

                        if (cancelTokan.IsCancellationRequested)
                        {
                            //任务取消
                            WarnFunc.Invoke($"[AsyncTimer] 定时任务 id={task.tid} 取消");
                            task.cancelCallBack?.Invoke(task.tid);
                            return;
                        }

                        try
                        {
                            CallBackTaskCB(task);
                        }
                        catch (Exception e)
                        {
                            ErrorFunc.Invoke("[AsyncTimer Error]");
                            ErrorFunc.Invoke(e.ToString());
                            //task.cancelCallBack?.Invoke(task.tid);
                            return;
                        }
                    }
                    #endregion
                }
            }, cancelTokan);
        }


        void CallBackTaskCB(AsyncTask task)
        {
            task.taskCallBack.Invoke(task.tid);

            if (task.count == 0)
            {
                if (taskCancelDic.TryRemove(task.tid, out CancellationTokenSource cts))
                {
                    LogFunc.Invoke($"[AsyncTimer] 定时任务 id:{task.tid} 执行完成.");
                }
                else
                {
                    ErrorFunc.Invoke($"[AsyncTimer] Remove tid:{task.tid} task in taskDic failed.");
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
                    if (!taskCancelDic.ContainsKey(tid))
                    {
                        return tid;
                    }
                }
            }
        }


        class AsyncTask
        {
            public int tid;//唯一id
            public uint delay;//任务(循环)开始执行时的延迟【毫秒】
            public int count;//任务循环次数(默认1次)

            public Action<int> taskCallBack;//任务执行回调
            public Action<int> cancelCallBack;//任务取消回调

            public DateTime startTime;//开始时间
            public ulong loopIndex;//任务已循环次数
            public int fixDelta;//延迟累积产生的误差

            //取消任务的token
            public CancellationTokenSource cts;
            public CancellationToken ct;

            public AsyncTask(
                int tid,
                uint delya,
                int count,
                Action<int> taskCallBack,
                Action<int> cancelCallBack)
            {
                this.tid = tid;
                this.delay = delya;
                this.count = count;
                this.taskCallBack = taskCallBack;
                this.cancelCallBack = cancelCallBack;
                this.startTime = DateTime.UtcNow; ;

                this.loopIndex = 0;
                this.fixDelta = 0;

                cts = new CancellationTokenSource();
                ct = cts.Token;
            }

        }
    }
}
