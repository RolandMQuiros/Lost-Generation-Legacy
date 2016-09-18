using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

public class BoardView : MonoBehaviour {
    public BoardTheme Theme;
    public GameObject CombatantViewPrefab;

    public Board Board { get; private set; }
    public ICharacterManager Characters;

    private const string _TILE_CHILD_NAME = "_tileChild";
    private const string _PAWN_CHILD_NAME = "_pawnChild";
    private GameObject _tileChild;
    private GameObject _pawnChild;
    
    private Dictionary<Combatant, CombatantView> _combatantViews = new Dictionary<Combatant, CombatantView>();
    private MessageBuffer _buffer = new MessageBuffer();

    public void Start() {
        //if (CombatantViewPrefab.GetComponent<CombatantView>()) {
        //    throw new MissingComponentException("The Combatant View prefab is missing a CombatantView component");
        //}
    }

    public void AttachBoard(Board board) {
        Board = board;
        Board.PawnAdded += OnPawnAdded;
        Board.PawnRemoved += OnPawnRemoved;

        RebuildBoard();

        IEnumerator<Pawn> pawnIter = Board.GetPawnIterator();
        while (pawnIter.MoveNext()) {
            OnPawnAdded(pawnIter.Current);
        }
    }

    public void RebuildBoard() {
        if (_tileChild != null && _tileChild.transform.childCount > 0) {
            GameObject.Destroy(_tileChild);
        }

        _tileChild = new GameObject(_TILE_CHILD_NAME);
        _tileChild.transform.SetParent(transform);

        for (int y = 0; y < Board.Height; y++) {
            for (int x = 0; x < Board.Width; x++) {
                GameObject newTile;
                Vector3 position = Theme.PointToVector3(new Point(x, y));
                switch (Board.GetTile(x, y)) {
                    case Board.FLOOR_TILE:
                        newTile = GameObject.Instantiate(Theme.FloorTile);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                    case Board.WALL_TILE:
                        newTile = Theme.WallTile.GetTile(Board, new Point(x, y), Board.WALL_TILE);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                }
            }
        }
    }

    public bool Step() {
        return Board.Step();
    }

    private void OnPawnAdded(Pawn pawn) {
        Combatant combatant = pawn as Combatant;
        if (combatant != null) {
            Character character = Characters.GetCharacter(combatant.CharacterID);
            Vector3 position = Theme.PointToVector3(pawn.Position);

            GameObject combatantObj = GameObject.Instantiate<GameObject>(CombatantViewPrefab);
            CombatantView combatantView = combatantObj.GetComponent<CombatantView>();

            if (_pawnChild == null) {
                _pawnChild = new GameObject(_PAWN_CHILD_NAME);
                _pawnChild.transform.SetParent(transform);
            }

            combatantObj.transform.SetParent(_pawnChild.transform);
            combatantObj.transform.position = Theme.PointToVector3(combatant.Position);
            combatantObj.name = combatant.Name;

            combatantView.BoardTheme = Theme;
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
