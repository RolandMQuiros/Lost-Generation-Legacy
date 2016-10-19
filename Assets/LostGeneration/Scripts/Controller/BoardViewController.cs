using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardViewController : MonoBehaviour {
    #region UnityFields
    public BoardTheme BoardTheme;
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

    public void Step() {
        if (!_actionsLeft) {
            Board.BeginTurn();
        }

        if (_readyToStep) {
            _actionsLeft = Board.Step();
        }

        _readyToStep = true;
        _readyToStep &= !CombatantViewManager.OnStep();
    }

    public void Turn() {
        Board.BeginTurn();
        Board.Turn();

        while (CombatantViewManager.OnStep());
    }

    #region MonoBehaviour

    private void Awake() {
        BoardTheme.Set(BoardTheme);
    }

    private void Start() {
        List<BoardView> boardViews = new List<BoardView>(GetComponentsInChildren<BoardView>());
        List<BoardClickCollider> clickColliders = new List<BoardClickCollider>(GetComponentsInChildren<BoardClickCollider>());
        List<BoardCursor> cursors = new List<BoardCursor>(GetComponentsInChildren<BoardCursor>());
        List<BoardCamera> cameras = new List<BoardCamera>(GetComponentsInChildren<BoardCamera>());
        List<BoardCameraController> cameraControllers = new List<BoardCameraController>(GetComponentsInChildren<BoardCameraController>());
        List<CombatantViewManager> combatantViewManagers = new List<CombatantViewManager>(GetComponentsInChildren<CombatantViewManager>());

        // Assign all Model-bound variables to View and Controller GameObjects
        boardViews.ForEach(x => x.Initialize(Board, BoardTheme));
        clickColliders.ForEach(x => x.Initialize(BoardTheme, Board.Width, Board.Height));
        cursors.ForEach(x => x.Initialize(BoardTheme));
        cameras.ForEach(x => x.Initialize(BoardTheme));
        cameraControllers.ForEach(x => x.Initialize(BoardTheme));
        combatantViewManagers.ForEach(x => x.Initialize(_characters, Board, BoardTheme));

        Board.BeginTurn();
    }

    #endregion
}

