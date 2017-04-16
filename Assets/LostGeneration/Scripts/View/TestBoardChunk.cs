using System;
using UnityEngine;
using LostGen;
public class TestBoardChunk : MonoBehaviour {
    public BoardRef BoardRef;
    public GameObject BlockPrefab;

    private Board _board;

    #region MonoBehaviour
    private void Start() {
        if (BoardRef == null) {
            throw new NullReferenceException("No Board Inspector assigned to this BoardGridView");
        }

        _board = BoardRef.Board;
        _board.BlocksChanged += Rebuild;

        ConstructGridView();
    }
    #endregion MonoBehaviour

    #region PrivateMethods
    private void ConstructGridView() {
        Board board = BoardRef.Board;
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

    private void Rebuild() {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
        ConstructGridView();
    }
    #endregion PrivateMethods
}