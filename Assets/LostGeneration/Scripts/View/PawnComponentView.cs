using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public abstract class PawnComponentView : MonoBehaviour {
    public Pawn Pawn;
    protected IEnumerable<IPawnMessage> Messages {
        get { return _messages.AsReadOnly(); }
    }

    private List<IPawnMessage> _messages = new List<IPawnMessage>();

    public virtual void PushMessage(IPawnMessage message) {
        _messages.Add(message);
    }

    public abstract IEnumerator HandleMessages();
}