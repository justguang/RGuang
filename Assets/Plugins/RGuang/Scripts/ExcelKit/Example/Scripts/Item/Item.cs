using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RGuang.Kit.Example
{

    /*
     * 要求public，或者带有[SerializeField]标签的非public字段
     * 
     */

    [Serializable]
    public class Item
    {
        public int ID;
        public string Name;
        public int Price;
        public GradeType Grade;
        public string Description;
        [SerializeField] protected string Nothing2;
        [SerializeField] protected string SP2;
        [SerializeField] protected string OP2;
        [SerializeField] protected string SP7;
        public string SP8;

        public override string ToString()
        {
            return $"ID={ID},\n" +
                    $"Name={Name},\n" +
                    $"Price={Price},\n" +
                    $"GradeType={Grade},\n" +
                    $"Description={Description}\n" +
                    $"Nothing2={Nothing2}\n" +
                    $"SP2={SP2}\n" +
                    $"SP6={OP2}\n" +
                    $"SP7={SP7}\n" +
                    $"SP8={SP8}\n";
        }

    }

    public enum GradeType
    {
        低,
        中,
        高,
    }

}

