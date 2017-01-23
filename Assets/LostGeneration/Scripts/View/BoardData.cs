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
public class BoardData : MonoBehaviour {
    [Serializable]
    public class BoardPawnEvent : UnityEvent<Pawn> { }

    public Board Board;

    #region UnityEvents
    public UnityEvent BlocksChanged;
    public BoardPawnEvent PawnAdded;
    public BoardPawnEvent PawnRemoved; 
    #endregion UnityEvents

    public void Step() {
        Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
        Board.Step(messages);
    }

    #region MonoBehaviour
    private void Awake() {
        int[,,] grid = new int[,,] {
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 }
            },
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            }
        };

        Board = new Board(new Point(grid.GetLength(2), grid.GetLength(0), grid.GetLength(1)));

        for (int y = 0; y < grid.GetLength(0); y++) {
            for (int z = 0; z < grid.GetLength(1); z++) {
                for (int x = 0; x < grid.GetLength(2); x++) {
                    Point point = new Point(x, y, z);
                    if (grid[y,z,x] == 1) {
                        Board.SetBlock(new BoardBlock() { Point = point, IsSolid = true, IsOpaque = true });
                    } else {
                        Board.SetBlock(new BoardBlock() { Point = point, IsSolid = false, IsOpaque = false });
                    }
                }
            }
        }

        Board.BlocksChanged += delegate() { BlocksChanged.Invoke(); };
        Board.PawnAdded += delegate(Pawn pawn) { PawnAdded.Invoke(pawn); };
        Board.PawnRemoved += delegate(Pawn pawn) { PawnRemoved.Invoke(pawn); };
    }
    #endregion MonoBehaviour
}