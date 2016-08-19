using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    
    public class Planner {
        private class StartNode : DecisionNode {
            public override bool ArePreconditionsMet(StateOffset state) { return true; }
            public override int GetCost(StateOffset offset = null) { return 0; }
            public override void Run() { }
            public override void Setup() { }

            public override StateOffset GetPostconditions(StateOffset state = null) {
                return (state == null) ? new StateOffset() : state;
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
            _rebuildGraph = success;
            return success;
        }

        public void FindPlan(DecisionNode goal) {
            if (!_decisions.Contains(goal)) {
                throw new ArgumentException("Goal does not exist in Decision graph. Use Planner.AddDecision to add to graph.", "goal");
            }

            if (_rebuildGraph) {
                BuildGraph();
            }
        }

        private void BuildGraph() {
            Stack<DecisionNode> open = new Stack<DecisionNode>();
            open.Push(_root);

            StateOffset currentPost = new StateOffset();
            while (open.Count > 0) {
                DecisionNode current = open.Pop();
                currentPost = current.GetPostconditions(currentPost);

                foreach (DecisionNode other in _decisions) {
                    if (current == other || !other.ArePreconditionsMet(currentPost)) {
                        continue;
                    }

                    if (other.AddCause(current, other.GetCost(currentPost))) {
                        open.Push(other);
                        break;
                    }
                }
            }

            foreach (DecisionNode decision in _decisions) {
                StateOffset post = decision.GetPostconditions();
                foreach (DecisionNode other in _decisions) {
                    if (decision != other && other.ArePreconditionsMet(post)) {
                        
                    }
                }
            }
        }
    }
}
