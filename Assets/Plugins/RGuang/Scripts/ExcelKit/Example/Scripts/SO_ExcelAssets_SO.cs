using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using RGuang.Kit.Example;
using UnityEditor;
using UnityEngine;
using static RGuang.ExcelKit.ExcelAsset2AssetsAttribute;

namespace RGuang.ExcelKit.Example
{
    [ExcelAsset2AssetsAttribute(
    hideFlags: HideFlags.NotEditable,
    saveDataPath: "Assets/Plugins/RGuang/Scripts/ExcelKit/Example/Excels",
    excelName: "ConfigItems",
    excelSheet: "资源配置_SO",
    fieldStartRow: 2,
    fieldStartColumn: 1,
    enableLog: true
    )]
    public class SO_ExcelAssets_SO : ScriptableObject
    {
        public List<MyAssets_SOInfo> DataLst;



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
    public class MyAssets_SOInfo
    {
        public string Id;
        [AssetNameWithExtension]
        public string Name;
        [AssetDir]
        public string DirPath;
        //[Texture2DAsset]
        //public Texture2D Texture2DAsset;
        //[SpriteAsset]
        //public Sprite SpriteAsset;
        //[PrefabAsset]
        //public GameObject PrefabAsset;
        [AssetReferenceAttribute]
        public ScriptableObject AssetRefernce;

    }





}
