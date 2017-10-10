using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public abstract class PawnComponentView : MonoBehaviour {
    public Pawn Pawn;
    public abstract void PushMessage(IPawnMessage message);
    public abstract IEnumerator HandleMessages();
}