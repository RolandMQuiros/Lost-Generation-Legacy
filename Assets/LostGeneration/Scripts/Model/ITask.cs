using System.Collections.Generic;

namespace LostGen {
    public interface ITask {
        StateOffset ApplyPostconditions(StateOffset state);
        bool ArePreconditionsMet(StateOffset state);
        IEnumerable<ITask> Decompose(StateOffset from, StateOffset to);
    }
}