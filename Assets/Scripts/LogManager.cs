///****************************************************************************
/// @author  	：                                                          
/// @date    	：                                                                 
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang.Utils;
public class LogManager : QFramework.PersistentMonoSingleton<LogManager>
{
    [Header("启用日志等级"), SerializeField] ULoggerLevel m_logLevel;
    [Header("日志内容显示时间"), SerializeField] bool m_enableTime;
    [Header("日志内容显示线程ID"), SerializeField] bool m_enableThreadID;
    [Header("日志内容保存到文件"), SerializeField] bool m_enableSave;
    [Header("日志文件新内容覆盖原有内容"), SerializeField] bool m_enableSaveCover;
    [Header("日志文件名"), SerializeField] string m_saveFileName = "log.txt";
    [Header("日志文件保存路劲"), SerializeField] string m_saveFilePath = @"D:\workspace\lcg_a\Logs\ULog\";

    readonly ULoggerType m_loggerType = ULoggerType.Unity;


    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        if (!System.IO.Directory.Exists(m_saveFilePath)) System.IO.Directory.CreateDirectory(m_saveFilePath);
#else
        string logPath = PathUtils.Path_StreamingAssets + "/ULog/";
        if (!System.IO.Directory.Exists(logPath)) System.IO.Directory.CreateDirectory(logPath);
#endif


        ULogConfig cfg = new ULogConfig
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

        ULog.InitSetting(cfg);
        ULog.ColorLog(ULogColor.Cyan, "日志初始化完成.");
        ULog.ColorLog(ULogColor.Cyan, $"日志等级{cfg.logLevel}");
        ULog.ColorLog(ULogColor.Cyan, $"日志保存路劲{cfg.savePath}");
        ULog.ColorLog(ULogColor.Cyan, $"日志文件名{cfg.saveName}");
    }

}

