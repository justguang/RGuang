///****************************************************************************
/// @fileName   ： LogManager.cs                                                                
/// @description： 日志配置管理                                                                                      
///****************************************************************************
using UnityEngine;
using RGuang;
using System.Collections;
using System.Collections.Generic;
using System;
using RGuang.Operation;

public class LogManager : QFramework.PersistentMonoSingleton<LogManager>
{
    [Header("启用日志等级"), SerializeField] RLoggerLevel m_logLevel = RLoggerLevel.Log | RLoggerLevel.Warn | RLoggerLevel.Error;
    [Header("日志内容前缀标记字符"), SerializeField] string m_logPrefix = "#";
    [Header("日志内容前缀标记字符与日志内容间隔符号"), SerializeField] string m_logSeparate = ">>";
    [Header("日志内容显示时间"), SerializeField] bool m_enableTime = true;
    [Header("日志内容显示线程ID"), SerializeField] bool m_enableThreadID = true;
    [Header("日志内容显示堆栈信息"), SerializeField] bool m_enableTrace = false;

    [Header("日志内容保存成日志文件"), SerializeField] bool m_enableSave = false;
    [Header("日志文件新内容覆盖原有内容"), SerializeField] bool m_enableSaveCover = false;
    [Header("日志文件保存路劲"), SerializeField] string m_saveFilePath = string.Empty;
    [Header("日志文件名"), SerializeField] string m_saveFileName = string.Empty;

    readonly RLoggerType m_loggerType = RLoggerType.Unity;




    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        if (string.IsNullOrWhiteSpace(m_saveFilePath))
        {
            int index = RGuang.RPath.StreamingAssetPath.LastIndexOf("/", RGuang.RPath.StreamingAssetPath.Length - 20);
            m_saveFilePath = RGuang.RPath.StreamingAssetPath.Substring(0, index) + "/Logs/RLog/";
        }
#else
        if (string.IsNullOrWhiteSpace(m_saveFilePath)) m_saveFilePath = RGuang.RPath.StreamingAssetPath + "RLog/";
#endif
        if (string.IsNullOrWhiteSpace(m_saveFileName)) m_saveFileName = "log.txt";
        if (!System.IO.Directory.Exists(m_saveFilePath)) System.IO.Directory.CreateDirectory(m_saveFilePath);


        RLogConfig cfg = new RLogConfig
        {
            logLevel = m_logLevel,
            logPrefix = m_logPrefix,
            logSeparate = m_logSeparate,

            enableTime = m_enableTime,
            enableThreadID = m_enableThreadID,

            enableTrace = m_enableTrace,
            enableSave = m_enableSave,
            enableCover = m_enableSaveCover,
            saveName = m_saveFileName,

            savePath = m_saveFilePath,
            loggerType = m_loggerType,
        };

        RLog.InitSetting(cfg);
        RLog.ColorLog(RLogColor.Cyan, "日志初始化完成.");
        RLog.ColorLog(RLogColor.Cyan, $"日志等级{cfg.logLevel}");
        RLog.ColorLog(RLogColor.Cyan, $"日志保存路劲{cfg.savePath}");
        RLog.ColorLog(RLogColor.Cyan, $"日志文件名{cfg.saveName}");
        RLog.ColorLog(RLogColor.Cyan, $"UnityEngine.Application.platform = {UnityEngine.Application.platform}");
    }


    private void Start()
    {
        string _path = @"D:\workspace\Creator\Build\Data_Comm\";
        Dynamic.DynamicLoadCSharpScripts(_path, null, null);
        Test();
        //TestLoadImg();
    }

    #region 测试加载图片
    [SerializeField] private UnityEngine.SpriteRenderer img;
    void TestLoadImg()
    {
        if (img == null) return;
        string _path = @"E:\My\Image\jg2.png";
        var testImg = RGuang.RLoadImg.GetSpriteFromFile(_path);
        img.sprite = testImg;

    }

    #endregion

    void Test()
    {
        float _014 = 0.14f;
        float _015 = 0.15f;
        float _016 = 0.16f;
        float _024 = 0.24f;
        float _025 = 0.25f;
        float _026 = 0.26f;


        float _14 = 1.4f;
        float _15 = 1.5f;
        float _16 = 1.6f;
        float _24 = 2.4f;
        float _25 = 2.5f;
        float _26 = 2.6f;

        RLog.Log("【-0.14】=> " + RMath.Round(_014, 1));
        RLog.Log("【-0.15】=> " + RMath.Round(_015, 1));
        RLog.Log("【-0.16】=> " + RMath.Round(_016, 1));
        RLog.Log("");
        RLog.Log("【-0.24】=> " + RMath.Round(_024, 1));
        RLog.Log("【-0.25】=> " + RMath.Round(_025, 1));
        RLog.Log("【-0.26】=> " + RMath.Round(_026, 1));
        RLog.Log("");

        RLog.Log("【-1.4】=> " + RMath.Round(_14, 0));
        RLog.Log("【-1.5】=> " + RMath.Round(_15, 0));
        RLog.Log("【-1.6】=> " + RMath.Round(_16, 0));
        RLog.Log("");
        RLog.Log("【-2.4】=> " + RMath.Round(_24, 0));
        RLog.Log("【-2.5】=> " + RMath.Round(_25, 0));
        RLog.Log("【-2.6】=> " + RMath.Round(_26, 0));
        RLog.Log("");

        RInt t1 = new RInt(0.120);
        RInt t2 = new RInt(-1.930);
        RInt t3 = new RInt(-0.120);
        RInt t4 = new RInt(1.930);

        RLog.Log($"【0.1】 {t1} RawInt=>" + t1.RawInt + "  RawFloat=>" + t1.RawFloat);
        RLog.Log($"【1.53】 {t2} RawInt=>" + t2.RawInt + "  RawFloat=>" + t2.RawFloat);
        RLog.Log($"【-0.1】 {t3} RawInt=>" + t3.RawInt + "  RawFloat=>" + t3.RawFloat);
        RLog.Log($"【-1.53】{t4} RawInt=>" + t4.RawInt + "  RawFloat=>" + t4.RawFloat);
        RLog.Log("");


        var t1__2 = t1 + t2;
        var t1_2 = t1 - t2;
        var t1x2 = t1 * t2;
        var t1xx2 = t1 / t2;

        RLog.Log($"【0.120 +  -1.530】{t1__2.ScaledValue}  RawInt=>{t1__2.RawInt}  RawFloat=>{t1__2.RawFloat} RawDouble=>{t1__2.RawDouble}");
        RLog.Log($"【0.120 -  -1.530】{t1_2.ScaledValue}  RawInt=>{t1_2.RawInt}  RawFloat=>{t1_2.RawFloat} RawDouble=>{t1_2.RawDouble}");
        RLog.Log($"【0.120 *  -1.530】{t1x2.ScaledValue}  RawInt=>{t1x2.RawInt}  RawFloat=>{t1x2.RawFloat} RawDouble=>{t1x2.RawDouble}");
        RLog.Log($"【0.120 /  -1.530】{t1xx2.ScaledValue}  RawInt=>{t1xx2.RawInt}  RawFloat=>{t1xx2.RawFloat} RawDouble=>{t1xx2.RawDouble}");
        RLog.Log("");
        RLog.Log(" acosTable.count=> " + RAcosTable.Table.Length + " acosTable.IndexCount=> " + RAcosTable.IndexCount);
        RLog.Log("");




        RLog.Log("");




        RLog.Log("");



    }

}


