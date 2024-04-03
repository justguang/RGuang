///****************************************************************************
/// @fileName   ： SkipUnityLogo.cs                                                                
/// @description： 跳过 程序启动时 出现的unity logo                                                                                      
///****************************************************************************
#if !UNITY_EDITOR && SKIP_UNITY_LOGO

using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 跳过 程序启动时 出现的unity logo
/// 
/// </summary>
[UnityEngine.Scripting.Preserve]
public class SkipUnityLogo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
#if UNITY_WEBGL
        Application.focusChanged += Application_focusChanged;
#else
        System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
    }


#if UNITY_WEBGL
    private static void Application_focusChanged(bool obj)
    {
        Application.focusChanged -= Application_focusChanged;
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#else
    private static void AsyncSkip()
    {
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#endif
}

#endif
