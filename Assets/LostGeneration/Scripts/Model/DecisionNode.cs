using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DecisionNode : IGraphNode {
        private Dictionary<DecisionNode, int> _causes = new Dictionary<DecisionNode, int>();

        public IEnumerable<IGraphNode> GetNeighbors() {
            return _causes.Keys.Cast<IGraphNode>();
        }

        public int GetEdgeCost(IGraphNode neighbor) {
            DecisionNode neighborDecision = (DecisionNode)neighbor;
            int edgeCost = 0;

            if (_causes.ContainsKey(neighborDecision)) {
                edgeCost = _causes[neighborDecision];
            } else {
                throw new ArgumentOutOfRangeException("neighbor", "Given IGraphNode is not a neighbor of this DecisionNode");
            }

            return edgeCost;
        }

        public void ClearCauses() {
            _causes.Clear();
        }

        public bool AddCause(DecisionNode other, int cost) {
            bool added = false;
            if (!_causes.ContainsKey(other)) {
                _causes.Add(other, cost);
                added = true;
            }

            return added;
        }

        public abstract bool ArePreconditionsMet(StateOffset state);
        public abstract StateOffset GetPostconditions(StateOffset state = null);

        public virtual void Setup() { }
        public abstract int GetCost(StateOffset state = null);
        public virtual void Run() { }

        public bool IsMatch(IGraphNode other) {
            DecisionNode otherDecision = (DecisionNode)other;
            return ArePreconditionsMet(otherDecision.GetPostconditions());
        }
    }
}
