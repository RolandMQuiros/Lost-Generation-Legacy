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
    private Queue<IPawnMessage> _messages = new Queue<IPawnMessage>();

    public void HandleMessage(object sender, IPawnMessage message) {
        _messages.Enqueue(message);
    }

    public void PushMessages(IEnumerable<IPawnMessage> messages) {
        foreach (IPawnMessage message in messages) {
            _messages.Enqueue(message);
        }
    }

    public void PopMessages(Queue<IPawnMessage> messages) {
        HashSet<Pawn> actingPawns = new HashSet<Pawn>();
        bool stopPopping = false;

        while(_messages.Count > 0 && !stopPopping) {
            IPawnMessage next = _messages.Dequeue();

            if (next.Source != null) {
                actingPawns.Add(next.Source);
            }

            // If the next message is critical or one of the previous messages acted on the same target,
            // then stop popping
            if (next.IsCritical ||
                (next.Target != null && !actingPawns.Add(next.Target))) {
                stopPopping = true;
            }

            messages.Enqueue(next);
        }
    }
}

