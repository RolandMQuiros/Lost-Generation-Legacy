using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Decision {
    public class Approach : IDecision {
        public int Cost {
            get {
                throw new NotImplementedException();
            }
        }

        private Combatant _source;
        private Point _destination;

        public Approach(Combatant source) {
            _source = source;
        }

        public StateOffset ApplyPostconditions(StateOffset state) {
            state.Set(StateOffset.CombatantKey(_source, "position"), _destination);
            return state;
        }

        public bool ArePreconditionsMet(StateOffset state) {
            Point destination = state.Get(StateOffset.CombatantKey(_source, "ApproachDest"), _source.Position);
            Point currentPosition = state.Get(StateOffset.CombatantKey(_source, "position"), _source.Position);
            return !destination.Equals(currentPosition);
        }

        public void Run() {
            
        }

        public void Setup() {
            throw new NotImplementedException();
        }
    }
}
