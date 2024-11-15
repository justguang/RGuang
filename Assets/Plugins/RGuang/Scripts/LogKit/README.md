#	Log日志工具
	namespace RGuang.LogKit

### ---		func:	---
	Log.Info
	Log.ColorInfo
	Log.Warn
	Log.Trace
	Log.Error


### ---		LogConfig	---
	日志配置：
	//日志启用等级(可多选)
    logLevel = LoggerLevel.None|Info|Warn|Trace|Error
	
	//日志类型(Unity | Console)
    cfg.loggerType = LoggerType.Unity、LoggerType.Console
	
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
	



### ---		Example For Console	---
	RGuang.LogKit.LogConfig cfg = new RGuang.LogKit.LogConfig
	{
		LogLevel = RGuang.LogKit.LoggerLevel.Info | LoggerLevel.Warn | LoggerLevel.Error,
		LoggerType = LoggerType.Console,

		LogPrefix = "#",
		EnableTime = true,
		EnableThreadID = true,
		LogSeparate = ">>",

		EnableTrace = true,

		EnableSave = true,
	};
	
	
	Log.InitSetting(cfg);
	Log.ColorInfo(ColorLog.Cyan, cfg.ToString());
    Log.Info("hello world");


###	--- 	Example For Unity   ---
    LogConfig cfg = new LogConfig();
    cfg.logLevel = LoggerLevel.Log | LoggerLevel.Warn | LoggerLevel.Error;
    cfg.loggerType = LoggerType.Unity;
    cfg.logPrefix = "#";
    cfg.enableTime = true;
    cfg.enableThreadID = true;
    cfg.logSeparate = ">";
    cfg.enableTrace = false;
    cfg.enableSave = true;

	RGuang.LogKit.Log.InitSetting(cfg);

	RGuang.LogKit.Log.Info($" ↓↓↓RGuang.Info 日志初始化Start↓↓↓ 时间=>{DateTime.Now.ToString("yyyy-MM-dd[HH]")}");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Cyan, cfg.ToString());

	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.White, " -- 测试白色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Gray, "-- 测试灰色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Black, "-- 测试黑色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Red, "-- 测试红色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Green, "-- 测试绿色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Blue, "-- 测试蓝色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Yellow, "-- 测试黄色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Cyan, "-- 测试青色 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.Magenta, "-- 测试洋红色 --");
	RGuang.LogKit.Log.Info("---------------- 分割线 ----------------------------");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkGray, "-- 测试深灰 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkRed, "-- 测试深红 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkGreen, "-- 测试深绿 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkBlue, "-- 测试深蓝 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkYellow, "-- 测试暗黄 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkCyan, "-- 测试暗青 --");
	RGuang.LogKit.Log.ColorInfo(RGuang.LogKit.ColorLog.DarkMagenta, "-- 测试紫 --");
	RGuang.LogKit.Log.Info($" ↑↑↑ RGuang.Info 日志初始化End↑↑↑  时间=>{DateTime.Now.ToString("yyyy-MM-dd[HH]")}");







