using System;
using System.Collections.Generic;
using LostGen;

/// <summary>
/// Buffers messages from the LostGen model, so messages are delegated out to the view at a controllable rate.
/// That is, this class makes sure that messages that depend on the order of actions are displayed as such.
/// </summary>
public class MessageBuffer {
    public bool IsStepFinished {
        get {
            return _messages.Count == 0;
        }
    }
    private Queue<MessageArgs> _messages = new Queue<MessageArgs>();

    public void HandleMessage(object sender, MessageArgs message) {
        _messages.Enqueue(message);
    }

    public Queue<MessageArgs> PopMessages() {
        Queue<MessageArgs> popped = new Queue<MessageArgs>();

        HashSet<Pawn> actingPawns = new HashSet<Pawn>();
        bool stopPopping = false;

        while(_messages.Count > 0 && !stopPopping) {
            MessageArgs next = _messages.Dequeue();

            if (next.Source != null) {
                actingPawns.Add(next.Source);
            }

            // If the next message is critical or one of the previous messages acted on the same target,
            // then stop popping
            if (next.IsCritical ||
                (next.Target != null && !actingPawns.Add(next.Target))) {
                stopPopping = true;
            }

            popped.Enqueue(next);
        }

        return popped;
    }
}

