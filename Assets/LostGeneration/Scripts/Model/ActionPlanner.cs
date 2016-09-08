using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class GoalNode : IGraphNode {
        private StateOffset _state;
        private Dictionary<GoalNode, SortedList<IDecision>> _neighbors = new Dictionary<GoalNode, List<IDecision>>();

        public GoalNode(StateOffset state) {
            _state = state;
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

        public int GetEdgeCost(IGraphNode neighbor) {
            GoalNode neighborGoal = (GoalNode)neighbor;
            List<IDecision> edges;

            _neighbors.TryGetValue(neighborGoal, out edges);
            
            if (edges != null) {
                
            }
        }
        
        public IEnumerable<IGraphNode> GetNeighbors() {
            return _neighbors.Keys.Cast<IGraphNode>();
        }
    }

    public class ActionPlanner {
        private List<IDecision> _decisions = new List<IDecision>();
        private List<StateOffset> _states = new List<StateOffset>();

        public void AddDecision(IDecision decision) {
            _decisions.Add(decision);
        }

        public List<IDecision> CreatePlan(StateOffset goal) {

        }

        public void BuildGraph() {
            for (int i = 0; i < _decisions.Count; i++) {
                StateOffset postState = _decisions[i].PostCondition;
                _states.Add(postState);
            }
        }
    }
}
