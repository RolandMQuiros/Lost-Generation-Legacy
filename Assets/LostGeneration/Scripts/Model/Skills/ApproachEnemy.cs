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
        private int _scanRange;

        public ApproachWithCaution(Combatant owner, int scanRange) 
            : base(owner) {
            _scanRange = scanRange;
        }

        protected override int Heuristic(Point start, Point end) {
            int distance = base.Heuristic(start, end);

            int cost = distance;

            foreach (Pawn pawn in Owner.GetPawnsInView()) {
                Combatant enemy = pawn as Combatant;
                if (enemy != null && Point.Distance(start, enemy.Position) < _scanRange) {
                    cost += 100;
                }
            }

            return cost;
        }

        public override int GetDecisionCost() {
            int cost = ActionPoints;
            return cost;
        }
    }
}
