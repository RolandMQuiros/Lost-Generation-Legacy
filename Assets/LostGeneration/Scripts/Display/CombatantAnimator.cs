using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
	public class CombatantAnimator : MonoBehaviour {
		
		private Animator _animator;
		private int _runTrigger = Animator.StringToHash("Base Layer.Grounded");

		public IEnumerator Move(Point from, Point to, float duration, bool rotate = true) {
			Vector3 vFrom = PointVector.ToVector(from);
			Vector3 vTo = PointVector.ToVector(to);

			Quaternion rotFrom = transform.rotation;
			Quaternion rotTo = transform.rotation;
			if (rotate) {
				rotTo = Quaternion.LookRotation((vTo - vFrom).normalized, Vector3.up);
			}

			float time = 0f;

			//_animator.SetFloat("Run", 1f);
			while (time < duration) {
				time += Time.deltaTime;
				transform.position = Vector3.Lerp(vFrom, vTo, time / duration);
				
				if (rotate) {
					transform.rotation = Quaternion.Lerp(rotFrom, rotTo, time / (duration / 5f));
				}
				
				yield return null;
			}
			//_animator.SetFloat("Run", 0f);

			transform.position = vTo;

			if (rotate) {
				transform.rotation = rotTo;
			}
		}
		#region MonoBehaviour
		private void Awake() {
			_animator = GetComponent<Animator>();
		}
		#endregion
	}
}