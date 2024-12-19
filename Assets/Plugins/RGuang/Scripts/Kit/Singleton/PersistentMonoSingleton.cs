using UnityEngine;

namespace RGuang.Kit
{
    /// <summary>
    /// MonoBehaviour 单例类
    /// 如果场景里包含两个及以上的 PersistentMonoSingleton 则保留最先创建的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        protected bool _enabled;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                var obj = new GameObject();
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                _enabled = true;
            }
            else
            {
                if (this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
