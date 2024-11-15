using System;
using UnityEditor;
using UnityEngine;

namespace RGuang.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(ReadWriteInspectorAttribute))]
    public class ReadWriteInspectorAttributeInspector : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var myAttr = attribute as ReadWriteInspectorAttribute;

            switch (myAttr.Mode)
            {
                case Mode.InSpector:
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.enabled = true;
                    break;
                case Mode.InPlayMode:
                    if (EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        GUI.enabled = false;
                        EditorGUI.PropertyField(position, property, label, true);
                        GUI.enabled = true;
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property, label, true);
                    }
                    break;
                default:
                    break;
            }

        }

    }


}
