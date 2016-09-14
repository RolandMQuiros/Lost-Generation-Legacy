using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    /// <summary>
    /// A node in the Planner's decision graph. A GoalNode represents a possible state of the world created
    /// as the result or cause of an IDecision.
    /// 
    /// With a given list of possible IDecisions, a single GoalNode will generate its full graph on its own through
    /// calls to GetNeighbors. The graphs are constructed from end to start, so the edges are unidirectional and point
    /// towards the current GoalNode.
    /// </summary>
    public class GoalNode : IGraphNode {
        public StateOffset State { get; private set; }
        private List<IDecision> _decisions;
        private Dictionary<GoalNode, IDecision> _edges = new Dictionary<GoalNode, IDecision>();
        private bool _cacheComplete = false;

        public GoalNode(StateOffset state, List<IDecision> decisions) {
            State = state ?? new StateOffset();
            _decisions = decisions ?? new List<IDecision>();
        }

        public IDecision GetEdge(GoalNode neighbor) {
            IDecision edge;
            _edges.TryGetValue(neighbor, out edge);

            if (edge == null) {
                throw new ArgumentException("neighbor", "Neighbor is not connected to this node");
            }

            return edge;
        }

        public int GetEdgeCost(IGraphNode neighbor) {
            // We're basically banking on GetNeighbor being called BEFORE GetEdgeCost, since the goals won't be
            // cached until the former is used.
            // This might bite us in the ass later.

            // While we're at it, we're also assuming that all GoalNodes traversed are generated from the root,
            // so all nodes used by Pathfinder are actually in the same graph.  This might be sound, but still...

            GoalNode neighborGoal = (GoalNode)neighbor;
            IDecision edge = GetEdge(neighborGoal);

            return edge.Cost;
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
            if (_cacheComplete) {
                foreach (GoalNode neighbor in _edges.Keys) {
                    yield return neighbor;
                }
            } else {
                for (int i = 0; i < _decisions.Count; i++) {
                    if (_decisions[i].ArePreconditionsMet(State)) {
                        StateOffset post = new StateOffset(State);
                        _decisions[i].ApplyPostconditions(post);

                        GoalNode previousGoal = new GoalNode(post, _decisions);
                        _edges.Add(previousGoal, _decisions[i]);

                        yield return previousGoal;
                    }
                }
                _cacheComplete = true;
            }
        }

        private bool IsMatch(GoalNode other) {
            return other.State.IsSubsetOf(State);
        }
    }

    public class Planner {
        private List<IDecision> _decisions = new List<IDecision>();
        private List<GoalNode> _goals = new List<GoalNode>();
        private Func<StateOffset, StateOffset, int> _heuristic;

        public Planner(Func<StateOffset, StateOffset, int> heuristic) {
            if (heuristic == null) {
                throw new ArgumentNullException("heuristic", "The Planner must be constructed with a heuristic");
            }

            _heuristic = heuristic;
        }

        public void AddDecision(IDecision decision) {
            _decisions.Add(decision);
        }

        public Queue<IDecision> CreatePlan(StateOffset goal) {
            GoalNode start = new GoalNode(new StateOffset(), _decisions);
            GoalNode end = new GoalNode(goal, _decisions);
            Queue<GoalNode> goals = new Queue<GoalNode>(Pathfinder<GoalNode>.FindPath(start, end, Heuristic));

            Queue<IDecision> plan = new Queue<IDecision>();
            if (goals.Count > 0) {
                GoalNode previous = goals.Dequeue();
                while (goals.Count > 0) {
                    GoalNode top = goals.Dequeue();
                    IDecision edge = previous.GetEdge(top);
                    plan.Enqueue(edge);
                    previous = top;
                }
            }

            return plan;
        }

        /*
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

                    GoalNode next = new GoalNode(cause.ApplyPostconditions(current.State));
                    next.AddNeighbor(current, cause);
                    open.Push(next);
                    causeFound = true;

                    _goals.Add(next);
                }

                if (finishedNode) {
                    open.Pop();
                }
            }
        }*/

        private int Heuristic(GoalNode g1, GoalNode g2) {
            return _heuristic(g1.State, g2.State);
        }
    }
}
