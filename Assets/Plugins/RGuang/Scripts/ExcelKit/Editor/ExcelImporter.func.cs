#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.SS.UserModel;
using UnityEditor;
using UnityEngine;
using static RGuang.ExcelKit.ExcelAsset2AssetsAttribute;

namespace RGuang.ExcelKit
{
    public partial class ExcelImporter
    {

        static void ImportExcel(IWorkbook book, ExcelAssetInfo info)
        {
            string assetName = info.AssetType.Name + ".asset";
            string assetPath = Path.Combine(info.Attribute.AssetPath, assetName);

            UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);
            asset.hideFlags = info.Attribute.HideFlags;

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

            if (info.Attribute is ExcelAsset2AssetsAttribute)
            {
                AddReference_Assets(asset);
            }

            if (info.Attribute.LogOnImport)
            {
                Debug.Log($"从 [{info.Attribute.AssetPath}/{info.Attribute.ExcelName}] 表中 [{info.Attribute.ExcelSheetName}]页 成功导入数据,数据保存在 [{AssetDatabase.GetAssetPath(asset)}] ");
            }

            EditorUtility.SetDirty(asset);
        }


        // --- Create ScriptableObject
        static UnityEngine.Object LoadOrCreateAsset(string assetPath, Type assetType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

            var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(assetType.Name);
                AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
                //asset.hideFlags = HideFlags.NotEditable;
            }

            ScriptableObject so = asset as ScriptableObject;


            return asset;
        }


        //
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
                if (entryCell != null && entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;
                //if (entryCell != null) continue;

                ICell c1 = row.GetCell(contextIdx);
                // skip NullOrWhiteSpace
                if (c1 == null) continue;
                //if (c1.CellType == CellType.Blank) continue;
                //if (c1.CellType == CellType.String && string.IsNullOrWhiteSpace(c1.StringCellValue)) continue;

                var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);

                listAddMethod.Invoke(list, new object[] { entity });

            }
            return list;
        }


        // -- 读取Excel中的字段名
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


        // -- 创建对象数据
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


        // -- 数据类型
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


        static void AddReference_Assets(UnityEngine.Object obj)
        {

            FieldInfo SOFieldInfo = obj.GetType().GetFields()[0];
            List<object> dataLst = ((IEnumerable)SOFieldInfo.GetValue(obj)).Cast<object>().ToList();
            Type entityType = dataLst[0].GetType();


            FieldInfo fiedDir = null;
            FieldInfo fiedName = null;

            List<FieldInfo> assetFieldLst = new List<FieldInfo>();

            FieldInfo[] fieldInfos = entityType.GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var attr_dir = (AssetDirAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(AssetDirAttribute), false);
                if (attr_dir != null)
                {
                    fiedDir = fieldInfos[i];
                    continue;
                }

                var attr_name = (AssetNameWithExtensionAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(AssetNameWithExtensionAttribute), false);
                if (attr_name != null)
                {
                    fiedName = fieldInfos[i];
                    continue;
                }

                var attr_texture2D = (Texture2DAssetAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(Texture2DAssetAttribute), false);
                if (attr_texture2D != null)
                {
                    assetFieldLst.Add(fieldInfos[i]);
                    continue;
                }
                var attr_sprite = (SpriteAssetAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(SpriteAssetAttribute), false);
                if (attr_sprite != null)
                {
                    assetFieldLst.Add(fieldInfos[i]);
                    continue;
                }
                var attr_spriteAtlas = (SpriteAtlasAssetAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(SpriteAtlasAssetAttribute), false);
                if (attr_spriteAtlas != null)
                {
                    assetFieldLst.Add(fieldInfos[i]);
                    continue;
                }
                var attr_prefab = (PrefabAssetAttribute)Attribute.GetCustomAttribute(fieldInfos[i], typeof(PrefabAssetAttribute), false);
                if (attr_prefab != null)
                {
                    assetFieldLst.Add(fieldInfos[i]);
                    continue;
                }
            }

            if (fiedName == null) return;
            if (assetFieldLst.Count < 1) return;

            for (int i = 0; i < dataLst.Count; i++)
            {
                string dir = (string)fiedDir.GetValue(dataLst[i]);
                string nameWithExtension = (string)fiedName.GetValue(dataLst[i]);

                if (string.IsNullOrEmpty(nameWithExtension)) continue;

                string rootDir = "Assets";
                if (string.IsNullOrWhiteSpace(dir))
                {
                    dir = rootDir;
                }
                if (dir.StartsWith(rootDir) == false)
                {
                    dir = Path.Combine(rootDir, dir);
                }

                for (int j = 0; j < assetFieldLst.Count; j++)
                {
                    FieldInfo field = assetFieldLst[j];
                    var assetObj = AssetDatabase.LoadAssetAtPath(dir + "/" + nameWithExtension, field.FieldType);
                    field.SetValue(dataLst[i], assetObj);
                }
            }



        }



    }


}

#endif


