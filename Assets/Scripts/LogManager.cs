///****************************************************************************
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang.Kit;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogManager : RGuang.Kit.MonoSingleton<LogManager>
{
    [Header("启用日志等级"), SerializeField] LoggerLevel m_logLevel = LoggerLevel.Log | LoggerLevel.Warn | LoggerLevel.Error;
    [Header("日志内容前缀标记字符"), SerializeField] string m_logPrefix = "#";
    [Header("日志内容前缀标记字符与日志内容间隔符号"), SerializeField] string m_logSeparate = ">>";
    [Header("日志内容显示时间"), SerializeField] bool m_enableTime = true;
    [Header("日志内容显示线程ID"), SerializeField] bool m_enableThreadID = true;
    [Header("日志内容显示堆栈信息"), SerializeField] bool m_enableTrace = false;

    [Header("日志内容保存成日志文件"), SerializeField] bool m_enableSave = false;
    [Header("日志文件保存路劲"), SerializeField] string m_saveFilePath = string.Empty;
    [Header("日志文件名"), SerializeField] string m_saveFileName = string.Empty;

    //日志类型
    readonly LoggerType m_loggerType = LoggerType.Unity;


    public override void OnSingletonInit()
    {
        base.OnSingletonInit();
    }

    private void Awake()
    {

#if UNITY_EDITOR
        if (string.IsNullOrWhiteSpace(m_saveFilePath))
        {
            int index = PathKit.StreamingAssetPath.LastIndexOf("/", PathKit.StreamingAssetPath.Length - 20);
            m_saveFilePath = PathKit.StreamingAssetPath.Substring(0, index) + "/Logs/RLog/";
        }
#else
        if (string.IsNullOrWhiteSpace(m_saveFilePath)) m_saveFilePath = PathKit.StreamingAssetPath + "RLog/";
#endif
        if (!System.IO.Directory.Exists(m_saveFilePath)) System.IO.Directory.CreateDirectory(m_saveFilePath);


        LogConfig cfg = new LogConfig
        {
            LogLevel = m_logLevel,
            LogPrefix = m_logPrefix,
            LogSeparate = m_logSeparate,

            EnableTime = m_enableTime,
            EnableThreadID = m_enableThreadID,

            EnableTrace = m_enableTrace,
            EnableSave = m_enableSave,
            SaveName = m_saveFileName,

            SavePath = m_saveFilePath,
            LoggerType = m_loggerType,
        };

        LogKit.InitSetting(cfg);
        LogKit.ColorLog(ColorLog.Cyan, cfg.ToString());
        LogKit.Log($" test = {DateTime.Now.ToString("yyyy-MM-dd[HH]")}");
    }



}


