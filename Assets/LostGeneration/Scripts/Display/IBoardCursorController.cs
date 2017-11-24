using System;
using LostGen;
public interface IBoardCursorController {
    Point BoardPoint { get; }
    event Action<Point> Clicked;
    event Action<Point> TappedDown;
    event Action<Point> TappedUp;
    event Action<Point> Moved;
}