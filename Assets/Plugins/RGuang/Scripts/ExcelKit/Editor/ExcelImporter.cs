
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace RGuang.Kit
{
    public sealed class ExcelImporter : AssetPostprocessor
    {
        class ExcelAssetInfo
        {
            public Type AssetType { get; set; }
            public ExcelAssetAttribute Attribute { get; set; }
            public string ExcelName
            {
                get
                {
                    return string.IsNullOrEmpty(Attribute.ExcelName) ? AssetType.Name : Attribute.ExcelName;
                }
            }
        }

        static List<ExcelAssetInfo> cachedInfos = null; // Clear on compile.


        /// <summary>
        /// 当所有资源 的导入，删除，移动，修改操作Editor都会调用该方法
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool imported = false;
            foreach (string path in importedAssets)
            {
                if (Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx")
                {
                    if (cachedInfos == null) cachedInfos = FindExcelAssetInfos();

                    var excelName = Path.GetFileNameWithoutExtension(path);
                    if (excelName.StartsWith("~$")) continue;

                    ExcelAssetInfo info = cachedInfos.Find(i => i.ExcelName == excelName);

                    if (info == null) continue;

                    ImportExcel(path, info);
                    imported = true;
                }
            }

            if (imported)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        static List<ExcelAssetInfo> FindExcelAssetInfos()
        {
            var list = new List<ExcelAssetInfo>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
                    if (attributes.Length == 0) continue;
                    var attribute = (ExcelAssetAttribute)attributes[0];
                    var info = new ExcelAssetInfo()
                    {
                        AssetType = type,
                        Attribute = attribute
                    };
                    list.Add(info);
                }
            }
            return list;
        }


        static UnityEngine.Object LoadOrCreateAsset(string assetPath, Type assetType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

            var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(assetType.Name);
                AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
                asset.hideFlags = HideFlags.NotEditable;
            }

            return asset;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns>
        /// int => 标识字段在表格中第几纵列
        /// string => 字段名
        /// </returns>
        static List<ValueTuple<int, string>> GetFieldNamesFromSheetHeader(ISheet sheet)
        {
            IRow headerRow = sheet.GetRow(0);

            var fieldNames = new List<ValueTuple<int, string>>();
            for (int i = 1, idx = 1; i < headerRow.LastCellNum; i++, idx++)
            {
                var cell = headerRow.GetCell(i);
                if (cell == null || cell.CellType == CellType.Blank) continue;
                fieldNames.Add((idx, cell.StringCellValue));
            }
            return fieldNames;
        }

        static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
        {
            var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

            switch (type)
            {
                case CellType.String:
                    if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                    else return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
                case CellType.Formula:
                    if (isFormulaEvalute) return null;
                    return CellToFieldObject(cell, fieldInfo, true);
                default:
                    if (fieldInfo.FieldType.IsValueType)
                    {
                        return Activator.CreateInstance(fieldInfo.FieldType);
                    }
                    return null;
            }
        }

        static object CreateEntityFromRow(IRow row, List<ValueTuple<int, string>> fieldNames, Type entityType, string sheetName)
        {
            var entity = Activator.CreateInstance(entityType);

            for (int fieldIdx = 0; fieldIdx < fieldNames.Count; fieldIdx++)
            {
                FieldInfo entityField = entityType.GetField(
                    fieldNames[fieldIdx].Item2,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
                if (entityField == null) continue;
                if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

                ICell cell = row.GetCell(fieldNames[fieldIdx].Item1);
                if (cell == null) continue;

                try
                {
                    object fieldValue = CellToFieldObject(cell, entityField);
                    entityField.SetValue(entity, fieldValue);
                }
                catch
                {
                    throw new Exception($"无效的数据类型. 请确认 【{sheetName}】页 的 {row.RowNum + 1}行,  第{fieldNames[fieldIdx].Item1 + 1}纵列 的数据类型. ");
                }
            }
            return entity;
        }

        static object GetEntityListFromSheet(ISheet sheet, Type entityType)
        {
            var excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

            Type listType = typeof(List<>).MakeGenericType(entityType);
            MethodInfo listAddMethod = listType.GetMethod("Add", new Type[] { entityType });
            object list = Activator.CreateInstance(listType);

            // row of index 0 is header
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                ICell entryCell = row.GetCell(0);
                // skip comment row
                if (entryCell != null && entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

                ICell c1 = row.GetCell(1);
                // skip NullOrWhiteSpace
                if (c1 == null) continue;
                if (c1 != null)
                {
                    if (c1.CellType == CellType.Blank) continue;
                    if (c1.CellType == CellType.String && string.IsNullOrWhiteSpace(c1.StringCellValue)) continue;
                }

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
                listAddMethod.Invoke(list, new object[] { entity });

            }
            return list;
        }

        static void ImportExcel(string excelPath, ExcelAssetInfo info)
        {
            string assetPath = "";
            string assetName = info.AssetType.Name + ".asset";

            if (string.IsNullOrEmpty(info.Attribute.AssetPath))
            {
                string basePath = Path.GetDirectoryName(excelPath);
                assetPath = Path.Combine(basePath, assetName);
            }
            else
            {
                var path = Path.Combine("Assets", info.Attribute.AssetPath);
                assetPath = Path.Combine(path, assetName);
            }
            UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);

            IWorkbook book = LoadBook(excelPath);

            var assetFields = info.AssetType.GetFields();
            int sheetCount = 0;

            foreach (var assetField in assetFields)
            {
                ISheet sheet = book.GetSheet(assetField.Name);
                if (sheet == null) continue;

                Type fieldType = assetField.FieldType;
                if (!fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

                Type[] types = fieldType.GetGenericArguments();
                Type entityType = types[0];

                object entities = GetEntityListFromSheet(sheet, entityType);
                assetField.SetValue(asset, entities);
                sheetCount++;
            }

            if (info.Attribute.LogOnImport)
            {
                Debug.Log($"从【{excelPath}】 成功导入 {sheetCount}个表数据 .");
            }

            EditorUtility.SetDirty(asset);
        }
    }

}


#endif


