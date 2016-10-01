using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class CombatantViewManager : MonoBehaviour {
    public GameObject CombatantViewPrefab;
    public BoardTheme BoardTheme;
    public Board Board { get; private set; }
    public ICharacterFactory Characters { get; set; }

    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private Dictionary<Combatant, CombatantView> _combatantViews = new Dictionary<Combatant, CombatantView>();
    private MessageBuffer _buffer = new MessageBuffer();
    private Transform _pawnChild;

    public void Start() {
        _pawnChild = transform.FindChild(_PAWN_CHILD_NAME);

        if (CombatantViewPrefab == null) {
            throw new NullReferenceException("CombatantViewPrefab was not set");
        }

        if (BoardTheme == null) {
            throw new NullReferenceException("BoardTheme was not set");
        }
    }

    public void Initialize(Board board, ICharacterFactory characters) {
        Board = board;
        Board.PawnAdded += OnPawnAdded;
        Board.PawnRemoved += OnPawnRemoved;
        Characters = characters;

        IEnumerator<Pawn> pawnIter = Board.GetPawnIterator();
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
            Vector3 position = BoardTheme.PointToVector3(pawn.Position);

            if (_pawnChild == null) {
                GameObject pawnChildObj = new GameObject(_PAWN_CHILD_NAME);
                _pawnChild = pawnChildObj.transform;
                _pawnChild.transform.SetParent(transform);
            }

            Transform parentTransform = transform;
            GameObject combatantObj = GameObject.Instantiate<GameObject>(CombatantViewPrefab);

            CombatantView combatantView = combatantObj.GetComponent<CombatantView>();

            combatantObj.transform.SetParent(_pawnChild.transform);
            combatantObj.transform.position = ViewCommon.PointToVector3(combatant.Position);
            combatantObj.name = combatant.Name;

            combatantView.BoardTheme = BoardTheme;
            combatantView.Combatant = combatant;

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



