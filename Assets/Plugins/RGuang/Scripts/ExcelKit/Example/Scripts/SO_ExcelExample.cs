using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGuang.ExcelKit.Example
{

    [ExcelAsset(assetPath: "Assets/Plugins/RGuang/Scripts/ExcelKit/Example/Excels", excelName: "ConfigItems", excelSheet: "ItemLst_1", fieldStartRow: 1, fieldStartColumn: 1, enableLog: true)]
    public class SO_ExcelExample : ScriptableObject
    {
        public List<Item> DataLst;
    }

}
