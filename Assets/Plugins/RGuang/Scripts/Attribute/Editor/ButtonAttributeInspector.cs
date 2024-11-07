using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RGuang.Attribute.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class ButtonAttributeInspector : UnityEditor.Editor
    {

        private MethodInfo[] Methods => target.GetType()
            .GetMethods(BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.NonPublic |
                        BindingFlags.Public);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawMethods();
        }

        private void DrawMethods()
        {
            if (Methods.Length < 1)
                return;

            foreach (var method in Methods)
            {
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                if (attr != null)
                    DrawButton(method, attr);
            }

        }

        public void DrawButton(MethodInfo method, ButtonAttribute attr)
        {
            string btnName = attr.ButtonName;
            float topSpace = attr.TopSpace;
            if (string.IsNullOrEmpty(btnName))
            {
                btnName = method.Name;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(topSpace);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginHorizontal();

            bool btnOK = GUILayout.Button(btnName);

            if (btnOK)
                method.Invoke(target, null);

            EditorGUILayout.EndHorizontal();


        }
    }
}


