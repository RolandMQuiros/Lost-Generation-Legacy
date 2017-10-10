using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;
using System;

[RequireComponent(typeof(CombatantAnimator))]
public class CombatantView : PawnComponentView
{
	#region EditorFields
	/// <summary>Invoked when this CombatantView's coroutines finish.</summary>
	[TooltipAttribute("Invoked when this CombatantView's coroutines finish")]
	public UnityEvent Finished;
	#endregion EditorFields
	private CombatantAnimator _view;
	private Dictionary<IPawnMessage, Coroutine> _activeRuns = new Dictionary<IPawnMessage, Coroutine>();

    private List<IPawnMessage> _messages = new List<IPawnMessage>();

    /// <summary>
    /// Takes a message and starts a Coroutine based on that message.
    /// This should only be fed messages pumped out of a MessageBuffer on a
    /// per-Step basis, which should prevent contradictory messages from running
    /// at the same time. e.g., if two MoveMessages are processed at the same time,
    /// the resulting Coroutines will fight to update the object's location.
    /// </summary>
    /// <param name="message">IPawnMessage to process</param>
    public override void PushMessage(IPawnMessage message) {
        _messages.Add(message);
    }

    public override IEnumerator HandleMessages() {
        for (int i = 0; i < _messages.Count; i++) {
            yield return StartCoroutineFromMessage(_messages[i]);
        }
    }

    private Coroutine StartCoroutineFromMessage(IPawnMessage message) {
        Coroutine co = null;
        if (Pawn == message.Source) {
            MoveMessage move = message as MoveMessage;
            if (move != null) {
                co = StartCoroutine(OnMoveMessage(move));
            }
        }
        return co;
    }

	private void Finish(IPawnMessage message) {
		// Removes the message from the list of active Coroutines
		_activeRuns.Remove(message);
		if (_activeRuns.Count == 0) {
			Finished.Invoke();
		}
	}

	private IEnumerator OnMoveMessage(MoveMessage move)
	{
		yield return _view.Move(move.From, move.To);
		Finish(move);
	}


	#region MonoBehaviour
	private void Awake()
	{
		_view = GetComponent<CombatantAnimator>();
	}
	#endregion MonoBehaviour
}
