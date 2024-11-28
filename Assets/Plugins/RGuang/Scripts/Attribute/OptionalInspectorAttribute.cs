using System;
using UnityEngine;
using UnityEngine.Events;

namespace RGuang.Attribute
{
    [System.Serializable]
    public struct OptionalInspector<T>
    {
        /// <summary>
        /// True在Inspector面板上可读写，
        /// False在Inspector面板上只读
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public T Value
        {
            get => value;
            set
            {
                if (value.Equals(this.value)) return;
                T eventArgs = this.value;
                this.value = value;
                E_OnValueChanged.Invoke(eventArgs);
            }
        }
        [UnityEngine.SerializeField] private bool enabled;
        [UnityEngine.SerializeField] private T value;

        /// <summary>
        /// 变更值后事件通知：
        ///     通知参数：变更前的值
        /// </summary>
        public UnityEvent<T> E_OnValueChanged;

        public OptionalInspector(T initialValue, UnityAction<T> onValueChangedCallback = null)
        {
            enabled = true;
            value = initialValue;
            E_OnValueChanged = new UnityEvent<T>();
            if (onValueChangedCallback != null) E_OnValueChanged.AddListener(onValueChangedCallback);
        }

        public OptionalInspector(bool enable, T initialValue, UnityAction<T> onValueChangedCallback = null)
        {
            enabled = enable;
            value = initialValue;
            E_OnValueChanged = new UnityEvent<T>();
            if (onValueChangedCallback != null) E_OnValueChanged.AddListener(onValueChangedCallback);
        }




    }

}
