using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public abstract class PrimitiveTask : ITask {
        public abstract StateOffset ApplyPreconditions(StateOffset state);
        public abstract bool ArePreconditionsMet(StateOffset state);
        public abstract StateOffset ApplyPostconditions(StateOffset state);
        public abstract void Do();
        public IEnumerable<ITask> Decompose(StateOffset from, StateOffset to) { return Enumerable.Empty<ITask>(); }
    }
}