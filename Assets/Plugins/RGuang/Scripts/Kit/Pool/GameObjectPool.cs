#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;
using NPOI.SS.Formula.Functions;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace RGuang.Kit
{
    [DisallowMultipleComponent]
    public sealed class GameObjectPool : MonoBehaviour
    {
        private GameObjectPool() { }

        #region --- CONST ---
        /// <summary>
        /// 默认容量
        /// </summary>
        public const int DefaultCapacity = 10;
        /// <summary>
        /// 允许最小容量
        /// </summary>
        public const int MinCapacity = 2;
        /// <summary>
        /// 允许最大容量
        /// </summary>
        public const int MaxCapacity = 500;
        /// <summary>
        /// 默认释放闲置资源间隔【秒】
        /// </summary>
        public const int DefaultReleaseIdleInterval = 180;
        /// <summary>
        /// 最小释放闲置资源间隔【秒】
        /// </summary>
        public const int MinReleaseIdleInterval = 30;
        /// <summary>
        /// 最大释放闲置资源间隔【秒】
        /// </summary>
        public const int MaxReleaseIdleInterval = 3600;
        #endregion

        #region ---  Properties  ---
        [SerializeField] private string m_poolName;
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private Transform m_parentRoot;
        [SerializeField] private int m_capacity = DefaultCapacity;
        [SerializeField] private int m_releaseIdleInterval = DefaultReleaseIdleInterval;
        private int m_createdCount;
        private int m_overflowCount;
        private int m_usingCount;
        private long m_createdPoolTime;
        private long m_lastCreatedObjTime;
        private long m_lastSpawnObjTime;
        private long m_lastUnSpawnObjTime;
        private Queue<GameObject> m_pool = new Queue<GameObject>(DefaultCapacity);

        private bool m_initialized;
        #endregion

        #region ---  Getter/Setter  ---
        /// <summary>
        /// 对象池名称【唯一】
        /// </summary>
        public string PoolName
        {
            get => m_poolName;
            private set => m_poolName = value;
        }
        /// <summary>
        /// 目标对象
        /// </summary>
        public GameObject Prefab
        {
            get => m_prefab;
            private set => m_prefab = value;
        }
        /// <summary>
        /// 默认父级
        /// </summary>
        public Transform ParentRoot
        {
            get => m_parentRoot;
            private set => m_parentRoot = value;
        }
        /// <summary>
        /// 配置容量
        /// </summary>
        public int ConfigCapacity
        {
            get => m_capacity;
            private set
            {
                m_capacity = Mathf.Clamp(value, MinCapacity, MaxCapacity);
                m_pool = new Queue<GameObject>(m_capacity);
            }
        }
        /// <summary>
        /// 释放闲置资源间隔【秒】
        /// </summary>
        public int ReleaseIdleInterval
        {
            get => m_releaseIdleInterval;
            private set
            {
                m_releaseIdleInterval = Mathf.Clamp(value, MinReleaseIdleInterval, MaxReleaseIdleInterval);
            }
        }
        /// <summary>
        /// 已创建对象的数量
        /// </summary>
        public int CreatedCount
        {
            get => m_createdCount;
            private set => m_createdCount = value;
        }
        /// <summary>
        /// 超出配置容量的对象数量
        /// </summary>
        public int OverflowCount
        {
            get => m_overflowCount;
            private set => m_overflowCount = value;
        }
        /// <summary>
        /// 正在使用的对象数量
        /// </summary>
        public int UseingCount
        {
            get => m_usingCount;
            private set => m_usingCount = value;
        }
        /// <summary>
        /// 闲置对象数量
        /// </summary>
        public int IdleCount
        {
            get => m_pool.Count;
        }

        /// <summary>
        /// 对象池创建时间
        /// </summary>
        public long CreatedPoolTime
        {
            get => m_createdPoolTime;
            private set => m_createdPoolTime = value;
        }
        /// <summary>
        /// 上一次创建对象时间
        /// </summary>
        public long LastCreatedObjTime
        {
            get => m_lastCreatedObjTime;
            private set => m_lastCreatedObjTime = value;
        }
        /// <summary>
        /// 最新取用对象时间
        /// </summary>
        public long LastSpawnObjTime
        {
            get => m_lastSpawnObjTime;
            private set => m_lastSpawnObjTime = value;
        }
        /// <summary>
        /// 上一次回收对象时间
        /// </summary>
        public long LastUnSpawnObjTime
        {
            get => m_lastUnSpawnObjTime;
            private set => m_lastUnSpawnObjTime = value;
        }
        /// <summary>
        /// True => 已初始化
        /// </summary>
        public bool Initialized
        {
            get => m_initialized;
            private set => m_initialized = value;
        }
        #endregion

        #region --- Private Function ---
        GameObject CreateObj()
        {
            CreatedCount++;
            LastCreatedObjTime = DateTime.UtcNow.Ticks;
            var copy = GameObject.Instantiate(Prefab, ParentRoot);
            copy.SetActive(false);
            return copy;
        }
        GameObject AvailableObject(bool active = true, Transform parent = null)
        {
            UseingCount++;
            LastSpawnObjTime = DateTime.UtcNow.Ticks;
            GameObject availableObject = null;
            if (m_pool.Count > 0 && !m_pool.Peek().activeSelf)
            {
                availableObject = m_pool.Dequeue();
            }
            else
            {
                availableObject = CreateObj();
            }

            if (availableObject.activeSelf != active) availableObject.SetActive(active);
            if (parent)
            {
                availableObject.transform.SetParent(parent);
            }

            return availableObject;
        }
        #endregion

        #region --- Init/Dispose ---
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab">对象预制体</param>
        /// <param name="parentRoot">创建对象的根节点</param>
        /// <param name="poolName">对象池名</param>
        /// <param name="poolCapacity">对象池容量</param>
        /// <param name="releaseIdleInterval">释放闲置资源间隔【秒】</param>
        /// <returns></returns>
        public GameObjectPool InitPool(
            GameObject prefab,
            Transform parentRoot,
            string poolName = null,
            int poolCapacity = DefaultCapacity,
            int releaseIdleInterval = DefaultReleaseIdleInterval)
        {
            if (Initialized)
            {
                Debug.LogError($"重复Init！[{PoolName}]对象池已初始化。");
                return this;
            }

            Prefab = prefab;
            ParentRoot = parentRoot;
            if (string.IsNullOrWhiteSpace(poolName)) PoolName = Prefab.name;
            else PoolName = poolName;

            ConfigCapacity = poolCapacity;
            ReleaseIdleInterval = releaseIdleInterval;
            CreatedPoolTime = DateTime.UtcNow.Ticks;

            Initialized = true;
            return this;
        }

        /// <summary>
        /// 释放闲置资源
        /// </summary>
        public void ClearIdle()
        {
            var senc = (DateTime.UtcNow - new DateTime(LastSpawnObjTime)).TotalSeconds;
            if (senc < ReleaseIdleInterval) return;
            LastSpawnObjTime = DateTime.UtcNow.Ticks;

            if (m_pool.Count < 1) return;
            var arr = m_pool.ToArray();
            m_pool.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                CreatedCount--;
                GameObject.Destroy(arr[i]);
            }
        }

        public void DisposePool()
        {
            if (Initialized)
            {
                Initialized = false;
                if (m_pool.Count > 0)
                {
                    var arr = m_pool.ToArray();
                    m_pool.Clear();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        CreatedCount--;
                        GameObject.Destroy(arr[i]);
                    }
                }

                UseingCount = 0;
                CreatedCount = 0;
                OverflowCount = 0;
            }
            else
            {
                Debug.LogError($"对象池重复Dispose！[{PoolName}]对象池已释放。");
            }
        }
        #endregion

        #region --- Spawn ---
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="active">该对象要重置的激活状态</param>
        /// <param name="parent">该对象要重置的父级</param>
        public GameObject Spawn(bool active = true, Transform parent = null)
        {
            GameObject obj = AvailableObject(active, parent);
            return obj;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="pos">该对象要重置的位置</param>
        /// <param name="active">该对象要重置的激活状态</param>
        /// <param name="parent">该对象要重置的父级</param>
        public GameObject Spawn(Vector3 pos, bool active = true, Transform parent = null)
        {
            GameObject obj = AvailableObject(active, parent);
            obj.transform.position = pos;
            return obj;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="pos">该对象要重置的位置</param>
        /// <param name="rot">该对象要重置的旋转</param>
        /// <param name="active">该对象要重置的激活状态</param>
        /// <param name="parent">该对象要重置的父级</param>
        public GameObject Spawn(Vector3 pos, Quaternion rot, bool active = true, Transform parent = null)
        {
            GameObject obj = AvailableObject(active, parent);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            return obj;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="pos">该对象要重置的位置</param>
        /// <param name="rot">该对象要重置的旋转</param>
        /// <param name="localScale">该对象要重置的缩放比例</param>
        /// <param name="active">该对象要重置的激活状态</param>
        /// <param name="parent">该对象要重置的父级</param>
        public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 localScale, bool active = true, Transform parent = null)
        {
            GameObject obj = AvailableObject(active, parent);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.transform.localScale = localScale;
            return obj;
        }

        #endregion

        #region --- UnSpawn ---
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="Exception">可能重复回收</exception>
        public void UnSpawn(GameObject obj)
        {
            LastUnSpawnObjTime = DateTime.UtcNow.Ticks;
            if (obj.activeSelf) obj.SetActive(false);
            obj.transform.SetParent(ParentRoot);

            if (m_pool.Contains(obj))
            {
                throw new Exception($"对象池[{PoolName}] 重复回收对象{obj}");
            }

            if (IdleCount >= ConfigCapacity)
            {
                OverflowCount++;
                GameObject.Destroy(obj);
            }
            else
            {
                m_pool.Enqueue(obj);
            }

        }

        /// <summary>
        /// 检测对象是否可回收
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回True则可回收，返回False则池子已满或者对象已存在池子中</returns>
        public bool CanUnSpawn(GameObject obj)
        {
            return IdleCount < ConfigCapacity && !m_pool.Contains(obj);
        }
        public bool Contains(GameObject obj) => m_pool.Contains(obj);
        #endregion



    }


    /**  -------------------     Editor     ---------------------  */

#if UNITY_EDITOR
    [CustomEditor(typeof(GameObjectPool))]
    internal sealed class GameObjectPoolInspector : UnityEditor.Editor
    {
        #region Properties
        private SerializedProperty m_poolName;
        private SerializedProperty m_prefab;
        private SerializedProperty m_parentRoot;
        private SerializedProperty m_capacity;
        private SerializedProperty m_releaseIdleInterval;
        #endregion

        #region GUILabel
        private readonly GUIContent label_poolName = new GUIContent("池名");
        private readonly GUIContent label_preafb = new GUIContent("预制体");
        private readonly GUIContent label_parentRoot = new GUIContent("默认父级");
        private readonly GUIContent label_capacity = new GUIContent("池容量");
        private readonly GUIContent label_releaseIdleInterval = new GUIContent("释放闲置对象间隔(秒)");

        private readonly GUIStyle style_createdCount = new GUIStyle();
        private readonly GUIStyle style_overflowCount = new GUIStyle();
        private readonly GUIStyle style_usingCount = new GUIStyle();
        private readonly GUIStyle style_IdleCount = new GUIStyle();
        #endregion
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            GameObjectPool t = (GameObjectPool)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_poolName, label_poolName);
                EditorGUILayout.PropertyField(m_prefab, label_preafb);
                EditorGUILayout.PropertyField(m_parentRoot, label_parentRoot);
                m_capacity.intValue = EditorGUILayout.IntSlider(label_capacity, m_capacity.intValue, GameObjectPool.MinCapacity, GameObjectPool.MaxCapacity);
                m_releaseIdleInterval.intValue = EditorGUILayout.IntSlider(label_releaseIdleInterval, m_releaseIdleInterval.intValue, GameObjectPool.MinReleaseIdleInterval, GameObjectPool.MaxReleaseIdleInterval);

                EditorGUILayout.Space(5);
                //style_createdCount.normal.textColor = new Color(0.80f, 0.30f, 0.30f);//Red
                //style_createdCount.normal.textColor = new Color(0.30f, 0.80f, 0.30f);//Green
                //style_createdCount.normal.textColor = new Color(0.30f, 0.30f, 0.80f);//Blue
                //style_createdCount.normal.textColor = new Color(0.60f, 0.60f, 0.0f);//DarkYellow
                //style_createdCount.normal.textColor = new Color(0.0f, 0.60f, 0.60f);//DarkCyan
                style_createdCount.normal.textColor = Color.white;
                style_createdCount.fontSize = 13;
                EditorGUILayout.LabelField($"已创建数量: {t.CreatedCount}", style_createdCount);

                style_overflowCount.normal.textColor = new Color(0.60f, 0.60f, 0.0f);
                style_overflowCount.fontSize = 13;
                EditorGUILayout.LabelField($"溢出数量: {t.OverflowCount}", style_overflowCount);

                style_usingCount.normal.textColor = new Color(0.0f, 0.60f, 0.60f);
                style_usingCount.fontSize = 13;
                EditorGUILayout.LabelField($"正在使用数量: {t.UseingCount}", style_usingCount);

                style_IdleCount.normal.textColor = new Color(0.30f, 0.80f, 0.30f);
                style_IdleCount.fontSize = 13;
                EditorGUILayout.LabelField($"闲置数量: {t.IdleCount}", style_IdleCount);

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField($"对象池创建时间: {new DateTime(t.CreatedPoolTime).ToLocalTime()}");
                EditorGUILayout.LabelField($"上一次创建对象时间: {new DateTime(t.LastCreatedObjTime).ToLocalTime()}");
                EditorGUILayout.LabelField($"上一次取用对象时间: {new DateTime(t.LastSpawnObjTime).ToLocalTime()}");
                EditorGUILayout.LabelField($"上一次回收对象时间: {new DateTime(t.LastUnSpawnObjTime).ToLocalTime()}");
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
            //Repaint();
        }


        private void OnEnable()
        {
            m_poolName = serializedObject.FindProperty("m_poolName");
            m_prefab = serializedObject.FindProperty("m_prefab");
            m_parentRoot = serializedObject.FindProperty("m_parentRoot");
            m_capacity = serializedObject.FindProperty("m_capacity");
            m_releaseIdleInterval = serializedObject.FindProperty("m_releaseIdleInterval");

        }
    }
#endif


#endif

}


