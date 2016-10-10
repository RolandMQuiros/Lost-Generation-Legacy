using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class CombatantViewManager : MonoBehaviour {
    #region UnityFields
    public CombatantView CombatantViewPrefab;
    public BoardGridField BoardGridFieldPrefab;
    public BoardCursor Cursor;
    #endregion

    #region Components
    private BoardView _boardView;
    private BoardGridField _boardGridField;
    private TrashMan _pool;
    #endregion

    #region References
    private Transform _pawnChild;
    #endregion

    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private const string _GRID_FIELD_CHILD = "_grids";
    private Dictionary<Combatant, CombatantView> _combatantViews = new Dictionary<Combatant, CombatantView>();
    private MessageBuffer _buffer = new MessageBuffer();
    private Transform _gridFieldChild;

    private ICharacterFactory _characters;
    private Board _board;
    private BoardTheme _theme;

    public void Initialize(ICharacterFactory characters, Board board, BoardTheme theme) {
        _characters = characters;
        _board = board;
        _theme = theme;

        _board.PawnAdded += OnPawnAdded;
        _board.PawnRemoved += OnPawnRemoved;
        
        IEnumerator<Pawn> pawnIter = _board.GetPawnIterator();
        while (pawnIter.MoveNext()) {
            OnPawnAdded(pawnIter.Current);
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>true if there are still messages left on the queue</returns>
    public bool OnStep() {
        Queue<MessageArgs> messages = _buffer.PopMessages();

        while (messages.Count > 0) {
            MessageArgs message = messages.Dequeue();

            Combatant source = message.Source as Combatant;
            if (source != null) {
                CombatantView view = _combatantViews[source];
                view.OnMessage(message);
            }
        }
        
        return !_buffer.IsStepFinished;
    }

    #region MonoBehaviour
    private void Awake() {
        _boardView = GetComponent<BoardView>();
        _boardGridField = GetComponent<BoardGridField>();
        _pool = GetComponent<TrashMan>();
    }

    private void Start() {
        _pawnChild = transform.FindChild(_PAWN_CHILD_NAME);
        _gridFieldChild = transform.FindChild(_GRID_FIELD_CHILD);

        Cursor = GetComponentInChildren<BoardCursor>();

        if (CombatantViewPrefab == null) {
            throw new NullReferenceException("CombatantViewPrefab was not set");
        }
    }
    #endregion

    #region PrivateMethods
    private void OnPawnAdded(Pawn pawn) {
        Combatant combatant = pawn as Combatant;
        if (combatant != null) {
            Character character = _characters.GetCharacter(combatant.CharacterID);
            Vector3 position = _theme.PointToVector3(pawn.Position);

            if (_pawnChild == null) {
                GameObject pawnChildObj = new GameObject(_PAWN_CHILD_NAME);
                _pawnChild = pawnChildObj.transform;
                _pawnChild.transform.SetParent(transform);
            }

            if (_gridFieldChild == null) {
                GameObject gridObj = new GameObject(_GRID_FIELD_CHILD);
                _gridFieldChild = gridObj.transform;
                _gridFieldChild.SetParent(transform);
            }

            Transform parentTransform = transform;
            GameObject combatantObj = GameObject.Instantiate<GameObject>(CombatantViewPrefab.gameObject);            
            combatantObj.transform.SetParent(_pawnChild.transform);
            combatantObj.transform.position = ViewCommon.PointToVector3(combatant.Position);
            combatantObj.name = combatant.Name;

            CombatantView combatantView = combatantObj.GetComponent<CombatantView>();
            combatantView.Initialize(combatant, _theme);

            SkillView skillView = combatantObj.GetComponent<SkillView>();

            GameObject gridFieldObj = TrashMan.spawn(BoardGridFieldPrefab.gameObject, Vector3.zero, Quaternion.identity, _gridFieldChild);

            BoardGridField gridField = gridFieldObj.GetComponent<BoardGridField>();
            gridField.Initialize(_theme);

            skillView.Initialize(gridField);
            skillView.Combatant = combatant;

            combatant.Messages += _buffer.HandleMessage;
            _combatantViews.Add(combatant, combatantView);
        }
    }

    private void OnPawnRemoved(Pawn pawn) {
        Combatant combatant = pawn as Combatant;
        if (combatant != null) {
            CombatantView view;
            _combatantViews.TryGetValue(combatant, out view);
            if (view != null) {
                combatant.Messages -= _buffer.HandleMessage;

                view.gameObject.SetActive(false);
                _combatantViews.Remove(combatant);
            }
        }
    }
    #endregion PrivateMethods
}
