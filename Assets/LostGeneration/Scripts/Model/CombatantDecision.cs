using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class CombatantDecision : IDecision {
        public Combatant Source { get { return _source; } }
        public abstract int DecisionCost { get; }
        private Combatant _source;

        public CombatantDecision(Combatant source) {
            _source = source;
        }

        public abstract StateOffset ApplyPostconditions(StateOffset state);
        public abstract bool ArePreconditionsMet(StateOffset state);
        public abstract void Run();

        /// <summary>
        /// Called before a plan is created. Used to set up values that persist across multiple iterations of this Decision.
        /// </summary>
        public virtual void Setup() { }
    }
}
