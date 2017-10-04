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
    [Serializable]
    public class BoardBlocksEvent : UnityEvent<Dictionary<BoardBlock, BoardBlock>> { }

    public Board Board
    {
        get { return _board; }
        set
        {
            if (_board != null)
            {
                _board.Blocks.Changed -= OnBlocksChanged;
                _board.Pawns.Added -= OnPawnAdded;
                _board.Pawns.Removed -= OnPawnRemoved;
            }
            _board = value;
            _board.Blocks.Changed += OnBlocksChanged;
            _board.Pawns.Added += OnPawnAdded;
            _board.Pawns.Removed += OnPawnRemoved;
        }
    }

    #region UnityEvents
    public BoardBlocksEvent BlocksChanged;
    public BoardPawnEvent PawnAdded;
    public BoardPawnEvent PawnRemoved;
    public BoardStepEvent BoardStepped;
    #endregion UnityEvents

    private Board _board;

    public void Step() {
        Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
        Board.Step(messages);
        BoardStepped.Invoke(messages);
    }

    public void BeginTurn() {
        _board.BeginTurn();
    }

    public void Turn() {
        Queue<IPawnMessage> messages = new Queue<IPawnMessage>();

        do {
            messages.Clear();
            Board.Step(messages);
            if (messages.Count > 0) {
                BoardStepped.Invoke(messages);
            }
        } while (messages.Count > 0);
    }

    private void OnBlocksChanged(Dictionary<BoardBlock, BoardBlock> blocksChanged)
    {
        BlocksChanged.Invoke(blocksChanged);
    }

    private void OnPawnAdded(Pawn pawn)
    {
        PawnAdded.Invoke(pawn);
    }

    private void OnPawnRemoved(Pawn pawn)
    {
        PawnRemoved.Invoke(pawn);
    }

    #region MonoBehaviour
    private void Start() {
        BeginTurn();
    }
    #endregion MonoBehaviour
}