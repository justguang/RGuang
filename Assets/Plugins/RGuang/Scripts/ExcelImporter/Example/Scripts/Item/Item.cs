using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RGuang.Kit.Example
{
    [Serializable]
    public class Item
    {
        public int ID;
        public string Name;
        public int Price;
        public GradeType Grade;
        public string Description;

        public override string ToString()
        {
            return $"ID={ID},\n" +
                    $"Name={Name},\n" +
                    $"Price={Price},\n" +
                    $"GradeType={Grade},\n" +
                    $"Description={Description}\n";
        }

    }

    public enum GradeType
    {
        低,
        中,
        高,
    }

}

