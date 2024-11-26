using System;
using UnityEngine;

namespace RGuang.Attribute
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        /// <summary>
        /// 按钮名
        /// </summary>
        public string ButtonName { get; set; }
        /// <summary>
        /// 按钮上间距
        /// </summary>
        public float TopSpace { get; set; }
        /// <summary>
        /// 按钮下间距
        /// </summary>
        public float BottomSpace { get; set; }

        /// <summary>
        /// 给指定函数功能添加buton按钮在Inspector
        /// </summary>
        /// <param name="buttonName">button明</param>
        /// <param name="topSpace">button顶部间距</param>
        /// <param name="bottomSpace">button底部间距</param>
        public ButtonAttribute(string buttonName = null, float topSpace = 0.0f, float bottomSpace = 0.0f)
        {
            ButtonName = buttonName;
            TopSpace = topSpace;
            BottomSpace = bottomSpace;
        }

    }


}

