using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public interface ITask {
        WorldState Preconditions { get; }
        WorldState Postconditions { get; }
        bool IsValid();
        IEnumerable<ITask> Decompose(WorldState from, WorldState to);
        IEnumerator Do(WorldState start, WorldState goal);
    }
}