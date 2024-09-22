using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RGuang.IEffect
{
    /// <summary>
    /// 可交互2D水效果
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
    [RequireComponent(typeof(WaterTriggerHandler))]
    public class InteractableWater : MonoBehaviour
    {
        [Header("Springs")]
        [SerializeField] private float m_spriteConstant = 1.40f;
        [SerializeField] private float m_damping = 1.10f;
        [SerializeField] private float m_spread = 6.50f;
        [SerializeField, Range(1, 10)] private int m_wavePropogationIterations = 8;
        [SerializeField, Range(0.0f, 20.0f)] private float m_speedMult = 5.50f;

        [Header("Force")]
        public float ForceMultiplier = 0.20f;
        [Range(1.0f, 50.0f)] public float MaxForce = 5.0f;

        [Header("Collision")]
        [SerializeField, Range(1.0f, 10.0f)] private float m_playerCollisionRadiusMult = 4.150f;


        [Header(" Mesh Generation ")]
        [Range(2, 500)] public int NumOfXVertices = 70;
        public float Width = 10.0f;
        public float Height = 4.0f;
        public Material WaterMaterial;
        private const int NUM_OF_Y_VERTICES = 2;

        [Header("Gizmo")]
        public Color GizmoColor = Color.white;

        private Mesh m_mesh;
        private MeshRenderer m_meshRenderer;
        private MeshFilter m_meshFilter;
        private Vector3[] m_vertices;
        private int[] m_topVerticesIndex;

        private EdgeCollider2D m_coll;

        private class WaterPoint
        {
            public float velocity, acceleration, pos, targetHeight;
        }
        private List<WaterPoint> m_waterPointLst = new List<WaterPoint>();

        private void Start()
        {
            m_coll = GetComponent<EdgeCollider2D>();

            GenerateMesh();
            CreateWaterPoints();
        }

        private void Reset()
        {
            m_coll = GetComponent<EdgeCollider2D>();
            m_coll.isTrigger = true;
        }

        private void FixedUpdate()
        {
            return;
            //update all spring postions
            for (int i = 1; i < m_waterPointLst.Count - 1; i++)
            {
                WaterPoint point = m_waterPointLst[i];

                float x = point.pos - point.targetHeight;
                float acceleration = m_spriteConstant * x - m_damping * point.velocity;
                point.pos += point.velocity * m_speedMult * Time.fixedDeltaTime;
                m_vertices[m_topVerticesIndex[i]].y = point.pos;
                point.velocity += acceleration * m_speedMult * Time.fixedDeltaTime;
            }

            //wave propogation
            for (int j = 0; j < m_wavePropogationIterations; j++)
            {
                for (int i = 1; i < m_waterPointLst.Count - 1; i++)
                {
                    float leftDelta = m_spread * (m_waterPointLst[i].pos - m_waterPointLst[i - 1].pos) * m_speedMult * Time.fixedDeltaTime;
                    m_waterPointLst[i - 1].velocity += leftDelta;

                    float rightDelta = m_spread * (m_waterPointLst[i].pos - m_waterPointLst[i + 1].pos) * m_speedMult * Time.fixedDeltaTime;
                    m_waterPointLst[i + 1].velocity += rightDelta;
                }
            }

            //update the mesh
            m_mesh.vertices = m_vertices;
        }


        public void Splash(Collider2D collision,float force)
        {
            float radius = collision.bounds.extents.x * m_playerCollisionRadiusMult;
            Vector2 center = collision.transform.position;
            for (int i = 0; i < m_waterPointLst.Count; i++)
            {
                Vector2 vertexWorldPos = transform.TransformPoint(m_vertices[m_topVerticesIndex[i]]);
                if (IsPointInsideCircle(vertexWorldPos, center, radius))
                {
                    m_waterPointLst[i].velocity = force;
                }
            }
        }

        private bool IsPointInsideCircle(Vector2 point,Vector2 center,float radius)
        {
            float distanceSquared = (point - center).sqrMagnitude;
            return distanceSquared <= radius * radius;
        }

        public void ResetEdgeCollider()
        {
            m_coll = GetComponent<EdgeCollider2D>();

            Vector2[] newPoint = new Vector2[2];

            Vector2 firstPoint = new Vector2(m_vertices[m_topVerticesIndex[0]].x, m_vertices[m_topVerticesIndex[0]].y);
            newPoint[0] = firstPoint;

            Vector2 secondPoint = new Vector2(m_vertices[m_topVerticesIndex[m_topVerticesIndex.Length - 1]].x, m_vertices[m_topVerticesIndex[m_topVerticesIndex.Length - 1]].y);
            newPoint[1] = secondPoint;

            m_coll.offset = Vector2.zero;
            m_coll.points = newPoint;
        }

        public void GenerateMesh()
        {
            m_mesh = new Mesh();

            // 顶点
            m_vertices = new Vector3[NumOfXVertices * NUM_OF_Y_VERTICES];
            m_topVerticesIndex = new int[NumOfXVertices];
            for (int y = 0; y < NUM_OF_Y_VERTICES; y++)
            {
                for (int x = 0; x < NumOfXVertices; x++)
                {
                    float xPos = (x / (float)(NumOfXVertices - 1)) * Width - Width / 2;
                    float yPos = (y / (float)(NUM_OF_Y_VERTICES - 1)) * Height - Height / 2;
                    m_vertices[y * NumOfXVertices + x] = new Vector3(xPos, yPos, 0.0f);

                    if (y == NUM_OF_Y_VERTICES - 1)
                    {
                        m_topVerticesIndex[x] = y * NUM_OF_Y_VERTICES + x;
                    }
                }
            }


            // 三角面
            int[] triangles = new int[(NumOfXVertices - 1) * (NUM_OF_Y_VERTICES - 1) * 6];
            int index = 0;
            for (int y = 0; y < NUM_OF_Y_VERTICES - 1; y++)
            {
                for (int x = 0; x < NumOfXVertices - 1; x++)
                {
                    int bottomLeft = y * NumOfXVertices + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + NumOfXVertices;
                    int topRight = topLeft + 1;

                    //1 tri
                    triangles[index++] = bottomLeft;
                    triangles[index++] = topLeft;
                    triangles[index++] = bottomRight;

                    //2 tri
                    triangles[index++] = bottomRight;
                    triangles[index++] = topLeft;
                    triangles[index++] = topRight;

                }
            }


            //UVs
            Vector2[] uvs = new Vector2[m_vertices.Length];
            for (int i = 0; i < m_vertices.Length; i++)
            {
                uvs[i] = new Vector2((m_vertices[i].x + Width / 2) / Width, (m_vertices[i].y + Height / 2) / Height);
            }

            if (m_meshRenderer == null)
                m_meshRenderer = GetComponent<MeshRenderer>();

            if (m_meshFilter == null)
                m_meshFilter = GetComponent<MeshFilter>();

            m_meshRenderer.material = WaterMaterial;

            m_mesh.vertices = m_vertices;
            m_mesh.triangles = triangles;
            m_mesh.uv = uvs;

            m_mesh.RecalculateNormals();
            m_mesh.RecalculateBounds();
            m_meshFilter.mesh = m_mesh;
        }

        private void CreateWaterPoints()
        {
            m_waterPointLst.Clear();
            for (int i = 0; i < m_topVerticesIndex.Length; i++)
            {
                m_waterPointLst.Add(new WaterPoint
                {
                    pos = m_vertices[m_topVerticesIndex[i]].y,
                    targetHeight = m_vertices[m_topVerticesIndex[i]].y,
                });
            }
        }

    }





}
