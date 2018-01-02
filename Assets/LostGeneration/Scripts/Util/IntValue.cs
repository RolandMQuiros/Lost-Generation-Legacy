using System;
using UnityEngine;
using ByteSheep.Events;

namespace LostGen.Util {
    [Serializable]
    public class IntValue : MonoBehaviour {
        public int Value {
            get { return _value; }
            set {
                if (!_value.Equals(value)) {
                    _value = value;
                    Changed.Invoke(_value);
                }
            }
        }
        [SerializeField]private string _label;
        [SerializeField]private int _value;
        public QuickIntEvent Changed;
        
        public void SetValue(object value) {
            Value = (int)value;
        }

        public void SetValue(float value) {
            Value = (int)value;
        }
    }
}