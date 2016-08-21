using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class Goal : DecisionNode {
        /// <summary>
        /// Checks the current state, and other factors, often related to the planning actor's preferences,
        /// and returns a score affecting the actor's probability of choosing this Goal for its planning.
        /// </summary>
        /// <param name="state">The current </param>
        /// <returns></returns>
        public abstract int ChoiceWeight(StateOffset state);

        /// <summary>
        /// Heuristic for this Goal.  Used to judge the cost of Decisions based on achieving this Goal.
        /// As the Planner search starts from the Goal and ends at the current state, write this Heuristic with this in mind.
        /// </summary>
        /// <param name="start">Current DecisionNode in the search</param>
        /// <param name="end">Ending DecisionNode in the search. Will always be the root of the Graph.</param>
        /// <returns></returns>
        public abstract int Heuristic(DecisionNode start, DecisionNode end);
    }
}
