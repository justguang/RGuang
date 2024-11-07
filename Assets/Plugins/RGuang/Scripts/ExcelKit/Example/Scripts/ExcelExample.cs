using System;
using System.Collections.Generic;
using RGuang.ExcelKit.Example;
using UnityEngine;
using UnityEngine.UI;

namespace RGuang.Kit.Example
{
    public class ExcelExample : MonoBehaviour
    {
        [SerializeField] private SO_ExcelExample so_data;
        [SerializeField] private SO_ExcelExample2 so_data2;
        void Start()
        {

            if (so_data != null && so_data.DataLst != null)
            {
                so_data.DataLst.ForEach(e => Debug.Log(e.ToString()));
            }

            Debug.LogWarning(" --- 分割线 ---");

            if (so_data2 != null && so_data2.DataLst != null)
            {
                so_data2.DataLst.ForEach(e => Debug.Log(e.ToString()));
            }


        }





    }

}

