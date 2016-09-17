using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

public class BoardView : MonoBehaviour {
    public BoardTheme Theme;
    public Board Board;

    private const string _TILE_CHILD_NAME = "_tileChild";
    private GameObject _tileChild;

    public void Awake() {
        Board.PawnAdded += OnPawnAdded;
        Board.PawnRemoved += OnPawnRemoved;
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

    private void OnPawnAdded(Pawn pawn) {

    }

    private void OnPawnRemoved(Pawn pawn) {

    }
}
