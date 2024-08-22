#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace RGuang.Kit
{

    public class ExcelAssetScriptMenu
    {
        const string ScriptTemplateName = "ExcelAssetScriptTemplete.cs.txt";
        const string FieldTemplete = "\t//public List<ItemType> #FIELDNAME#; // 将 'ItemType' 替换为可序列化的自定义类.";

        [MenuItem("Assets/Create/RGuang/Excel转存Script脚本数据", priority = 2, validate = false)]
        static void CreateScript()
        {
            string savePath = EditorUtility.SaveFolderPanel("选择Excel转存的Script脚本文件保存路径", Application.dataPath, "");
            if (savePath == "") return;

            var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

            string excelPath = AssetDatabase.GetAssetPath(selectedAssets[0]);
            string excelName = Path.GetFileNameWithoutExtension(excelPath);
            List<string> sheetNames = GetSheetNames(excelPath);

            string scriptString = BuildScriptString(excelName, sheetNames);

            string path = Path.ChangeExtension(Path.Combine(savePath, excelName), "cs");
            File.WriteAllText(path, scriptString, Encoding.UTF8);

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/RGuang/Excel转存Script脚本数据", priority = 2, validate = true)]
        static bool CreateScriptValidation()
        {
            var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            if (selectedAssets.Length != 1) return false;
            var path = AssetDatabase.GetAssetPath(selectedAssets[0]);
            return Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx";
        }

        static List<string> GetSheetNames(string excelPath)
        {
            var sheetNames = new List<string>();
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(excelPath) == ".xls") book = new HSSFWorkbook(stream);
                else book = new XSSFWorkbook(stream);

                for (int i = 0; i < book.NumberOfSheets; i++)
                {
                    var sheet = book.GetSheetAt(i);
                    sheetNames.Add(sheet.SheetName);
                }
            }
            return sheetNames;
        }

        static string GetScriptTempleteString()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] filePath = Directory.GetFiles(currentDirectory, ScriptTemplateName, SearchOption.AllDirectories);
            if (filePath.Length == 0) throw new Exception($"脚本模板【{ScriptTemplateName}】未找到.");

            string templateString = File.ReadAllText(filePath[0]);
            return templateString;
        }

        static string BuildScriptString(string excelName, List<string> sheetNames)
        {
            string scriptString = GetScriptTempleteString();

            scriptString = scriptString.Replace("#ASSETSCRIPTNAME#", excelName);

            foreach (string sheetName in sheetNames)
            {
                string fieldString = String.Copy(FieldTemplete);
                fieldString = fieldString.Replace("#FIELDNAME#", sheetName);
                fieldString += "\n#ENTITYFIELDS#";
                scriptString = scriptString.Replace("#ENTITYFIELDS#", fieldString);
            }
            scriptString = scriptString.Replace("#ENTITYFIELDS#\n", "");

            return scriptString;
        }
    }

}

#endif

