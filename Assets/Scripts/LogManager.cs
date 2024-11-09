///****************************************************************************
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang.Kit;
using System;
using UnityEngine.Assertions;

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
        Application.logMessageReceived += LogKit.OnUnityLogReceived;

        LogKit.Log($" ↓↓↓RGuang.LogKit 日志初始化Start↓↓↓ 时间=>{DateTime.Now.ToString("yyyy-MM-dd[HH]")}");
        LogKit.ColorLog(ColorLog.Cyan, cfg.ToString());

        LogKit.ColorLog(ColorLog.White, " -- 测试白色 --");
        LogKit.ColorLog(ColorLog.Gray, "-- 测试灰色 --");
        LogKit.ColorLog(ColorLog.Black, "-- 测试黑色 --");
        LogKit.ColorLog(ColorLog.Red, "-- 测试红色 --");
        LogKit.ColorLog(ColorLog.Green, "-- 测试绿色 --");
        LogKit.ColorLog(ColorLog.Blue, "-- 测试蓝色 --");
        LogKit.ColorLog(ColorLog.Yellow, "-- 测试黄色 --");
        LogKit.ColorLog(ColorLog.Cyan, "-- 测试青色 --");
        LogKit.ColorLog(ColorLog.Magenta, "-- 测试洋红色 --");
        LogKit.Log("---------------- 分割线 ----------------------------");
        LogKit.ColorLog(ColorLog.DarkGray, "-- 测试深灰 --");
        LogKit.ColorLog(ColorLog.DarkRed, "-- 测试深红 --");
        LogKit.ColorLog(ColorLog.DarkGreen, "-- 测试深绿 --");
        LogKit.ColorLog(ColorLog.DarkBlue, "-- 测试深蓝 --");
        LogKit.ColorLog(ColorLog.DarkYellow, "-- 测试暗黄 --");
        LogKit.ColorLog(ColorLog.DarkCyan, "-- 测试暗青 --");
        LogKit.ColorLog(ColorLog.DarkMagenta, "-- 测试紫 --");
        LogKit.Log("---------------- 分割线 ----------------------------");
        LogKit.Warn("-- RGuang.Kit Warn测试 --");
        LogKit.Error("-- RGuang.Kit Error测试--");
        LogKit.Trace("-- RGuang.Kit Trace测试--");
        LogKit.Log($" ↑↑↑ RGuang.LogKit 日志初始化End↑↑↑  时间=>{DateTime.Now.ToString("yyyy-MM-dd[HH]")}");

        Debug.Log($"Unity Log2");
        Debug.LogWarning($"Unity LogWarning2");
        UnityEngine.Debug.LogError($"Unity LogErrorg2");

        RGuang.ExcelKit.Example.Item item = null;
        item.Id += 1;

        throw new Exception(" Exce测试");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnityEngine.Application.logMessageReceived -= LogKit.OnUnityLogReceived;
    }


}


