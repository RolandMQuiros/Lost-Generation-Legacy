using System;
using LostGen.Model;

namespace LostGen.Display {
    public interface IBoardCursorController {
        Point BoardPoint { get; }
        event Action<Point> Clicked;
        event Action<Point> TappedDown;
        event Action<Point> TappedUp;
        event Action<Point> Moved;
    }
}