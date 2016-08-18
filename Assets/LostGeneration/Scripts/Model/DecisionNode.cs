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

        /// <summary>
        /// Returns the postcondition Board State of this Decision
        /// </summary>
        /// <returns>Postcondition BoardState</returns>
        public IEnumerable<IGraphNode> GetNeighbors() {
            foreach (DecisionNode key in _neighbors.Keys) {
                yield return key;
            }
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

        /// <summary>Sets Skills</summary>
        public abstract void Setup();
        /// <summary>Calculates decision cost</summary>
        /// <returns></returns>
        public abstract int GetCost();
        /// <summary>Runs Skills</summary>
        public abstract void Run();

        public abstract bool IsMatch(IGraphNode other);
    }
}
