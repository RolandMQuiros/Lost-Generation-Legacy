using System.Collections.Generic;

namespace LostGen {
    public interface ITask {
        StateOffset Preconditions { get; }
        StateOffset ApplyPostconditions(StateOffset state);
        IEnumerable<ITask> Decompose(StateOffset from, StateOffset to);
    }
}