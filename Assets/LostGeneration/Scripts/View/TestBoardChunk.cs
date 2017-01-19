using System;
using UnityEngine;
using LostGen;
public class TestBoardChunk : MonoBehaviour {
    public BoardData BoardInspector;
    public Point Corner1;
    public Point Corner2;

    public GameObject BlockPrefab;

    private Board _board;

    #region MonoBehaviour
    private void Start() {
        if (BoardInspector == null) {
            throw new NullReferenceException("No Board Inspector assigned to this BoardGridView");
        }

        _board = BoardInspector.Board;
        
        if (!_board.InBounds(Corner1) || !_board.InBounds(Corner2)) {
            throw new IndexOutOfRangeException("Given BoardChunk corners are outside the bounds of the Board");
        }

        ConstructGridView();
    }
    #endregion MonoBehaviour

    #region PrivateMethods
    private void ConstructGridView() {
        Board board = BoardInspector.Board;
        Point size = board.Size;

        for (int z = 0; z < size.Z; z++) {
            for (int y = 0; y < size.Y; y++) {
                for (int x = 0; x < size.X; x++) {
                    Point point = new Point(x, y, z);
                    BoardBlock block = _board.GetBlock(point);

                    if (block.IsSolid) {
                        Vector3 position = new Vector3((float)x, (float)y, (float)z);
                        GameObject newBlock = GameObject.Instantiate(BlockPrefab, position, Quaternion.identity);
                        newBlock.transform.parent = gameObject.transform;
                    }
                }
            }
        }
    }
    #endregion PrivateMethods
}