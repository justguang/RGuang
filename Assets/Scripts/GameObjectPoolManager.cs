using System;
using System.Collections.Generic;
using UnityEngine;
using RGuang.Kit;
using RGuang.Attribute;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CardGame.IUtility
{
    [System.Serializable]
    public sealed class PoolableInfo
    {
        public string PoolName;
        public GameObject Prefab;
        [Range(GameObjectPool.MinCapacity, GameObjectPool.MaxCapacity)] public int Capacity = GameObjectPool.DefaultCapacity;
        [Range(GameObjectPool.MinReleaseIdleInterval, GameObjectPool.MaxReleaseIdleInterval)] public int ReleaseIdleInterval = GameObjectPool.DefaultReleaseIdleInterval;
    }

    [DisallowMultipleComponent]
    public sealed class GameObjectPoolManager : RGuang.Kit.MonoSingleton<GameObjectPoolManager>
    {
        /**  ---  默认池容量  ---  */
        [SerializeField] private int m_defaultPoolCapacity = GameObjectPool.DefaultCapacity;
        /**  ---  检测池溢出  ---  */
        [SerializeField] private bool m_checkPoolOverflow;
        /**  ---  自动释放池中闲置对象信息  --- */
        [SerializeField] private float m_releaseInactiveTimer = 0;
        [SerializeField] private bool m_enableReleaseIdle = false;
        [SerializeField] private int m_releaseIdleInterval = GameObjectPool.DefaultReleaseIdleInterval;


        [SerializeField] OptionalInspector<PoolableInfo[]> PoolableInfo = new OptionalInspector<PoolableInfo[]>();

        /**
         *  所有对象池
         */
        [SerializeField] private List<GameObjectPool> m_pools = new List<GameObjectPool>(GameObjectPool.DefaultCapacity);

        #region Unity Function

        private void Awake()
        {
            _instance = this;
#if UNITY_EDITOR
            m_checkPoolOverflow = true;
#endif

            if (PoolableInfo.Value != null)
            {
                for (int i = 0; i < PoolableInfo.Value.Length; i++)
                {
                    var data = PoolableInfo.Value[i];
                    if (data.Prefab == null) continue;
                    CreatePool(data.Prefab, null, data.PoolName, data.Capacity, data.ReleaseIdleInterval);
                }
            }
        }

        private void Update()
        {
            if (m_enableReleaseIdle)
            {
                m_releaseInactiveTimer += Time.deltaTime;
                if (m_releaseInactiveTimer >= m_releaseIdleInterval)
                {
                    m_releaseInactiveTimer -= m_releaseIdleInterval;
                    for (int i = 0; i < m_pools.Count; i++)
                    {
                        m_pools[i].ClearIdle();
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < m_pools.Count; i++)
            {
                var data = m_pools[i];
                if (m_checkPoolOverflow)
                {
                    Debug.LogWarning($" >>> [{data.PoolName}] 池子数据：\n" +
                        $"池子配置容量[{data.ConfigCapacity}]\n" +
                        $"已创建的对象数量[{data.CreatedCount}]\n" +
                        $"正在使用的对象数量[{data.UseingCount}]\n" +
                        $"闲置的对象数量[{data.IdleCount}]\n" +
                        $"溢出数量[{data.OverflowCount}]。");
                }
                data.DisposePool();
            }
            m_pools.Clear();

        }
        #endregion

        GameObjectPool CreatePool(GameObject obj, Transform parentRoot = null, string poolName = null, int capacity = GameObjectPool.DefaultCapacity, int releaseIdleInterval = GameObjectPool.DefaultReleaseIdleInterval)
        {
            if (obj == null) return null;
            if (string.IsNullOrWhiteSpace(poolName)) poolName = obj.name;
            var existPool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (existPool != null)
            {
                Debug.LogError($"对象池创建失败！已存在同名池={poolName}");
                return null;
            }

            existPool = m_pools.Find(p => p.Prefab.Equals(obj));
            if (existPool != null)
            {
                Debug.LogError($"对象池创建失败！已存在同类型池={obj}");
                return existPool;
            }

            if (parentRoot == null)
            {
                parentRoot = new GameObject($"{poolName}").transform;
                parentRoot.SetParent(this.transform);
            }

            GameObjectPool pool = null;
            if (parentRoot.TryGetComponent<GameObjectPool>(out pool) == false)
            {
                pool = parentRoot.gameObject.AddComponent<GameObjectPool>();
            }

            m_pools.Add(pool);
            pool.InitPool(obj, parentRoot, poolName, capacity, releaseIdleInterval);
            return pool;
        }

        #region --- Pool Getter/Setter ---
        public GameObjectPool GetPool(GameObject prefab)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, capacity: m_defaultPoolCapacity);

            return pool;
        }
        public GameObjectPool GetPool(string poolName)
        {
            return m_pools.Find(p => p.PoolName.Equals(poolName));
        }
        #endregion


        #region --- Spawn Obj ---

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="active">重置激活状态</param>
        /// <param name="parent">重置父级</param>
        public GameObject Spawn(GameObject prefab, bool active = true, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, capacity: m_defaultPoolCapacity);
            return pool?.Spawn(active, parent);
        }
        public GameObject Spawn(string poolName, bool active, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(active, parent);
            }
            return obj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="pos">该对象要重复的位置</param>
        /// <param name="active">重置激活状态</param>
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, bool active = true, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, capacity: m_defaultPoolCapacity);
            return pool?.Spawn(pos, active, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, bool active = true, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, active, parent);
            }
            return obj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="pos">该对象要重复的位置</param>
        /// <param name="rot">该对象要重复的旋转</param>
        /// <param name="active">重置激活状态</param>
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, bool active = true, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, capacity: m_defaultPoolCapacity);
            return pool?.Spawn(pos, rot, active, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, Quaternion rot, bool active = true, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, rot, active, parent);
            }
            return obj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="pos">该对象要重复的位置</param>
        /// <param name="rot">该对象要重复的旋转</param>
        /// <param name="localScale">该对象要重复的缩放比例</param>
        /// <param name="active">重置激活状态</param>
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Vector3 localScale, bool active = true, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, capacity: m_defaultPoolCapacity);
            return pool?.Spawn(pos, rot, localScale, active, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, Quaternion rot, Vector3 localScale, bool active = true, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, rot, localScale, active, parent);
            }
            return obj;
        }

        #endregion


        #region --- UnSpawn Obj ---
        public void UnSpawn(GameObject prefab, GameObject obj)
        {
            obj.SetActive(false);
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));

            if (pool != null)
            {
                pool.UnSpawn(obj);
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }
        public void UnSpawn(string poolName, GameObject obj)
        {
            obj.SetActive(false);
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                pool.UnSpawn(obj);
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }
        #endregion

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GameObjectPoolManager))]
    internal sealed class GameObjectPoolManagerInspector : UnityEditor.Editor
    {
        #region Properties
        private SerializedProperty m_defaultPoolCapacity;
        private SerializedProperty m_checkPoolOverflow;
        private SerializedProperty m_enableReleaseIdle;
        private SerializedProperty m_releaseIdleInterval;
        private SerializedProperty m_releaseInactiveTimer;
        private SerializedProperty m_PoolableInfo;
        private SerializedProperty m_pools;
        #endregion

        #region GUILabel
        private readonly GUIContent label_defaultPoolCapacity = new GUIContent("默认池容量");
        private readonly GUIContent label_checkPoolOverflow = new GUIContent("检测池溢出");
        private readonly GUIContent label_enableReleaseIdle = new GUIContent("启用自动释放池中闲置对象");
        private readonly GUIContent label_releaseIdleInterval = new GUIContent("自动释放池中闲置对象周期(秒)");
        private readonly GUIContent label_PoolableInfo = new GUIContent("配置可池化信息");
        private readonly GUIContent label_pools = new GUIContent("所有已创建对象池");
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_defaultPoolCapacity.intValue = EditorGUILayout.IntSlider(label_defaultPoolCapacity, m_defaultPoolCapacity.intValue, GameObjectPool.MinCapacity, GameObjectPool.MaxCapacity);
                m_checkPoolOverflow.boolValue = EditorGUILayout.Toggle(label_checkPoolOverflow, m_checkPoolOverflow.boolValue);
                m_enableReleaseIdle.boolValue = EditorGUILayout.Toggle(label_enableReleaseIdle, m_enableReleaseIdle.boolValue);
                if (m_enableReleaseIdle.boolValue == true)
                {
                    m_releaseIdleInterval.intValue = EditorGUILayout.IntSlider(label_releaseIdleInterval, m_releaseIdleInterval.intValue, GameObjectPool.MinReleaseIdleInterval, GameObjectPool.MaxReleaseIdleInterval);
                    EditorGUILayout.LabelField("自动释放池中闲置对象计时(秒)", m_releaseInactiveTimer.floatValue.ToString("f0"));
                }


                EditorGUILayout.PropertyField(m_PoolableInfo, label_PoolableInfo);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(true);
            { EditorGUILayout.PropertyField(m_pools, label_pools); }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_defaultPoolCapacity = serializedObject.FindProperty("m_defaultPoolCapacity");
            m_checkPoolOverflow = serializedObject.FindProperty("m_checkPoolOverflow");
            m_enableReleaseIdle = serializedObject.FindProperty("m_enableReleaseIdle");
            m_releaseIdleInterval = serializedObject.FindProperty("m_releaseIdleInterval");
            m_releaseInactiveTimer = serializedObject.FindProperty("m_releaseInactiveTimer");
            m_PoolableInfo = serializedObject.FindProperty("PoolableInfo");
            m_pools = serializedObject.FindProperty("m_pools");
        }

    }
#endif

}

