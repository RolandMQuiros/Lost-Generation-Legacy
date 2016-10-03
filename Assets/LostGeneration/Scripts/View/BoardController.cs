using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(ObjectRecycler))]
[RequireComponent(typeof(BoardView))]
[RequireComponent(typeof(CombatantViewManager))]
public class BoardController : MonoBehaviour {
    public Board Board { get; set; }
    public ICharacterFactory Characters {
        get { return _characters; }
    }

    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private const string _GRID_PREFAB_NAME = "BoardGridField";

    private ObjectRecycler _recycler;
    private BoardView _boardView;
    private CombatantViewManager _combatantManager;

    private ICharacterFactory _characters = new TestCharacterFactory();

    private List<IPawnController> _pawnControllers = new List<IPawnController>();
    private int _finishedControllers = 0;

    private bool _actionsLeft = false;
    private bool _readyToStep = true;

    public void Awake() {
        _boardView = GetComponent<BoardView>();
        _recycler = GetComponent<ObjectRecycler>();
        _combatantManager = GetComponent<CombatantViewManager>();

        if (!_recycler.IsRegistered(_GRID_PREFAB_NAME)) {
            throw new ObjectRecycler.NotRegisteredException(_GRID_PREFAB_NAME);
        }
    }

    public void Start() {
        _boardView.AttachBoard(Board);
        _combatantManager.Initialize(_characters);
    }

    public bool Step() {
        if (_readyToStep) {
            _actionsLeft = Board.Step();
        }

        _readyToStep = true;
        _readyToStep &= !_combatantManager.OnStep();

        return _actionsLeft;
    }

    public GameObject GetBoardFieldObject() {
        return _recycler.Spawn(_GRID_PREFAB_NAME);
    }
}

