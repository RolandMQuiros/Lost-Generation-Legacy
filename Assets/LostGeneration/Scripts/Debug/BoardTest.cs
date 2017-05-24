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

        for (int x = 0; x < _boardRef.Board.Size.X; x++)
        {
            for (int y = 0; y < _boardRef.Board.Size.Y; y++)
            {
                for (int z = 0; y < _boardRef.Board.Size.Z; z++)
                {
                    _boardRef.Board.SetBlock
                    (
                        new BoardBlock()
                        {
                            BlockType = (int)(Random.value * 5f)
                        }
                    );
                }
            }
        }
    }
    #endregion MonoBehaviour
}