using UnityEngine;
using LostGen;
public class TestCursorDigger : MonoBehaviour {
    public BoardData BoardData;

    public void Dig(Point point) {
        if (BoardData.Board.InBounds(point)) {
            BoardData.Board.SetBlock(new BoardBlock() { Point = point, IsSolid = false, IsOpaque = false });
        }
    }
}