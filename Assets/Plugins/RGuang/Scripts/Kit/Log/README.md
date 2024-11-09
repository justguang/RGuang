#	RLog日志工具
	namespace RGuang.Kit

### --- func: ---
	LogKit.Log	
	LogKit.ColorLog
	LogKit.Warn
	LogKit.Trace
	LogKit.Error


### --- RLogConfig ---
	日志配置：
	//日志启用等级(可多选)
    logLevel = LoggerLevel.None|Log|Warn|Trace|Error
	
	//日志类型(Unity | Console)
    cfg.loggerType = RLoggerType.Unity、RLoggerType.Console
	
	//日志信息前缀 - 前缀标记字符
	logPrefix = "#"
	
	//日志信息前缀 - True显示时间
	enableTime = true/false
	
	//日志信息前缀 - True显示线程ID
	enableThreadID = true/false
	
	//日志信息前缀 - 与日志具体内容间隔符号
    logSeparate = ">>" 
	
	//True显示堆栈信息
	enableTrace = true/false
	
	//True保存日志文件
	enableSave = true/false
	
	//保存日志文件路径【Unity在Assets目录的Logs\RLog文件夹下 | Console在应用程序同级目录的RLog文件夹下】
	savePath = "Rlog\"
	
	//日志文件名
	saveName = "RLog.txt"
	



### --- Example For Unity ---
    LogConfig cfg = new LogConfig();
    cfg.logLevel = LoggerLevel.Log|LoggerLevel.Warn;
    cfg.loggerType = LoggerType.Unity;
    cfg.logPrefix = "#";
    cfg.enableTime = true;
    cfg.enableThreadID = true;
    cfg.logSeparate = ">";
    cfg.enableTrace = false;
    cfg.enableSave = false;
    cfg.savePath = null;
    cfg.saveName = null;
	
	LogKit.InitSetting(cfg);
	UnityEngine.Application.logMessageReceived += LogKit.OnUnityLogReceived;
	
    LogKit.Log("hello world");

##### ps: 日志文件同时保存 UnityEngine.Debug 输出的信息需要添加事件监听 如下：
	void Awake() => UnityEngine.Application.logMessageReceived += LogKit.OnUnityLogReceived;
	
	void OnDestroy() => UnityEngine.Application.logMessageReceived -= LogKit.OnUnityLogReceived;



