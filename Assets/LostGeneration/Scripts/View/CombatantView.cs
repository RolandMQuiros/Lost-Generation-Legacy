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
	private Animator _animator;
	private int _runTrigger = Animator.StringToHash("Base Layer.Grounded");

	#region MonoBehaviour
	private void Awake() {
		_animator = GetComponent<Animator>();
	}
	#endregion MonoBehaviour

	/// <summary>
    /// Takes a message and starts a Coroutine based on that message.
	/// This should only be fed messages pumped out of a MessageBuffer on a
	/// per-Step basis, which should prevent contradictory messages from running
	/// at the same time. e.g., if two MoveMessages are processed at the same time,
	/// the resulting Coroutines will fight to update the object's location.
	/// </summary>
    /// <param name="message">IPawnMessage to process</param>
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

		Quaternion rotFrom = transform.rotation;
		Quaternion rotTo = Quaternion.LookRotation((to - from).normalized, Vector3.up);

		Vector3 step = (to - from) / MoveDuration;
		float time = 0f;

		_animator.SetFloat("Run", 1f);
		while (time < MoveDuration) {
			time += Time.deltaTime;
			transform.position = Vector3.Lerp(from, to, time / MoveDuration);
			transform.rotation = Quaternion.Lerp(rotFrom, rotTo, time / (MoveDuration / 5f));
			
			yield return null;
		}
		_animator.SetFloat("Run", 0f);

		transform.position = to;
		transform.rotation = rotTo;

		Finish(move);
	}


}
