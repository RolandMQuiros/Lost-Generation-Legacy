using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class PawnDecision : IDecision {
        public Pawn Source { get { return _source; } }
        public abstract int Cost { get; }
        private Pawn _source;

        public PawnDecision(Pawn source) {
            _source = source;
        }

        public abstract StateOffset ApplyPostconditions(StateOffset state);
        public abstract bool ArePreconditionsMet(StateOffset state);
        public abstract void Run();
        public abstract PawnAction Step();
    }
}
