using System;
using UnityEngine;

namespace RGuang.ExcelKit
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExcelAssetAttribute : Attribute
    {

        /// <summary>
        /// Excel资源路径
        /// </summary>
        public string AssetPath { get; set; }
        /// <summary>
        /// Excel表名
        /// </summary>
        public string ExcelName { get; set; }
        /// <summary>
        /// 读取Excel表下的指定页里的数据
        /// </summary>
        public string ExcelSheetName { get; set; }
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
        /// <param name="assetPath">资源路径【Assets/】</param>
        /// <param name="excelName">Excel表名</param>
        /// <param name="excelSheet">读取Excel表下指定的页数据</param>
        /// <param name="fieldStartRow">字段在第n行开始</param>
        /// <param name="fieldStartColumn">字段在第n列开始</param>
        /// <param name="enableLog">True开启日志</param>
        public ExcelAssetAttribute(string assetPath, string excelName, string excelSheet, int fieldStartRow = 0, int fieldStartColumn = 1, HideFlags hideFlags = HideFlags.NotEditable, bool enableLog = false)
        {
            this.AssetPath = assetPath;
            this.ExcelName = excelName;
            this.ExcelSheetName = excelSheet;
            this.FieldStartRow = fieldStartRow;
            this.FieldStartColumn = fieldStartColumn;
            this.HideFlags = hideFlags;
            this.LogOnImport = enableLog;
        }



    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExcelAsset2AssetsAttribute : ExcelAssetAttribute
    {
        public ExcelAsset2AssetsAttribute(string assetPath, string excelName, string excelSheet, int fieldStartRow = 0, int fieldStartColumn = 1, HideFlags hideFlags = HideFlags.NotEditable, bool enableLog = false)
            : base(assetPath, excelName, excelSheet, fieldStartRow, fieldStartColumn, hideFlags, enableLog)
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
        public class PrefabAssetAttribute : Attribute { }


    }



}

