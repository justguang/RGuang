#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RGuang.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(ReadWriteInspectorAttribute))]
    public class ReadWriteInspectorAttributeInspector : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var myAttr = attribute as ReadWriteInspectorAttribute;

            bool disable = false;
            if (myAttr.Mode == Mode.InPlayMode && EditorApplication.isPlayingOrWillChangePlaymode) disable = true;
            if (myAttr.Mode == Mode.InSpector) disable = true;

            EditorGUI.BeginDisabledGroup(disable);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();

        }


    }


}

#endif


