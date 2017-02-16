using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class CombatantView : MonoBehaviour {
	#region EditorFields
	/// <summary>Time needed to move across one block</summary>
	[TooltipAttribute("Time needed to move across one block")]
	public float MoveDuration = 0.5f;
	/// <summary>Invoked when this CombatantView's coroutines finish.</summary>
	[TooltipAttribute("Invoked when this CombatantView's coroutines finish")]
	public UnityEvent Finished;
	#endregion EditorFields
	public Pawn Pawn;
	private Dictionary<IPawnMessage, Coroutine> _activeRuns = new Dictionary<IPawnMessage, Coroutine>();
	
	public void ProcessMessage(IPawnMessage message) {
		// Check the type of the message, then fire off the appropriate Coroutine
		if (Pawn == message.Source) {
			MoveMessage move = message as MoveMessage;
			if (move != null) {
				Coroutine moveCo = StartCoroutine(Move(move));
				_activeRuns.Add(move, moveCo);
			}
		}
	}

	private void Finish(IPawnMessage message) {
		// Removes the message from the list of active Coroutines
		_activeRuns.Remove(message);
		if (_activeRuns.Count == 0) {
			Finished.Invoke();
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

		Finish(move);
	}

	#region MonoBehaviour
	private void Awake() {
		
	}
	#endregion MonoBehaviour
}
