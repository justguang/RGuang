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

namespace RGuang.ExcelKit
{
    public class ExcelImporter : AssetPostprocessor
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

        static List<ExcelAssetInfo> m_cachedInfos = null; // Clear on compile.

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
                    if (m_cachedInfos == null) m_cachedInfos = FindExcelAssetInfos();

                    var excelName = Path.GetFileNameWithoutExtension(path);
                    if (excelName.StartsWith("~$")) continue;

                    List<ExcelAssetInfo> infoLst = m_cachedInfos.FindAll(i => i.ExcelName == excelName);

                    if (infoLst == null || infoLst.Count < 1) continue;


                    IWorkbook book = LoadBook(path);
                    for (int i = 0; i < infoLst.Count; i++)
                    {
                        ImportExcel(book, infoLst[i]);
                    }
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
                    if (string.IsNullOrWhiteSpace(attribute.AssetPath) || string.IsNullOrWhiteSpace(attribute.ExcelName)) continue;

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


        static List<ValueTuple<int, string>> GetFieldNamesFromSheet(ISheet sheet, int fieldStartRow, int fieldStartColumn)
        {
            IRow headerRow = sheet.GetRow(fieldStartRow);

            if (headerRow == null)
            {
                throw new Exception($"读取《{sheet.SheetName}》页的数据字段错误，[{fieldStartRow}]行[{fieldStartColumn}]列无数据字段， 请确认字段开始的行列.");
            }

            var fieldNames = new List<ValueTuple<int, string>>();
            for (int i = fieldStartColumn; i < headerRow.LastCellNum; i++)
            {
                var cell = headerRow.GetCell(i);
                if (cell == null || cell.CellType == CellType.Blank) continue;
                fieldNames.Add((i, cell.StringCellValue));
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
                FieldInfo entityField = entityType.GetField(fieldNames[fieldIdx].Item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (entityField == null) continue;


                //if (!entityField.IsPublic)
                //{
                //    if (entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;
                //}

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


        static object GetEntityListFromSheet(ISheet sheet, Type entityType, ExcelAssetInfo excelAssetInfo)
        {
            List<(int, string)> excelColumnNames = GetFieldNamesFromSheet(sheet, excelAssetInfo.Attribute.FieldStartRow, excelAssetInfo.Attribute.FieldStartColumn);

            Type listType = typeof(List<>).MakeGenericType(entityType);
            MethodInfo listAddMethod = listType.GetMethod("Add", new Type[] { entityType });
            object list = Activator.CreateInstance(listType);

            // row of index 0 is header

            int contextIdx = excelAssetInfo.Attribute.FieldStartColumn;
            for (int i = excelAssetInfo.Attribute.FieldStartRow + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                ICell entryCell = row.GetCell(0);
                // skip comment row
                //if (entryCell != null && entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;
                if (entryCell != null) continue;

                ICell c1 = row.GetCell(contextIdx);
                // skip NullOrWhiteSpace
                if (c1 == null) continue;
                if (c1.CellType == CellType.Blank) continue;
                if (c1.CellType == CellType.String && string.IsNullOrWhiteSpace(c1.StringCellValue)) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
                listAddMethod.Invoke(list, new object[] { entity });

            }
            return list;
        }

        static void ImportExcel(IWorkbook book, ExcelAssetInfo info)
        {
            string assetName = info.AssetType.Name + ".asset";
            string assetPath = Path.Combine(info.Attribute.AssetPath, assetName);

            UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);


            var assetFields = info.AssetType.GetFields();
            if (assetFields == null || assetFields.Length == 0) return;


            ISheet sheet = book.GetSheet(info.Attribute.ExcelSheetName);
            if (sheet == null)
            {
                if (info.Attribute.LogOnImport)
                {
                    Debug.LogWarning($"在[{info.Attribute.ExcelName}]表中没有找到[{info.Attribute.ExcelSheetName}]页的数据.");
                }
                return;
            }

            FieldInfo dataField = assetFields[0];
            Type fieldType = dataField.FieldType;
            if (!fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>)))
            {
                if (info.Attribute.LogOnImport)
                {
                    Debug.LogWarning($"{info.AssetType} 字段中没有指定存储数据的类型=>List<>。");
                }
                return;
            }

            Type[] types = fieldType.GetGenericArguments();
            Type entityType = types[0];

            object entities = GetEntityListFromSheet(sheet, entityType, info);
            dataField.SetValue(asset, entities);

            if (info.Attribute.LogOnImport)
            {
                Debug.Log($"从 [{info.Attribute.AssetPath}/{info.Attribute.ExcelName}] 表中 [{info.Attribute.ExcelSheetName}]页 成功导入数据 .");
            }

            EditorUtility.SetDirty(asset);
        }
    }

}




