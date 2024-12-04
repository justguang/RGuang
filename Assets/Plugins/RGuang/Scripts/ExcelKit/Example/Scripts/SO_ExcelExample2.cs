using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGuang.ExcelKit.Example
{

    [ExcelAsset(hideFlags: HideFlags.NotEditable,
                saveDataPath: "Assets/Plugins/RGuang/Scripts/ExcelKit/Example/Excels",
                excelName: "ConfigItems",
                excelSheet: "测试",
                fieldStartRow: 2,
                fieldStartColumn: 1,
                enableLog: true)]
    public class SO_ExcelExample2 : ScriptableObject
    {
        public List<Item> DataLst;
    }

}
