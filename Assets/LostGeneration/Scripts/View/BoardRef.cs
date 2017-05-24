using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

/// <summary>
/// Unity Inspector interface for the Board.  Allows us to edit the board through Unity either directly
/// or by some procgen utility.  More importantly, it lets us spread references to the Board through the
/// Editor, instead of having to create classes specifically to assign the reference to components.
/// </summary>
public class BoardRef : MonoBehaviour {
    [Serializable]
    public class BoardPawnEvent : UnityEvent<Pawn> { }
    [Serializable]
    public class BoardStepEvent : UnityEvent<Queue<IPawnMessage>> { }

    public Board Board;

    #region UnityEvents
    public UnityEvent BlocksChanged;
    public BoardPawnEvent PawnAdded;
    public BoardPawnEvent PawnRemoved;
    public BoardStepEvent BoardStepped;
    #endregion UnityEvents

    public void Step() {
        Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
        Board.Step(messages);

        BoardStepped.Invoke(messages);
    }

    #region MonoBehaviour
    private void Awake() {
        Board.BlocksChanged += delegate() { BlocksChanged.Invoke(); };
        Board.PawnAdded += delegate(Pawn pawn) { PawnAdded.Invoke(pawn); };
        Board.PawnRemoved += delegate(Pawn pawn) { PawnRemoved.Invoke(pawn); };
    }
    #endregion MonoBehaviour
}