///****************************************************************************
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang.Kit;
using System;
using RGuang.LogKit;
using Log = RGuang.LogKit.Log;
using ColorLog = RGuang.LogKit.ColorLog;

namespace CardGame
{

    public class LogManager : RGuang.Kit.MonoSingleton<LogManager>
    {
        [Header("启用日志等级"), SerializeField] LoggerLevel m_logLevel = LoggerLevel.Info | LoggerLevel.Warn | LoggerLevel.Error;
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

            RGuang.LogKit.LogConfig cfg = new RGuang.LogKit.LogConfig
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


            //RGuang.ExcelKit.Example.Item item = null;

            //Debug.LogError(item.ToString());
            throw new Exception("测试");


        }




    }

}







