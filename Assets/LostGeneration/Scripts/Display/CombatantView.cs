using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;
using System;

[RequireComponent(typeof(CombatantAnimator))]
public class CombatantView : PawnComponentView {
    #region EditorFields
    [SerializeField]private float _moveDuration = 0.5f;
    #endregion EditorFields
    private CombatantAnimator _view;
    private Queue<IPawnMessage> _messages = new Queue<IPawnMessage>();

    /// <summary>
    /// Takes a message and starts a Coroutine based on that message.
    /// This should only be fed messages pumped out of a MessageBuffer on a
    /// per-Step basis, which should prevent contradictory messages from running
    /// at the same time. e.g., if two MoveMessages are processed at the same time,
    /// the resulting Coroutines will fight to update the object's location.
    /// </summary>
    /// <param name="message">IPawnMessage to process</param>
    public override void PushMessage(IPawnMessage message) {
        _messages.Enqueue(message);
    }

    public override IEnumerator ProcessMessages() {
        while (_messages.Count > 0) {
            IPawnMessage message = _messages.Dequeue();
            yield return StartCoroutineFromMessage(message);
        }
    }

    private Coroutine StartCoroutineFromMessage(IPawnMessage message) {
        Coroutine co = null;
        if (Pawn == message.Source) {
            MoveMessage move = message as MoveMessage;
            if (move != null) {
                co = StartCoroutine(_view.Move(move.From, move.To, _moveDuration));
            }
        }
        return co;
    }

    #region MonoBehaviour
    private void Awake() {
        _view = GetComponent<CombatantAnimator>();
    }
    #endregion MonoBehaviour
}
