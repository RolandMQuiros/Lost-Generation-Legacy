using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LostGen {
    public interface IGoal {
        void GetTargetState(BoardState boardState);
        bool IsAchieved(BoardState boardState);
        int Heuristic(BoardState from);
    }
}
