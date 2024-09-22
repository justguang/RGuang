using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RGuang.IEffect
{


    [CustomEditor(typeof(InteractableWater))]
    public class InteractableWaterEditor : Editor
    {
        private InteractableWater m_water;
        private void OnEnable()
        {
            m_water = (InteractableWater)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            root.Add(new VisualElement { style = { height = 10 } });

            Button generateMeshButton = new Button(() => m_water.GenerateMesh())
            {
                text = "Generate Mesh"
            };

            root.Add(generateMeshButton);

            Button placeEdgeColliderButton = new Button(() => m_water.ResetEdgeCollider())
            {
                text = "Place Edge Collider "
            };
            root.Add(placeEdgeColliderButton);

            return root;
        }

        private void OnSceneGUI()
        {
            //Draw the wireframe box
            Handles.color = m_water.GizmoColor;
            Vector3 center = m_water.transform.position;
            Vector3 size = new Vector3(m_water.Width, m_water.Height, 0.10f);
            Handles.DrawWireCube(center, size);

            //Handles for width and height
            float handleSize = HandleUtility.GetHandleSize(center) * 0.10f;
            Vector3 snap = Vector3.one * .10f;

            //Corner handles
            Vector3[] corners = new Vector3[4];
            corners[0] = center + new Vector3(-m_water.Width / 2, -m_water.Height / 2, 0.0f);//Bottom-Left
            corners[1] = center + new Vector3(m_water.Width / 2, -m_water.Height / 2, 0.0f);//Bottom-Right
            corners[2] = center + new Vector3(-m_water.Width / 2, m_water.Height / 2, 0.0f);//Top-Left
            corners[3] = center + new Vector3(m_water.Width / 2, m_water.Height / 2, 0.0f);//Top-Right

            //Handle for each corner
            EditorGUI.BeginChangeCheck();
            Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], Quaternion.Euler(Vector3.zero), handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeDimensions(ref m_water.Width, ref m_water.Height, corners[1].x - newBottomLeft.x, corners[3].y - newBottomLeft.y);
                m_water.transform.position += new Vector3((newBottomLeft.x - corners[0].x) / 2, (newBottomLeft.y - corners[0].y) / 2, 0.0f);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], Quaternion.Euler(Vector3.zero), handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeDimensions(ref m_water.Width, ref m_water.Height, newBottomRight.x - corners[0].x, corners[3].y - newBottomRight.y);
                m_water.transform.position += new Vector3((newBottomRight.x - corners[1].x) / 2, (newBottomRight.y - corners[1].y) / 2, 0.0f);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], Quaternion.Euler(Vector3.zero), handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeDimensions(ref m_water.Width, ref m_water.Height, corners[3].x - newTopLeft.x, newTopLeft.y - corners[0].y);
                m_water.transform.position += new Vector3((newTopLeft.x - corners[2].x) / 2, (newTopLeft.y - corners[2].y) / 2, 0.0f);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], Quaternion.Euler(Vector3.zero), handleSize, snap, Handles.CubeHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeDimensions(ref m_water.Width, ref m_water.Height, newTopRight.x - corners[2].x, newTopRight.y - corners[1].y);
                m_water.transform.position += new Vector3((newTopRight.x - corners[3].x) / 2, (newTopRight.y - corners[3].y) / 2, 0.0f);
            }

            //Update the mesh if the handles are moved
            if (GUI.changed)
            {
                m_water.GenerateMesh();
            }

        }

        private void ChangeDimensions(ref float width, ref float height, float calculatedWidthMax, float calculatedHeightMax)
        {
            width = Mathf.Max(0.10f, calculatedWidthMax);
            height = Mathf.Max(0.10f, calculatedHeightMax);

        }



    }
}
