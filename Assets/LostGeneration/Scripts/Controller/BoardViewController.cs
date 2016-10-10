using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardViewController : MonoBehaviour {
    #region UnityFields
    public BoardTheme BoardTheme;
    public BoardView BoardView;
    public BoardCursor BoardCursor;
    public PlayerController PlayerController;
    public BoardCamera BoardCamera;
    public BoardCameraController BoardCameraController;
    public CombatantViewManager CombatantViewManager;
    #endregion UnityFields

    #region Properties
    public Board Board { get; protected set; }
    public ICharacterFactory Characters {
        get { return _characters; }
    }
    #endregion Properties

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
        _readyToStep &= !CombatantViewManager.OnStep();

        return _actionsLeft;
    }

    #region MonoBehaviour

    private void Awake() {
        // Construction based on BoardTheme here
    }

    private void Start() {
        // Assign all Model-bound variables to View and Controller GameObjects
        BoardView.Initialize(Board, BoardTheme);
        BoardCursor.Initialize(BoardTheme);
        BoardCamera.Initialize(BoardTheme);
        BoardCameraController.Initialize(BoardTheme);
        CombatantViewManager.Initialize(_characters, Board, BoardTheme);

        Board.BeginTurn();
    }

    #endregion
}

