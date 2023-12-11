using System;
using System.Collections.Generic;
using UnityEngine;

#region Unity GameObject Pool

public class GameObjectPool : MonoBehaviour
{
    #region Data

    private GameObject m_prefab;
    private int m_configSize;
    private Queue<GameObject> m_pool;

    #endregion

    #region Public Function

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="prefab">要池化的对象</param>
    /// <param name="size">配置数量，默认50</param>
    public GameObjectPool InitPool(GameObject prefab, int size = 50)
    {
        if (!prefab) return null;
        this.m_prefab = prefab;
        this.m_configSize = size > 0 ? size : 50;
        m_pool = new Queue<GameObject>(m_configSize);
        for (int i = 0; i < m_configSize; i++)
        {
            m_pool.Enqueue(CopyByInstantiate());
        }

        return this;
    }

    public void DisposePool()
    {
        while (m_pool.TryDequeue(out var obj))
        {
            Destroy(obj);
        }

        m_pool.Clear();
    }

    /// <summary>
    /// 获取池实际大小
    /// </summary>
    public int Count => m_pool.Count;

    /// <summary>
    /// 获取配置大小
    /// </summary>
    public int ConfiCount => m_configSize;


    public GameObject Get()
    {
        GameObject obj = AvailableObject();
        obj.SetActive(true);
        return obj;
    }

    public GameObject Get(Vector3 pos)
    {
        GameObject obj = AvailableObject();
        obj.SetActive(true);
        obj.transform.position = pos;
        return obj;
    }

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        GameObject obj = AvailableObject();
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        return obj;
    }

    public GameObject Get(Vector3 pos, Quaternion rot, Vector3 localScale)
    {
        GameObject obj = AvailableObject();
        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.transform.localScale = localScale;
        return obj;
    }

    public GameObject Get(Transform parent, bool initTransform = true)
    {
        GameObject obj = AvailableObject();
        obj.SetActive(true);
        obj.transform.SetParent(parent);
        if (initTransform)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }

        return obj;
    }

    public void Release(GameObject obj)
    {
        if (obj.activeSelf) obj.SetActive(false);
        m_pool.Enqueue(obj);
    }

    #endregion

    #region Private Function

    GameObject CopyByInstantiate()
    {
        GameObject obj = GameObject.Instantiate(m_prefab, transform);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        return obj;
    }

    GameObject AvailableObject()
    {
        GameObject _availableObject = null;
        if (m_pool.Count > 0)
        {
            _availableObject = m_pool.Dequeue();
        }
        else
        {
            _availableObject = CopyByInstantiate();
        }

        return _availableObject;
    }

    #endregion
}

#endregion


#region ----- Class型对象池 ------ Start

public class ClassObjectPool : IDisposable
{
    readonly object m_lock = "mLock";

    #region 构造

    public ClassObjectPool()
    {
        _maxSize = 70;
        _typeNum = 70;
        _pool = new Dictionary<int, Queue<object>>(_typeNum);
        mUnUsedCount = new Dictionary<Type, int>(_typeNum);
    }

    /// <param name="typeNum">对象种类数量</param>
    /// <param name="poolMaxSize">每个对象池最大容量</param>
    public ClassObjectPool(uint typeNum, uint poolMaxSize)
    {
        _maxSize = (int)poolMaxSize;
        _typeNum = (int)typeNum;
        _pool = new Dictionary<int, Queue<object>>(_typeNum);
        mUnUsedCount = new Dictionary<Type, int>(_typeNum);
    }

    #endregion

    #region 数据

    int _typeNum = 100; //对象种类数量

    /// <summary>
    /// 获取池内对象种类数量
    /// </summary>
    public int mTypeNum => _typeNum;

    int _maxSize = 100; //对象池最大容量

    /// <summary>
    /// 获取池最大容量
    /// </summary>
    public int mPoolMaxSize => _maxSize;

    /// <summary>
    /// key => 类型哈希码
    /// </summary>
    private readonly Dictionary<int, Queue<object>> _pool;

    /// <summary>
    /// 显示所有对象池限制对象
    /// </summary>
    public readonly Dictionary<Type, int> mUnUsedCount; //闲置对象

    /// <summary>
    /// 获取指定类型的对象池内对象数量，如果对象池不存在返回-1
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <returns></returns>
    public int GetUnUsedCount<T>()
    {
        var tp = typeof(T);
        if (mUnUsedCount.ContainsKey(tp))
        {
            return mUnUsedCount[tp];
        }

        return -1;
    }

    #endregion


    /// <summary>
    /// 创建新对象池
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="key">对象类型的HashCode作为key</param>
    /// <returns>返回true则创建成功</returns>
    bool CreateObjectPool<T>(int key) where T : class, new()
    {
        bool result = false;
        lock (m_lock)
        {
            if (!_pool.ContainsKey(key))
            {
                if (_pool.Count < _typeNum)
                {
                    _pool.Add(key, new Queue<object>(_maxSize));
                    mUnUsedCount.Add(typeof(T), 0);
                }
                else
                {
                    Debug.LogWarning($"获取大对象池数量达最大，{_typeNum}");
                }
            }
        }

        if (_pool.ContainsKey(key)) result = true;
        return result;
    }


    /// <summary>
    /// 取对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns></returns>
    public T Get<T>() where T : class, new()
    {
        T result = null;
        var key = typeof(T).GetHashCode();
        var exist = _pool.ContainsKey(key);
        if (!exist) exist = CreateObjectPool<T>(key);
        lock (m_lock)
        {
            if (exist && _pool[key].TryDequeue(out object obj))
            {
                result = obj as T;
                mUnUsedCount[typeof(T)] -= 1;
            }
        }

        if (result == null)
        {
            if (exist)
            {
                //对象池存在，但池内没有对象，创建新对象
                result = new T();
            }
            else
            {
                //对象池不存在...
                Debug.LogWarning($"获取对象池不存在，对象类型:{typeof(T)}");
            }
        }

        return result;
    }

    /// <summary>
    /// 还对象
    /// </summary>
    /// <param name="obj">要归还的对象</param>
    public void Release<T>(object obj) where T : class, new()
    {
        var key = typeof(T).GetHashCode();
        if (_pool.ContainsKey(key))
        {
            lock (m_lock)
            {
                if (_pool.ContainsKey(key))
                {
                    var pool = _pool[key];
                    if (pool.Count < _maxSize)
                    {
                        pool.Enqueue(obj);
                        mUnUsedCount[typeof(T)] += 1;
                    }
                    else
                    {
                        Debug.LogWarning($"获取对象池{typeof(T)},数量达最大，{_maxSize}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"归还对象池不存在，对象类型:{typeof(T)}");
        }
    }

    /// <summary>
    /// 释放对象池资源
    /// </summary>
    public void Dispose()
    {
        _pool.Clear();
        mUnUsedCount.Clear();
    }
}

#endregion ----- Class型对象池 ------ End
