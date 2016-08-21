using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    
    public class Planner {
        private class StartNode : DecisionNode {
            public override bool ArePreconditionsMet(StateOffset state) { return false; }
            public override int GetCost(StateOffset offset = null) { return Int32.MaxValue; }
            public override void Run() { }
            public override void Setup() { }

            public override StateOffset GetPostconditions(StateOffset state = null) {
                return new StateOffset();
            }
        }

        private StartNode _root;
        private HashSet<DecisionNode> _decisions = new HashSet<DecisionNode>();
        private bool _rebuildGraph = false;

        public Planner() {
            _root = new StartNode();
            _decisions.Add(_root);
        }

        /// <summary>
        /// Runs the Setup method on each Skill
        /// </summary>
        public void Setup() {
            foreach (DecisionNode decision in _decisions) {
                decision.Setup();
            }
        }

        public bool AddDecision(DecisionNode decision) {
            bool success = _decisions.Add(decision);
            _rebuildGraph |= success;
            return success;
        }

        public IEnumerable<DecisionNode> FindPlan(Goal goal) {
            if (!_decisions.Contains(goal)) {
                throw new ArgumentException("Goal does not exist in Decision graph. Use Planner.AddDecision to add to graph.", "goal");
            }

            if (_rebuildGraph) {
                BuildGraph();
            }

            return Pathfinder<DecisionNode>.FindPath(goal, _root, goal.Heuristic);
        }

        public void BuildGraph() {
            Stack<DecisionNode> open = new Stack<DecisionNode>();
            Stack<StateOffset> outcomes = new Stack<StateOffset>();

            open.Push(_root);
            outcomes.Push(new StateOffset());

            while (open.Count > 0) {
                DecisionNode current = open.Peek();
                StateOffset currentPost = outcomes.Peek();

                bool finishedNode = true;
                bool causeFound = false;
                foreach (DecisionNode cause in _decisions) {
                    if (causeFound) {
                        // Break on the opening of the following Decision
                        // This ensures that if the current neighbor is the last, and it is an eligible cause, it triggers the finishing pops
                        finishedNode = false;
                        break;
                    }

                    if (current == cause || !cause.ArePreconditionsMet(currentPost)) {
                        continue;
                    }

                    if (cause.AddCause(current, cause.GetCost(currentPost))) {
                        StateOffset nextState = new StateOffset(currentPost);
                        cause.GetPostconditions(nextState);

                        open.Push(cause);
                        outcomes.Push(nextState);

                        causeFound = true;
                    }
                }

                if (finishedNode) {
                    open.Pop();
                    outcomes.Pop();
                }
            }

            _rebuildGraph = false;
        }
    }
}
