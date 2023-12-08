using UnityEngine;
using QFramework;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public interface ISceneLoader
{
    string CurrentSceneName { get; }

}


/// <summary>
/// 场景加载器
/// </summary>
public class SceneLoaderController : MonoBehaviour, ISceneLoader
{


    string ISceneLoader.CurrentSceneName => throw new NotImplementedException();
}

