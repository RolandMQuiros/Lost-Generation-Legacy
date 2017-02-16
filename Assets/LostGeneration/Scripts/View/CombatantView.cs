using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class CombatantView : MonoBehaviour {
	#region EditorFields
	/// <summary>Time needed to move one tile</summary>
	public float MoveDuration = 0.5f;
	#endregion EditorFields
	public Pawn Pawn;

	public void ProcessMessage(IPawnMessage message) {
		// Check the type of the message, then fire off the appropriate Coroutine
		if (Pawn == message.Source) {
			MoveMessage move = message as MoveMessage;
			if (move != null) {
				StartCoroutine(Move(move));
			}
		}
	}

	private IEnumerator Move(MoveMessage move) {
		Vector3 from = PointVector.ToVector(move.From);
		Vector3 to = PointVector.ToVector(move.To);

		Vector3 step = (to - from) / MoveDuration;
		float time = 0f;

		while (time < MoveDuration) {
			time += Time.deltaTime;
			transform.position = transform.position + (step * Time.deltaTime);
			yield return null;
		}

		transform.position = to;
	}

	#region MonoBehaviour
	private void Awake() {
		
	}
	#endregion MonoBehaviour
}
