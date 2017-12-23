using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public interface IPawnManager {
        IEnumerable<Pawn> Ordered { get; }
        event Action<Pawn> Added;
        event Action<Pawn> Removed;
        bool Contains(Pawn pawn);
        IEnumerable<Pawn> FindByName(string name);
        IEnumerable<Pawn> At(Point point);
        IEnumerable<Pawn> At(IEnumerable<Point> points);
        bool Add(Pawn pawn);
        bool Remove(Pawn pawn);
        void Move(Pawn pawn, Point newPosition);
    }
}