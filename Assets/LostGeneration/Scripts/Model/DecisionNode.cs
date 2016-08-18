using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DecisionNode : IGraphNode {
        public Board Board { get; private set; }
        private Dictionary<DecisionNode, int> _neighbors = new Dictionary<DecisionNode, int>();

        public DecisionNode(Board board) {
            Board = board;
        }

        public IEnumerable<IGraphNode> GetNeighbors() {
            return _neighbors.Keys.Cast<IGraphNode>();
        }

        public int GetEdgeCost(IGraphNode neighbor) {
            DecisionNode neighborDecision = (DecisionNode)neighbor;
            int edgeCost = 0;

            if (_neighbors.ContainsKey(neighborDecision)) {
                edgeCost = _neighbors[neighborDecision];
            } else {
                throw new ArgumentOutOfRangeException("neighbor", "Given IGraphNode is not a neighbor of this DecisionNode");
            }

            return edgeCost;
        }

        public bool TryToAddNeighbor(DecisionNode other) {
            return false;
        }

        public abstract bool ArePreconditionsMet(BoardState state);
        public abstract BoardState GetPostconditions();

        public abstract void Setup();
        public abstract int GetCost();
        public abstract void Run();

        public bool IsMatch(IGraphNode other) {
            DecisionNode otherDecision = (DecisionNode)other;
            return ArePreconditionsMet(otherDecision.GetPostconditions());
        }
    }
}
