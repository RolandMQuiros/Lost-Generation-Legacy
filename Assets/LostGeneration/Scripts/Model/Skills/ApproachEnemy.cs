using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Skills {
    /// <summary>
    /// This Skill allows a Combatant to approach a pawn while factoring in the
    /// number of enemies within attacking range of their path.
    /// </summary>
    public class ApproachWithCaution : Walk {
        /// <summary>Reference to the Pawn we're approaching</summary>
        private Pawn _target;
        /// <summary>Range</summary>
        private int _range;

        public ApproachWithCaution(Combatant owner, Pawn target) 
            : base(owner) {
            _target = target;
        }

        public override void Setup() {
            
        }

        public override int GetDecisionCost() {
            int cost = ActionPoints;
            return cost;
        }
    }
}
