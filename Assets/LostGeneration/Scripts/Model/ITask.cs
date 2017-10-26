using System.Collections.Generic;

namespace LostGen {
    public interface ITask {
        StateOffset ApplyPreconditions(StateOffset state);
        bool ArePreconditionsMet(StateOffset state);
        StateOffset ApplyPostconditions(StateOffset state);
        IEnumerable<ITask> Decompose(StateOffset from, StateOffset to);
    }
}