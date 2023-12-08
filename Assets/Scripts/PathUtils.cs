using UnityEngine;

public class PathUtils
{
    public const string AB_RESOURCES = "AB_Resources";  // 打包AB包根路径
    public const string CONFIG_FILE = "Config";//配置路劲
    public const string EXCEL_FILE = "Excel";//excel

    /// <summary>
    /// 得到 AB 资源的输入目录
    /// </summary>
    /// <returns></returns>
    public static string Path_ABResources => Application.dataPath + "/" + AB_RESOURCES;

    /// <summary>
    /// 获取配置文件路劲
    /// </summary>
    /// <returns></returns>
    public static string Path_ConfigFile => Application.dataPath + "/" + CONFIG_FILE;

    /// <summary>
    /// 获取Excel文件路劲
    /// </summary>
    /// <returns></returns>
    public static string Path_ExcelFile => Path_StreamingAssets + "/" + EXCEL_FILE;

    /// <summary>
    /// 获得 streamingAssets 输出路径
    ///     1\ 平台(PC/移动端等)路径
    ///     2\ 平台名称
    /// </summary>
    /// <returns></returns>
    public static string Path_StreamingAssets => GetPlatformPath() + "/" + GetPlatformName();


    public static string PersistentDataPath => Application.persistentDataPath;
    public static string StreamingAssetPath => Application.streamingAssetsPath;

    /// <summary>
    /// 获得 AB 包输出路径
    ///     1\ 平台(PC/移动端等)路径
    ///     2\ 平台名称
    /// </summary>
    /// <returns></returns>
    public static string GetABOutPath()
    {
        return GetPlatformPath() + "/" + GetPlatformName();
    }

    /// <summary>
    /// 获得平台路径
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformPath()
    {

        string strReturenPlatformPath = string.Empty;

#if UNITY_STANDALONE_WIN
        strReturenPlatformPath = StreamingAssetPath;

#elif UNITY_IPHONE
                
            strReturenPlatformPath = PersistentDaataPath;
#elif UNITY_ANDROID
 
            strReturenPlatformPath = PersistentDaataPath;
#endif

        return strReturenPlatformPath;
    }

    /// <summary>
    /// 获得平台名称
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformName()
    {
        string strReturenPlatformName = string.Empty;

#if UNITY_STANDALONE_WIN
        strReturenPlatformName = "Windows";

#elif UNITY_IPHONE
                
            strReturenPlatformName = "Iphone";
#elif UNITY_ANDROID
 
            strReturenPlatformName = "Android";
#endif


        return strReturenPlatformName;
    }

    /// <summary>
    /// 返回 WWW 下载 AB 包加载路径
    /// </summary>
    /// <returns></returns>
    public static string GetWWWAssetBundlePath()
    {
        string strReturnWWWPath = string.Empty;

#if UNITY_STANDALONE_WIN
        strReturnWWWPath = "file://" + GetABOutPath();

#elif UNITY_IPHONE
                
            strReturnWWWPath = GetABOutPath() + "/Raw/";
#elif UNITY_ANDROID
 
            strReturnWWWPath = "jar:file://" + GetABOutPath()+"!/assets/";
#endif

        return strReturnWWWPath;
    }

}
