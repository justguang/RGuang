#	RLog日志工具
namespace RGuang

##### function:
	RLog.Log	
	RLog.ColorLog
	RLog.Warn
	RLog.Trace
	RLog.Error


##### RLogConfig
	日志配置：
    logLevel = RLoggerLevel.None/Log/Warn/Trace/Error 【日志启用等级(可多选)】
	logPrefix = "#"【日志信息前缀标记字符】
    logSeparate = ">>" 【前缀标记与日志具体内容间隔符号】
	enableTime = true/false【输出日志时显示/不显示时间】
	enableThreadID = true/false【显示/不显示线程ID】
	enableTrace = true/false【输出日志时显示/不显示堆栈信息】
	enableSave = true/false【保存/不保存日志成日志文件】
	enableCover = true/false【新日志内容覆盖/不覆盖原有日志内容】
    cfg.loggerType = RLoggerType.Unity/RLoggerType.Console 【日志类型(Unity/Console)】
	savePath = "Rlogs\"【保存路径】
	saveName = "RLog.txt"【保存的文件名】
	



##### Example
    RLogConfig cfg = new RLogConfig();
    cfg.logLevel = RLoggerLevel.Log|RLoggerLevel.Warn;
    cfg.logPrefix = "#";
    cfg.logSeparate = ">";
    cfg.enableTime = true;
    cfg.enableThreadID = true;
    cfg.enableTrace = false;
    cfg.enableSave = false;
    cfg.enableCover = true;
    cfg.loggerType = RLoggerType.Unity;
    cfg.savePath = null;
    cfg.saveName = null;
	RLog.InitSetting(cfg);
    RLog.Log("hello world");


