using UnityEngine;
using LostGen;
public class TestCursorDigger : MonoBehaviour {
    public BoardRef BoardRef;

    public void Dig(Point point) {
        if (BoardRef.Board.InBounds(point)) {
            BoardRef.Board.SetBlock(new BoardBlock() { Point = point, IsSolid = false, IsOpaque = false });
        }
    }
}