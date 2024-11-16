using System;
using UnityEngine;

namespace RGuang.Attribute
{
    /// <summary>
    /// 在Inspector面板上的读写模式
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// 在Inspector面板上只读
        /// </summary>
        InSpector,
        /// <summary>
        /// 在EditorMode模式读写， 在PlayMode模式时，只读
        /// </summary>
        InPlayMode,
    }

    /// <summary>
    /// 在Inspector面板上的读写模式
    /// 注意：对集合的增减无效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ReadWriteInspectorAttribute : PropertyAttribute
    {
        /// <summary>
        /// 读写模式
        /// </summary>
        public Mode Mode { get; set; }
        public ReadWriteInspectorAttribute(Mode mode = Mode.InSpector)
        {
            Mode = mode;
        }

    }


}
