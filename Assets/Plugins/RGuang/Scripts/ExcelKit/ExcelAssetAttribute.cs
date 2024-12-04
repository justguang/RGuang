using System;
using UnityEngine;

namespace RGuang.ExcelKit
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExcelAssetAttribute : Attribute
    {
        /// <summary>
        /// Excel表名
        /// </summary>
        public string ExcelName { get; set; }
        /// <summary>
        /// 读取Excel表下的指定页里的数据
        /// </summary>
        public string ExcelSheetName { get; set; }
        /// <summary>
        /// 保存数据的路径，如果不设置就则默认同Excel表同一路径下
        /// </summary>
        public string SaveDataPath { get; set; }
        /// <summary>
        /// 字段在第n行开始
        /// </summary>
        public int FieldStartRow { get; set; }
        /// <summary>
        /// 字段在第n列开始
        /// </summary>
        public int FieldStartColumn { get; set; }
        /// <summary>
        /// HideFlags
        /// </summary>
        public HideFlags HideFlags { get; set; }
        /// <summary>
        /// True开启日志
        /// </summary>
        public bool LogOnImport { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelName">Excel表名</param>
        /// <param name="excelSheet">读取Excel表下指定的页数据</param>
        /// <param name="saveDataPath">转存的数据资源保存的路径【Assets/】,如果不设置就则默认同Excel表同一路径下</param>
        /// <param name="fieldStartRow">字段在第n行开始</param>
        /// <param name="fieldStartColumn">字段在第n列开始</param>
        /// <param name="enableLog">True开启日志</param>
        public ExcelAssetAttribute(string excelName, string excelSheet, string saveDataPath = null, int fieldStartRow = 0, int fieldStartColumn = 1, HideFlags hideFlags = HideFlags.NotEditable, bool enableLog = false)
        {
            this.ExcelName = excelName;
            this.ExcelSheetName = excelSheet;
            if (string.IsNullOrEmpty(saveDataPath) == false)
            {
                this.SaveDataPath = saveDataPath.Replace("\\", "/");
            }
            this.FieldStartRow = fieldStartRow;
            this.FieldStartColumn = fieldStartColumn;
            this.HideFlags = hideFlags;
            this.LogOnImport = enableLog;
        }



    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExcelAsset2AssetsAttribute : ExcelAssetAttribute
    {
        public ExcelAsset2AssetsAttribute(string excelName, string excelSheet, string saveDataPath = null, int fieldStartRow = 0, int fieldStartColumn = 1, HideFlags hideFlags = HideFlags.NotEditable, bool enableLog = false)
            : base(excelName, excelSheet, saveDataPath, fieldStartRow, fieldStartColumn, hideFlags, enableLog)
        {
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class AssetDirAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class AssetNameWithExtensionAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class Texture2DAssetAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class SpriteAssetAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class SpriteAtlasAssetAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class PrefabAssetAttribute : Attribute { }


    }



}

