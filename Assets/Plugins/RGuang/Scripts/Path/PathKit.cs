using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RGuang
{
    /// <summary>
    /// 路径类
    /// 
    /// </summary>
    public static class PathKit
    {

        /// <summary>
        /// 持久化数据路径【外部目录】
        /// </summary>
        private static string _persistentDataPath;
        public static string PersistentDataPath
        {
            get
            {
                if (_persistentDataPath == null) _persistentDataPath = Application.persistentDataPath + "/";
                return _persistentDataPath;
            }
        }

        /// <summary>
        /// streamingAsset路径【内部目录】
        /// </summary>
        private static string _streamingAssetPath;
        public static string StreamingAssetPath
        {
            get
            {
                if (_streamingAssetPath == null)
                {

#if UNITY_IPHONE && !UNITY_EDITOR
					_streamingAssetPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID && !UNITY_EDITOR
					_streamingAssetPath = Application.streamingAssetsPath + "/";
#elif (UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                    _streamingAssetPath = Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
					_streamingAssetPath = Application.streamingAssetsPath + "/";
#else
                    _streamingAssetPath = Application.streamingAssetsPath + "/";
#endif
                }
                return _streamingAssetPath;
            }
        }


        /// <summary>
        /// 获取平台名
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetPlatformForAssetBundles(RuntimePlatform platform, Action<string> getError = null)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                    return "Editor";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return "WSAPlayer";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    getError?.Invoke($"未处理的运行平台：{platform}");
                    return null;
            }
        }


        /// <summary>
        /// 获得 AB 包输出路径
        ///     1\ 平台(PC/移动端等)路径
        ///     2\ 平台名称
        /// </summary>
        /// <returns></returns>
        public static string GetABOutPath() => StreamingAssetPath + GetPlatformForAssetBundles(UnityEngine.Application.platform);
        /// <summary>
        /// 获得 AB 包输出路径
        /// 【StreamingAssetPath/Windows】
        /// </summary>
        /// <returns></returns>
        public static string GetABOutPath2Windows() => StreamingAssetPath + GetPlatformForAssetBundles(RuntimePlatform.WindowsPlayer);

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
