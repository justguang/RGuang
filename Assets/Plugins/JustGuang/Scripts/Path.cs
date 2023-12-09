using UnityEngine;

namespace JustGuang
{
    /// <summary>
    /// 路径类
    /// 
    /// </summary>
    public sealed class Path
    {

        /// <summary>
        ///  streamingAssets 输出路径
        ///     1\ 平台(PC/移动端等)路径
        ///     2\ 平台名称
        /// </summary>
        /// <returns></returns>
        public static string StreamingAssetsAndPlatform => GetPlatformPath() + "/" + GetPlatformName();

        /// <summary>
        /// 持久化数据路径
        /// </summary>
        public static string PersistentDataPath => Application.persistentDataPath;
        /// <summary>
        /// streamingAsset路径
        /// </summary>
        public static string StreamingAssetPath => Application.streamingAssetsPath;

        /// <summary>
        /// 获得 AB 包输出路径
        ///     1\ 平台(PC/移动端等)路径
        ///     2\ 平台名称
        /// </summary>
        /// <returns></returns>
        public static string GetABOutPath() => GetPlatformPath() + "/" + GetPlatformName();

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

}
