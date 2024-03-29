using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen.Model {
    public abstract class PrimitiveTask : ITask {
        public abstract WorldState Preconditions { get; }
        public abstract WorldState Postconditions { get; }
        public abstract IEnumerator Do();
        public IEnumerator Do(WorldState start, WorldState goal) { return Do(); }
        public IEnumerable<ITask> Decompose(WorldState from, WorldState to) { return Enumerable.Empty<ITask>(); }
    }
}