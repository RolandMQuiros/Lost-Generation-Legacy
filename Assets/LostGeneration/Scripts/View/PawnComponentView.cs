using UnityEngine;
using LostGen;

public abstract class PawnComponentView : MonoBehaviour
{
    public Pawn Pawn;
    
    public abstract void HandleMessage(IPawnMessage message);
}