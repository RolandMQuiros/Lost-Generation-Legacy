using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class CombatantViewManager : MonoBehaviour {
    public GameObject CombatantViewPrefab;
    public ICharacterFactory Characters { get; set; }

    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private Dictionary<Combatant, CombatantView> _combatantViews = new Dictionary<Combatant, CombatantView>();
    private MessageBuffer _buffer = new MessageBuffer();

    private BoardView _boardView;
    private BoardGridField _boardGridField;
    private BoardCursor _cursor;
    private Transform _pawnChild;

    #region MonoBehaviour
    private void Awake() {
        _boardView = GetComponent<BoardView>();
        _boardGridField = GetComponent<BoardGridField>();
    }

    private void Start() {
        _pawnChild = transform.FindChild(_PAWN_CHILD_NAME);
        _cursor = GetComponentInChildren<BoardCursor>();

        if (CombatantViewPrefab == null) {
            throw new NullReferenceException("CombatantViewPrefab was not set");
        }
    }
    #endregion

    public void Initialize(ICharacterFactory characters) {
        _boardView.Board.PawnAdded += OnPawnAdded;
        _boardView.Board.PawnRemoved += OnPawnRemoved;
        Characters = characters;

        IEnumerator<Pawn> pawnIter = _boardView.Board.GetPawnIterator();
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

    private void OnPawnAdded(Pawn pawn) {
        Combatant combatant = pawn as Combatant;
        if (combatant != null) {
            Character character = Characters.GetCharacter(combatant.CharacterID);
            Vector3 position = _boardView.Theme.PointToVector3(pawn.Position);

            if (_pawnChild == null) {
                GameObject pawnChildObj = new GameObject(_PAWN_CHILD_NAME);
                _pawnChild = pawnChildObj.transform;
                _pawnChild.transform.SetParent(transform);
            }

            Transform parentTransform = transform;
            GameObject combatantObj = GameObject.Instantiate<GameObject>(CombatantViewPrefab);            
            combatantObj.transform.SetParent(_pawnChild.transform);
            combatantObj.transform.position = ViewCommon.PointToVector3(combatant.Position);
            combatantObj.name = combatant.Name;

            CombatantView combatantView = combatantObj.GetComponent<CombatantView>();
            combatantView.BoardTheme = _boardView.Theme;
            combatantView.Combatant = combatant;

            BoardGridField gridField = combatantObj.GetComponent<BoardGridField>();
            gridField.Theme = _boardView.Theme;

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
}



