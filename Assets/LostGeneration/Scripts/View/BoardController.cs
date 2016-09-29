using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(ObjectRecycler))]
[RequireComponent(typeof(BoardView))]
[RequireComponent(typeof(CombatantViewManager))]
public class BoardController : MonoBehaviour {
    public Board Board { get; set; }
    public ICharacterFactory Characters = new TestCharacterFactory();

    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private const string _GRID_PREFAB_NAME = "BoardGridField";

    private ObjectRecycler _recycler;
    private BoardView _boardView;
    private CombatantViewManager _combatantManager;

    private bool _readyToStep = true;

    public void Awake() {
        _boardView = GetComponent<BoardView>();
        _recycler = GetComponent<ObjectRecycler>();
        _combatantManager = GetComponent<CombatantViewManager>();

        _boardView.AttachBoard(Board);
        _combatantManager.AttachBoard(Board);
    }

    public void Step() {
        if (_readyToStep) {
            Board.Step();
        }

        _readyToStep = true;
        _readyToStep &= !_combatantManager.OnStep();
    }

    public GameObject GetBoardFieldObject() {
        return _recycler.Spawn(_GRID_PREFAB_NAME);
    }
}

