#if UNITY_2017_1_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGuang.Kit
{
    [System.Serializable]
    public class GameObjectPool
    {
        [SerializeField] private GameObject m_prefab;
        [Tooltip("配置池容量"), Range(1, 500), SerializeField] private int m_capacity = 5;
        private Queue<GameObject> m_pool;

        /// <summary>
        /// 目标对象
        /// </summary>
        public GameObject Prefab => m_prefab;
        /// <summary>
        /// 目标对象默认父级
        /// </summary>
        public Transform ObjParent
        {
            get;
            private set;
        }
        /// <summary>
        /// 配置容量
        /// </summary>
        public int ConfigCapacity
        {
            get => m_capacity;
            private set => m_capacity = value;
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

        #region --- Private Function ---
        GameObject CreateObj()
        {
            CreatedCount++;
            var copy = GameObject.Instantiate(m_prefab, ObjParent);
            copy.SetActive(false);
            return copy;
        }
        GameObject AvailableObject()
        {
            UseingCount++;
            GameObject availableObject = null;
            if (m_pool.Count > 0 && !m_pool.Peek().activeSelf)
            {
                availableObject = m_pool.Dequeue();
            }
            else
            {
                availableObject = CreateObj();
            }

            return availableObject;
        }
        #endregion

        #region --- Init/Dispose ---
        public void Init(Transform parent, int poolCapacity)
        {
            ConfigCapacity = poolCapacity;
            Init(parent);
        }
        public void Init(Transform parent)
        {
            ObjParent = parent;
            Init();
        }
        public void Init()
        {
            m_pool = new Queue<GameObject>(ConfigCapacity);
            for (int i = 0; i < ConfigCapacity; i++)
            {
                m_pool.Enqueue(CreateObj());
            }
        }
        public void Dispose()
        {
            while (m_pool.TryDequeue(out var obj))
            {
                GameObject.Destroy(obj);
            }

            m_pool.Clear();
            UseingCount = 0;
            CreatedCount = 0;
            OverflowCount = 0;
        }
        #endregion

        #region --- Spawn ---
        /// <summary>
        /// 获取一个对象
        /// </summary>
        public GameObject Spawn(Transform parent = null)
        {
            GameObject obj = AvailableObject();
            obj.SetActive(true);
            if (parent)
            {
                obj.transform.SetParent(parent);
            }
            return obj;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="pos">该对象要重置的位置</param>
        public GameObject Spawn(Vector3 pos, Transform parent = null)
        {
            GameObject obj = AvailableObject();
            obj.SetActive(true);
            if (parent)
            {
                obj.transform.SetParent(parent);
            }
            obj.transform.position = pos;
            return obj;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <param name="pos">该对象要重置的位置</param>
        /// <param name="rot">该对象要重置的旋转</param>
        public GameObject Spawn(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            GameObject obj = AvailableObject();
            obj.SetActive(true);
            if (parent)
            {
                obj.transform.SetParent(parent);
            }
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
        public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 localScale, Transform parent = null)
        {
            GameObject obj = AvailableObject();
            obj.SetActive(true);
            if (parent)
            {
                obj.transform.SetParent(parent);
            }
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.transform.localScale = localScale;
            return obj;
        }

        #endregion


        #region --- UnSpawn ---
        public void UnSpawn(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(ObjParent);

            if (m_pool.Contains(obj))
            {
                throw new Exception($"重复回收对象{obj}");
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
        #endregion


    }

}


#endif



