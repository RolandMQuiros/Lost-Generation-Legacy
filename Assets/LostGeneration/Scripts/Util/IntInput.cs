using System;
using UnityEngine;
using UnityEngine.Events;

namespace LostGen.Util {
	public class IntInput : MonoBehaviour {
		[Serializable]public class IntEvent : UnityEvent<int> { }
		[Serializable]public class FloatEvent : UnityEvent<float> { }
		public int Value {
            get { return _value; }
            set {
				if (_value != value) {
					_value = value;
					Notify();
				}
			}
        }
        [SerializeField]private int _value;
		public IntEvent IntChanged;
		public FloatEvent FloatChanged;
		public void SetValue(float value) { Value = (int)value; }
		private void Notify() {
			IntChanged.Invoke(_value);
			FloatChanged.Invoke(_value);
		}
	}
}