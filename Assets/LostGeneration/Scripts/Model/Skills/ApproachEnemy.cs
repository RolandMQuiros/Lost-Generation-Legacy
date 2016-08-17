using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Skills {
    /// <summary>
    /// Allows a Combatant to approach a pawn while factoring in the
    /// number of enemies within attacking range of their path.
    /// </summary>
    public class ApproachWithCaution : Walk {
        /// <summary>Reference to the Pawn we're approaching</summary>
        private Pawn _target;

        public ApproachWithCaution(Combatant owner, Pawn target) 
            : base(owner) {
            _target = target;
        }

        protected override int Heuristic(Point start, Point end) {
            int distance = base.Heuristic(start, end);

            int cost = distance;
            // Check current board state for enemies within range of the current starting point
            

            return cost;
        }

        public override int GetDecisionCost() {
            int cost = ActionPoints;
            return cost;
        }
    }
}
