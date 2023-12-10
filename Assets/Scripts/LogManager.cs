///****************************************************************************
/// @author  	：                                                          
/// @date    	：                                                                 
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang;

public class LogManager : QFramework.PersistentMonoSingleton<LogManager>
{
    [Header("启用日志等级"), SerializeField] RLoggerLevel m_logLevel;
    [Header("日志内容显示时间"), SerializeField] bool m_enableTime;
    [Header("日志内容显示线程ID"), SerializeField] bool m_enableThreadID;
    [Header("日志内容保存到文件"), SerializeField] bool m_enableSave;
    [Header("日志文件新内容覆盖原有内容"), SerializeField] bool m_enableSaveCover;
    [Header("日志文件名"), SerializeField] string m_saveFileName = "log.txt";
    [Header("日志文件保存路劲"), SerializeField] string m_saveFilePath = @"D:\workspace\RGuang\Logs\RLog\";

    readonly RLoggerType m_loggerType = RLoggerType.Unity;


    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        if (!System.IO.Directory.Exists(m_saveFilePath)) System.IO.Directory.CreateDirectory(m_saveFilePath);
#else
        string logPath = PathUtils.Path_StreamingAssets + "/RLog/";
        if (!System.IO.Directory.Exists(logPath)) System.IO.Directory.CreateDirectory(logPath);
#endif


        RLogConfig cfg = new RLogConfig
        {
            logLevel = m_logLevel,

            enableTime = m_enableTime,
            enableThreadID = m_enableThreadID,

            saveName = m_saveFileName,
            enableSave = m_enableSave,
            enableCover = m_enableSaveCover,

#if UNITY_EDITOR
            savePath = m_saveFilePath,
#else
                savePath = logPath,
#endif
            loggerType = m_loggerType,
        };

        RLog.InitSetting(cfg);
        RLog.ColorLog(RLogColor.Cyan, "日志初始化完成.");
        RLog.ColorLog(RLogColor.Cyan, $"日志等级{cfg.logLevel}");
        RLog.ColorLog(RLogColor.Cyan, $"日志保存路劲{cfg.savePath}");
        RLog.ColorLog(RLogColor.Cyan, $"日志文件名{cfg.saveName}");
    }

}

