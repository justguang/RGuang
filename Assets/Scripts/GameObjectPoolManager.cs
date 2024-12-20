using System;
using System.Collections.Generic;
using UnityEngine;
using RGuang.Kit;
using RGuang.Attribute;
using GameObjectPool = RGuang.Kit.GameObjectPool;

namespace RGuang.IUtility
{
    public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
    {
        [Header("配置池子"), SerializeField] OptionalInspector<GameObjectPool[]> ConfigPool = new OptionalInspector<GameObjectPool[]>();
        [Header("未配置的池子默认容量"), SerializeField, Range(RGuang.Kit.GameObjectPool.MinCapacity, RGuang.Kit.GameObjectPool.MaxCapacity)] private int m_defaultPoolCapacity = RGuang.Kit.GameObjectPool.DefaultCapacity;
        [Header("检测池子容量信息"), SerializeField, ReadWriteInspector] private bool m_checkPoolCount;

        /**
         *  所有对象池
         */
        private readonly List<RGuang.Kit.GameObjectPool> m_pools = new List<GameObjectPool>(RGuang.Kit.GameObjectPool.DefaultCapacity);

        #region Unity Function

        private void Awake()
        {
            _instance = this;

#if UNITY_EDITOR
            m_checkPoolCount = true;
#endif

            if (ConfigPool.Value != null)
            {
                for (int i = 0; i < ConfigPool.Value.Length; i++)
                {
                    var data = ConfigPool.Value[i];
                    CreatePool(data.Prefab, data.ConfigCapacity, data.PoolName);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < m_pools.Count; i++)
            {
                var data = m_pools[i];
                if (m_checkPoolCount)
                {
                    Debug.LogWarning($" >>> [{data.PoolName}] 池子数据：\n" +
                        $"池子配置容量[{data.ConfigCapacity}]\n" +
                        $"已创建的对象数量[{data.CreatedCount}]\n" +
                        $"正在使用的对象数量[{data.UseingCount}]\n" +
                        $"闲置的对象数量[{data.IdleCount}]\n" +
                        $"溢出数量[{data.OverflowCount}]。");
                }
                data.Dispose();
            }
            m_pools.Clear();

        }
        #endregion

        RGuang.Kit.GameObjectPool CreatePool(GameObject obj, int capacity, string poolName = null)
        {
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

            GameObject parentRoot = new GameObject($"{poolName}");
            parentRoot.transform.SetParent(this.transform);
            var pool = new RGuang.Kit.GameObjectPool(obj, parentRoot.transform, poolName, capacity);
            m_pools.Add(pool);
            return pool;
        }

        #region --- Pool Getter/Setter ---
        public RGuang.Kit.GameObjectPool GetPool(GameObject prefab)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, m_defaultPoolCapacity);

            return pool;
        }
        #endregion


        #region --- Spawn Obj ---

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="parent">重置父级</param>
        public GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, m_defaultPoolCapacity);
            return pool?.Spawn(parent);
        }
        public GameObject Spawn(string poolName, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(parent);
            }
            return obj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="pos">该对象要重复的位置</param>
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, m_defaultPoolCapacity);
            return pool?.Spawn(pos, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, parent);
            }
            return obj;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="prefab">对象的预制体</param>
        /// <param name="pos">该对象要重复的位置</param>
        /// <param name="rot">该对象要重复的旋转</param>
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, m_defaultPoolCapacity);
            return pool?.Spawn(pos, rot, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, rot, parent);
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
        /// <param name="parent">该对象要重复的父级</param>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Vector3 localScale, Transform parent = null)
        {
            var pool = m_pools.Find(p => p.Prefab.Equals(prefab));
            if (pool == null) pool = CreatePool(prefab, m_defaultPoolCapacity);
            return pool?.Spawn(pos, rot, localScale, parent);
        }
        public GameObject Spawn(string poolName, Vector3 pos, Quaternion rot, Vector3 localScale, Transform parent = null)
        {
            GameObject obj = null;
            var pool = m_pools.Find(p => p.PoolName.Equals(poolName));
            if (pool != null)
            {
                obj = pool.Spawn(pos, rot, localScale, parent);
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
}


