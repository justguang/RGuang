using UnityEngine;

namespace RGuang.Kit
{
    /// <summary>
    /// MonoBehaviour 单例类
    /// 如果场景里包含两个及以上的 ReplaceableMonoSingleton 则保留最后创建的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        public float InitializationTime;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var obj = new GameObject
                        {
                            hideFlags = HideFlags.HideAndDontSave
                        };
                        _instance = obj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            InitializationTime = Time.time;

            DontDestroyOnLoad(this.gameObject);

            var check = FindObjectsOfType<T>();
            foreach (var searched in check)
            {
                if (searched == this) continue;
                if (searched.GetComponent<ReplaceableMonoSingleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(searched.gameObject);
                }
            }

            if (_instance == null)
            {
                _instance = this as T;
            }
        }
    }
}
