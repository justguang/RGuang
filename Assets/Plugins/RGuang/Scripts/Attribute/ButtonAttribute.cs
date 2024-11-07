using System;
using UnityEngine;

namespace RGuang.Attribute
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string ButtonName { get; set; }
        public float TopSpace { get; set; }

        public ButtonAttribute(string buttonName = null, float topSpace = 0.0f)
        {
            ButtonName = buttonName;
            TopSpace = topSpace;
        }

    }

}

