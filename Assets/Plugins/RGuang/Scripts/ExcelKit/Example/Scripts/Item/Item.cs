using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RGuang.ExcelKit.Example
{


    [Serializable]
    public class Item
    {
        public int Id;
        public string Name;
        public int Price;
        public EnumGradeType Grade;
        public string Description;
        [SerializeField] protected string Nothing2;
        [SerializeField] protected string SP2;
        [SerializeField] protected string OP2;
        [SerializeField] protected string SP7;
        public string SP8;

        public override string ToString()
        {
            return $"Id={Id},\n" +
                    $"Name={Name},\n" +
                    $"Price={Price},\n" +
                    $"EnumGradeType={Grade},\n" +
                    $"Description={Description}\n" +
                    $"Nothing2={Nothing2}\n" +
                    $"SP2={SP2}\n" +
                    $"SP6={OP2}\n" +
                    $"SP7={SP7}\n" +
                    $"SP8={SP8}\n";
        }

    }

    public enum EnumGradeType
    {
        低,
        中,
        高,
    }

}

