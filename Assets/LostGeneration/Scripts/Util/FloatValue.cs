using System;
using UnityEngine;
using ByteSheep.Events;

namespace LostGen.Util {
    [Serializable]
    public class FloatValue : MonoBehaviour {
        public float Value {
            get { return _value; }
            set {
                if (!_value.Equals(value)) {
                    _value = value;
                    Changed.Invoke(_value);
                }
            }
        }
        [SerializeField]private string _label;
        [SerializeField]private float _value;
        public QuickFloatEvent Changed;

        public void SetValue(object value) {
            Value = (float)value;
        }

        public void SetValue(int value) {
            Value = (int)value;
        }
    }
}