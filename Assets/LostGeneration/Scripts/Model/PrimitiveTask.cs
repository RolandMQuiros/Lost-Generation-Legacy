using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public abstract class PrimitiveTask : ITask {
        public abstract WorldState Preconditions { get; }
        public abstract WorldState Postconditions { get; }
        public abstract bool ArePreconditionsMet();
        public abstract IEnumerator Do(WorldState start, WorldState goal);
        public IEnumerable<ITask> Decompose(WorldState from, WorldState to) { return Enumerable.Empty<ITask>(); }
    }
}