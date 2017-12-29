using System;
using UnityEngine;
using UnityEngine.Events;

namespace LostGen.Util {
	public class FloatInput : MonoBehaviour {
		[Serializable]public class InputEvent : UnityEvent<float> { }
		public float Value {
            get { return _value; }
            set { SetValue(value); }
        }
        [SerializeField]private float _value;
		public InputEvent Changed;
		public void SetValue(float value) {
			if (_value != value) {
				_value = value;
				Changed.Invoke(_value);
			}
		}

		public void SetValue(int value) {
			SetValue((float)value);
		}
	}
}