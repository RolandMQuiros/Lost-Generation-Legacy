using System.Collections.Generic;

namespace LostGen {
    public interface IPawnManager {
        IEnumerable<Pawn> Ordered { get; }
        bool Exists(Pawn pawn);
        IEnumerable<Pawn> FindByName(string name);
        IEnumerable<Pawn> At(Point point);
        IEnumerable<Pawn> At(IEnumerable<Point> points);
        bool Add(Pawn pawn);
        bool Remove(Pawn pawn);
        bool SetPosition(Pawn pawn, Point newPosition);
    }
}