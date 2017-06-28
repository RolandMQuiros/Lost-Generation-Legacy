using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class CombatantAnimator : MonoBehaviour {
	#region EditorFields
	/// <summary>Time needed to move across one block</summary>
	[TooltipAttribute("Time needed to move across one block")]
	public float MoveDuration = 0.5f;
	#endregion EditorFields
	
	private Animator _animator;
	private int _runTrigger = Animator.StringToHash("Base Layer.Grounded");

	#region MonoBehaviour
	private void Awake() {
		_animator = GetComponent<Animator>();
	}
	#endregion MonoBehaviour

	public IEnumerator Move(Point from, Point to) {
		Vector3 vFrom = PointVector.ToVector(from);
		Vector3 vTo = PointVector.ToVector(to);

		Quaternion rotFrom = transform.rotation;
		Quaternion rotTo = Quaternion.LookRotation((vTo - vFrom).normalized, Vector3.up);

		Vector3 step = (vTo - vFrom) / MoveDuration;
		float time = 0f;

		_animator.SetFloat("Run", 1f);
		while (time < MoveDuration) {
			time += Time.deltaTime;
			transform.position = Vector3.Lerp(vFrom, vTo, time / MoveDuration);
			transform.rotation = Quaternion.Lerp(rotFrom, rotTo, time / (MoveDuration / 5f));
			
			yield return null;
		}
		_animator.SetFloat("Run", 0f);

		transform.position = vTo;
		transform.rotation = rotTo;
	}
}
