using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace RGuang.ExcelKit
{
    public partial class ExcelImporter : AssetPostprocessor
    {
        #region -- ExcelAssetInfo Cached --
        class ExcelAssetInfo
        {
            public Type AssetType { get; set; }
            public ExcelAssetAttribute Attribute { get; set; }
        }


        static List<ExcelAssetInfo> m_cachedExcelAssetInfos = null; // Clear on compile.
        #endregion



        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool imported = false;
            foreach (string path in importedAssets)
            {

                if (Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx")
                {
                    FindExcelAssetInfos(out m_cachedExcelAssetInfos);

                    var excelName = Path.GetFileNameWithoutExtension(path);
                    if (excelName.StartsWith("~$")) continue;

                    List<ExcelAssetInfo> excelAssetLst = m_cachedExcelAssetInfos.FindAll(i => i.Attribute.ExcelName.Equals(excelName));

                    IWorkbook book = LoadBook(path);
                    for (int i = 0; i < excelAssetLst.Count; i++)
                    {
                        ImportExcel(book, excelAssetLst[i]);
                    }

                    book.Close();
                    imported = true;
                }
            }

            if (imported)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        // -- 遍历获取所有 ExcelAssetInfo
        static void FindExcelAssetInfos(out List<ExcelAssetInfo> excelAssetInfoLst)
        {
            excelAssetInfoLst = new List<ExcelAssetInfo>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var excelAssetAttributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
                    if (excelAssetAttributes.Length > 0)
                    {
                        var attribute = (ExcelAssetAttribute)excelAssetAttributes[0];
                        if (string.IsNullOrWhiteSpace(attribute.AssetPath) == false
                            && string.IsNullOrWhiteSpace(attribute.ExcelName) == false
                            && string.IsNullOrWhiteSpace(attribute.ExcelSheetName) == false)
                        {
                            excelAssetInfoLst.Add(new ExcelAssetInfo { AssetType = type, Attribute = attribute });
                        }
                    }

                    var excelAsset2Texture2DAttributes = type.GetCustomAttributes(typeof(ExcelAsset2AssetsAttribute), false);
                    if (excelAsset2Texture2DAttributes.Length > 0)
                    {
                        var attribute = (ExcelAssetAttribute)excelAsset2Texture2DAttributes[0];
                        if (string.IsNullOrWhiteSpace(attribute.AssetPath) == false
                            && string.IsNullOrWhiteSpace(attribute.ExcelName) == false
                            && string.IsNullOrWhiteSpace(attribute.ExcelSheetName) == false)
                        {
                            excelAssetInfoLst.Add(new ExcelAssetInfo { AssetType = type, Attribute = attribute });
                        }
                    }

                }
            }
        }





        /// <summary>
        /// FileOpen读取Excel
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        static IWorkbook LoadBook(string excelPath)
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
                else return new XSSFWorkbook(stream);
            }
        }








    }

}




