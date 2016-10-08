using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(BoardView))]
[RequireComponent(typeof(CombatantViewManager))]
public class BoardController : MonoBehaviour {
    #region UnityFields
    public BoardGridField BoardGridFieldPrefab;
    #endregion UnityFields

    #region PublicProperties
    public Board Board { get; set; }
    public ICharacterFactory Characters {
        get { return _characters; }
    }
    public event Action<Board> BoardInitialized;
    #endregion PublicProperties

    #region Components
    private BoardView _boardView;
    private CombatantViewManager _combatantManager;
    #endregion

    private ICharacterFactory _characters = new TestCharacterFactory();
    private bool _actionsLeft = false;
    private bool _readyToStep = true;

    public bool Step() {
        if (!_actionsLeft) {
            Board.BeginTurn();
        }

        if (_readyToStep) {
            _actionsLeft = Board.Step();
        }

        _readyToStep = true;
        _readyToStep &= !_combatantManager.OnStep();

        return _actionsLeft;
    }

    #region MonoBehaviour

    private void Awake() {
        _boardView = GetComponent<BoardView>();
        _combatantManager = GetComponent<CombatantViewManager>();
    }

    private void Start() {
        _boardView.AttachBoard(Board);
        _combatantManager.Initialize(_characters);

        // Construction based on BoardTheme here

        if (BoardInitialized != null) {
            BoardInitialized(Board);
        }

        Board.BeginTurn();
    }

    #endregion
}

