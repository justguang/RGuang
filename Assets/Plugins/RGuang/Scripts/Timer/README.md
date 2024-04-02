# UTimer 任务定时器工具
namespace RGuang

使用ConcurrentDictionary管理所有定时任务

### 【UTimer】基类:
    AddTask：添加一个定时任务

    DelTask：删除一个定时任务
    
    Reset：重置定时器


### 【TickTimer】毫秒级的精确定时器，后台线程轮询执行任务列表

### 【AsyncTimer】使用Async await语法驱动，运行在线程池中

### 【FrameTimer】使用外部帧循环驱动，并在帧循环中回调的帧定时器


