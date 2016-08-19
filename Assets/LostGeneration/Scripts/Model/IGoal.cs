using System;

namespace LostGen {
    public interface IGoal {
        bool ArePreconditionsMet(StateOffset state);
    }
}
