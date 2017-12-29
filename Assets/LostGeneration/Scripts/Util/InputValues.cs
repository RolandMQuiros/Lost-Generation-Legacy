using System;
using UnityEngine;
using UnityEngine.Events;

namespace LostGen.Util {
    [Serializable]
    public class InputValue<T> : MonoBehaviour {
        [Serializable]public class ValueEvent : UnityEvent<T> { }
        public T Value {
            get { return _value; }
            set {
                if (!_value.Equals(value)) {
                    _value = value;
                    Changed.Invoke(_value);
                }
            }
        }
        [SerializeField]private T _value;
        public ValueEvent Changed;
        public void SetValue(object value) {
            Value = (T)value;
        }
    }

    [Serializable]public class InputValues : InputValue<int> { }
}