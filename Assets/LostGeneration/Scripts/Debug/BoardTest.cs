using System.Collections.Generic;
using UnityEngine;
using LostGen;
public class BoardTest : MonoBehaviour
{   
    public Point BoardSize;
    private BoardRef _boardRef;

    #region MonoBehaviour
    private void Awake()
    {
        _boardRef = GetComponent<BoardRef>();
        _boardRef.Board = new Board(new Point(BoardSize.X, BoardSize.Y, BoardSize.Z));

        List<BoardBlock> blocks = new List<BoardBlock>();
        for (int x = 0; x < _boardRef.Board.Size.X; x++)
        {
            for (int y = 0; y < _boardRef.Board.Size.Y; y++)
            {
                for (int z = 0; z < _boardRef.Board.Size.Z; z++)
                {
                    blocks.Add
                    (
                        new BoardBlock()
                        {
                            Point = new Point(x, y, z),
                            BlockType = Mathf.RoundToInt(Random.value)
                        }
                    );
                }
            }
        }
        _boardRef.Board.SetBlocks(blocks);
    }
    #endregion MonoBehaviour
}