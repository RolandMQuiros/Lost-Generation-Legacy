using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

[RequireComponent(typeof(ObjectRecycler))]
public class BoardView : MonoBehaviour {
    public BoardTheme Theme;
    public Board Board { get; private set; }

    private const string _TILE_CHILD_NAME = "_tileChild";
    private const string _GRID_PREFAB_NAME = "BoardGridField";
    private GameObject _tileChild;

    public void AttachBoard(Board board) {
        Board = board;
        RebuildBoard();
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
                        newTile = GetBoardTile(Theme.WallTile, new Point(x, y), Board.WALL_TILE);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                }
            }
        }
    }

    private GameObject GetBoardTile(AutoTile autoTile, Point point, int tile) {
        return autoTile.GetTile(
            !Board.InBounds(point + Point.Right) || Board.GetTile(point + Point.Right) == tile,
            !Board.InBounds(point + Point.Down) || Board.GetTile(point + Point.Down) == tile,
            !Board.InBounds(point + Point.Left) || Board.GetTile(point + Point.Left) == tile,
            !Board.InBounds(point + Point.Up) || Board.GetTile(point + Point.Up) == tile
        );
    }
}
