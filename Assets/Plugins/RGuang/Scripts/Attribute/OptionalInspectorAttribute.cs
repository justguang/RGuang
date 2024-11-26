using System;
using UnityEngine;

namespace RGuang.Attribute
{
    [System.Serializable]
    public struct OptionalInspector<T>
    {
        public bool Enabled => enabled;
        public T Value => value;
        [UnityEngine.SerializeField] private bool enabled;
        [UnityEngine.SerializeField] private T value;
        public OptionalInspector(T initialValue)
        {
            enabled = true;
            value = initialValue;
        }

    }


}
