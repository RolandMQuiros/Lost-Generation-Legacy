using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LostGen;

public class BoardView : MonoBehaviour {
    private const string _TILE_CHILD_NAME = "_tiles";
    private GameObject _tileChild;

    private Board _board;
    private BoardTheme _theme;

    public void Initialize(Board board, BoardTheme theme) {
        _board = board;
        _theme = theme;
        RebuildBoard();
    }

    public void RebuildBoard() {
        if (_tileChild != null && _tileChild.transform.childCount > 0) {
            GameObject.Destroy(_tileChild);
        }

        _tileChild = new GameObject(_TILE_CHILD_NAME);
        _tileChild.transform.SetParent(transform);

        for (int y = 0; y < _board.Height; y++) {
            for (int x = 0; x < _board.Width; x++) {
                GameObject newTile;
                Vector3 position = _theme.PointToVector3(new Point(x, y));
                switch (_board.GetTile(x, y)) {
                    case Board.FLOOR_TILE:
                        newTile = GameObject.Instantiate(_theme.FloorTile);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                    case Board.WALL_TILE:
                        newTile = GetBoardTile(_theme.WallTile, new Point(x, y), Board.WALL_TILE);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                }
            }
        }
    }
    
    private GameObject GetBoardTile(AutoTile autoTile, Point point, int tile) {
        return autoTile.GetTile(
            !_board.InBounds(point + Point.Right) || _board.GetTile(point + Point.Right) == tile,
            !_board.InBounds(point + Point.Down) || _board.GetTile(point + Point.Down) == tile,
            !_board.InBounds(point + Point.Left) || _board.GetTile(point + Point.Left) == tile,
            !_board.InBounds(point + Point.Up) || _board.GetTile(point + Point.Up) == tile
        );
    }
}
