using UnityEngine;
using LostGen;
public class TestCursorDigger : MonoBehaviour {
    public BoardRef BoardRef;

    public void Dig(Point point) {
        if (BoardRef.Board.Blocks.InBounds(point)) {
            BoardRef.Board.Blocks.Set(new BoardBlock() { Point = point, IsSolid = false, IsOpaque = false });
        }
    }
}