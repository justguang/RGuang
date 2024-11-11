///****************************************************************************
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang.Kit;
using System;
using System.Threading;

namespace CardGame
{

    public class LogManager : RGuang.Kit.MonoSingleton<LogManager>
    {
        [Header("启用日志等级"), SerializeField] LoggerLevel m_logLevel = LoggerLevel.Log | LoggerLevel.Warn | LoggerLevel.Error;
        [Header("日志信息前缀 - 头部标记字符"), SerializeField] string m_logPrefix = "#";
        [Header("日志信息前缀 - True显示时间"), SerializeField] bool m_enableTime = true;
        [Header("日志信息前缀 - True显示线程ID"), SerializeField] bool m_enableThreadID = true;
        [Header("日志信息前缀与内容间隔符号"), SerializeField] string m_logSeparate = ">>";
        [Header("True显示堆栈信息"), SerializeField] bool m_enableTrace = false;

        [Header("True保存日志文件"), SerializeField] bool m_enableSave = true;
        [Header("文件路劲"), SerializeField] string m_saveFilePath = string.Empty;
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

            LogConfig cfg = new LogConfig
            {
                LogLevel = m_logLevel,
                LoggerType = m_loggerType,

                LogPrefix = m_logPrefix,
                EnableTime = m_enableTime,
                EnableThreadID = m_enableThreadID,
                LogSeparate = m_logSeparate,

                EnableTrace = m_enableTrace,

                EnableSave = m_enableSave,
                SavePath = m_saveFilePath,
                SaveName = m_saveFileName,

            };


            LogKit.InitSetting(cfg);

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
            LogKit.Log($" ↑↑↑ RGuang.LogKit 日志初始化End↑↑↑  时间=>{DateTime.Now.ToString("yyyy-MM-dd[HH]")}");


            RGuang.ExcelKit.Example.Item item = null;

            //Debug.LogError(item.ToString());
            throw new Exception("测试");


        }




    }

}







