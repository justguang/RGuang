#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGuang.Kit
{
    [System.Serializable]
    public class GameObjectPool
    {
        public GameObjectPool() { }
        public GameObjectPool(GameObject prefab, Transform parentRoot) => Init(prefab, parentRoot);
        public GameObjectPool(GameObject prefab, Transform parentRoot, string poolName = null, int capacity = DefaultCapacity, int releaseIdleInterval = DefaultReleaseIdleInterval)
            => Init(prefab, parentRoot, poolName, capacity, releaseIdleInterval);

        #region Properties
        [Tooltip("对象池名称"), SerializeField] private string m_poolName;
        [SerializeField] private GameObject m_prefab;
        [Tooltip("配置池容量"), Range(MinCapacity, MaxCapacity), SerializeField] private int m_capacity = DefaultCapacity;
        [Tooltip("释放闲置资源间隔(秒)"), Range(MinReleaseIdleInterval, MaxReleaseIdleInterval), SerializeField] private int m_releaseIdleInterval = DefaultReleaseIdleInterval;
        private bool m_initialized;
        private Queue<GameObject> m_pool = new Queue<GameObject>(DefaultCapacity);

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
        public const int DefaultReleaseIdleInterval = 55;
        /// <summary>
        /// 最小释放闲置资源间隔【秒】
        /// </summary>
        public const int MinReleaseIdleInterval = 30;
        /// <summary>
        /// 最大释放闲置资源间隔【秒】
        /// </summary>
        public const int MaxReleaseIdleInterval = 3600;
        #endregion

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
        /// 目标对象默认父级
        /// </summary>
        public Transform ParentRoot
        {
            get;
            private set;
        }


        #region --- Count ---
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
        /// 闲置对象数量
        /// </summary>
        public int IdleCount => m_pool.Count;
        /// <summary>
        /// 正在使用的对象数量
        /// </summary>
        public int UseingCount
        {
            get;
            private set;
        }
        /// <summary>
        /// 已创建对象的数量
        /// </summary>
        public int CreatedCount
        {
            get;
            private set;
        }
        /// <summary>
        /// 超出配置容量的对象数量
        /// </summary>
        public int OverflowCount
        {
            get;
            private set;
        }
        #endregion

        #region --- DateTime ---
        /// <summary>
        /// 对象池创建时间
        /// </summary>
        public DateTime CreatedTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 上一次创建对象时间
        /// </summary>
        public DateTime LastCreatedObjTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 最新取用对象时间
        /// </summary>
        public DateTime LastSpawnTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 上一次回收对象时间
        /// </summary>
        public DateTime LastUnSpawnTime
        {
            get;
            private set;
        }
        #endregion

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
            LastCreatedObjTime = DateTime.UtcNow;
            var copy = GameObject.Instantiate(Prefab, ParentRoot);
            copy.SetActive(false);
            return copy;
        }
        GameObject AvailableObject(bool active = true, Transform parent = null)
        {
            UseingCount++;
            LastSpawnTime = DateTime.UtcNow;
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
        public GameObjectPool Init(
            GameObject prefab = null,
            Transform parentRoot = null,
            string poolName = null,
            int poolCapacity = DefaultCapacity,
            int releaseIdleInterval = DefaultReleaseIdleInterval)
        {
            if (Initialized)
            {
                Debug.LogError($"重复Init！[{PoolName}]对象池已初始化。");
                return this;
            }

            if (prefab) Prefab = prefab;
            if (parentRoot) ParentRoot = parentRoot;
            if (string.IsNullOrWhiteSpace(poolName)) PoolName = Prefab.name;
            else PoolName = poolName;

            ConfigCapacity = poolCapacity;
            ReleaseIdleInterval = releaseIdleInterval;
            CreatedTime = DateTime.UtcNow;

            Initialized = true;
            return this;
        }

        /// <summary>
        /// 释放闲置资源
        /// </summary>
        public void ClearIdle()
        {
            var senc = (DateTime.UtcNow - LastSpawnTime).TotalSeconds;
            if (senc < ReleaseIdleInterval) return;
            LastSpawnTime = DateTime.UtcNow;

            if (m_pool.Count < 1) return;
            var arr = m_pool.ToArray();
            m_pool.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                CreatedCount--;
                GameObject.Destroy(arr[i]);
            }
        }

        public void Dispose()
        {
            if (Initialized)
            {
                Initialized = false;
                ClearIdle();
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
            LastUnSpawnTime = DateTime.UtcNow;
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

}


#endif



