using UnityEngine;
using LostGen;

public abstract class PawnComponentView : MonoBehaviour
{
    public Pawn Pawn { get; private set; }
    
    public abstract void HandleMessage(IPawnMessage message);
}