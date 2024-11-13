using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using static RGuang.ExcelKit.ExcelAsset2Texture2DAttribute;

namespace RGuang.ExcelKit.Example
{
    [ExcelAsset2Texture2DAttribute(
    hideFlags: HideFlags.NotEditable,
    assetPath: "Assets/Plugins/RGuang/Scripts/ExcelKit/Example/Excels",
    excelName: "ConfigItems",
    excelSheet: "资源配置",
    fieldStartRow: 2,
    fieldStartColumn: 1,
    enableLog: true
    )]
    public class SO_ExcelAssets : ScriptableObject
    {
        public List<MyAssetsInfo> DataLst;



        [ContextMenu("Set_NotEditable")]
        private void Set_NotEditable()
        {
            this.hideFlags = HideFlags.NotEditable;
        }
        [ContextMenu("Set_Editable")]
        private void Set_Editable()
        {
            this.hideFlags = HideFlags.None;
        }



    }


    [System.Serializable]
    public class MyAssetsInfo
    {
        public string Id;
        [AssetNameWithExtension]
        public string Name;
        [AssetDir]
        public string DirPath;
        [Texture2DAsset]
        public Texture2D Texture2DAsset;
        [SpriteAsset]
        public Sprite SpriteAsset;
        [PrefabAsset]
        public GameObject PrefabAsset;

    }





}
