using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class ActionPlanner {
        private class GoalNode : IGraphNode {
            public StateOffset State { get; private set; }
            private Dictionary<GoalNode, List<IDecision>> _neighbors = new Dictionary<GoalNode, List<IDecision>>();

            public GoalNode(StateOffset state = null) {
                if (state == null) {
                    State = new StateOffset();
                } else {
                    State = state;
                }
            }

            public void AddNeighbor(GoalNode neighbor, IDecision edge) {
                List<IDecision> edges;
                _neighbors.TryGetValue(neighbor, out edges);

                if (edges == null) {
                    edges = new List<IDecision>();
                    edges.Add(edge);
                    _neighbors[neighbor] = edges;
                }
            }

            public bool IsMatch(GoalNode other) {
                return State.IsSubset(other.State);
            }

            public int GetEdgeCost(IGraphNode neighbor) {
                GoalNode neighborGoal = (GoalNode)neighbor;
                List<IDecision> edges;

                _neighbors.TryGetValue(neighborGoal, out edges);
                int edgeCost = -1;

                if (edges != null) {
                    IDecision decision = edges.OrderBy(edge => edge.Cost).First();
                    edgeCost = decision.Cost;
                }

                return edgeCost;
            }

            public bool IsMatch(IGraphNode other) {
                GoalNode otherGoal = other as GoalNode;
                bool match = false;

                if (otherGoal != null) {
                    match = IsMatch(otherGoal);
                }

                return match;
            }

            public IEnumerable<IGraphNode> GetNeighbors() {
                return _neighbors.Keys.Cast<IGraphNode>();
            }
        }

        private List<IDecision> _decisions = new List<IDecision>();
        private List<GoalNode> _goals = new List<GoalNode>();
        private GoalNode _root = new GoalNode();
        private Func<StateOffset, StateOffset, int> _heuristic;

        public ActionPlanner(Func<StateOffset, StateOffset, int> heuristic) {
            if (heuristic == null) {
                throw new ArgumentNullException("heuristic", "The Planner must be constructed with a heuristic");
            }

            _heuristic = heuristic;
        }

        public void AddDecision(IDecision decision) {
            _decisions.Add(decision);
        }

        public Queue<IDecision> CreatePlan(StateOffset goal) {
            Queue<GoalNode> plan = new Queue<GoalNode>(Pathfinder<GoalNode>.FindPath(goal, _root, Heuristic));

            return plan;
        }

        public void BuildGraph() {
            Stack<GoalNode> open = new Stack<GoalNode>();

            _goals.Clear();

            open.Push(_root);
            _goals.Add(_root);

            while (open.Count > 0) {
                GoalNode current = open.Peek();

                bool finishedNode = true;
                bool causeFound = false;
                foreach (IDecision cause in _decisions) {
                    if (causeFound) {
                        // Break on the opening of the following Decision
                        // This ensures that if the current neighbor is the last, and it is an eligible cause, it triggers the finishing pops
                        finishedNode = false;
                        break;
                    }

                    if (!cause.ArePreconditionsMet(current.State)) {
                        continue;
                    }

                    GoalNode next = new GoalNode(cause.GetPostcondition(current.State));
                    next.AddNeighbor(current, cause);
                    open.Push(next);
                    causeFound = true;

                    _goals.Add(next);
                }

                if (finishedNode) {
                    open.Pop();
                }
            }
        }

        private int Heuristic(GoalNode g1, GoalNode g2) {
            return _heuristic(g1.State, g2.State);
        }
    }
}
