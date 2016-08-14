using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LostGen {
    public interface IGoal {
        bool IsAchieved(BoardState boardState);
        int Heuristic(BoardState from);
    }
}
