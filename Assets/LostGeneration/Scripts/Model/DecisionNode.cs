using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DecisionNode : IGraphNode {
        public Board Board { get; private set; }
        private List<DecisionNode> _neighbors = new List<DecisionNode>();

        public DecisionNode(Board board) {
            Board = board;
        }

        /// <summary>
        /// Returns the postcondition Board State of this Decision
        /// </summary>
        /// <returns>Postcondition BoardState</returns>
        public IEnumerable<IGraphNode> GetNeighbors() {
            for (int i = 0; i < _neighbors.Count; i++) {
                yield return _neighbors[i];
            }
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
