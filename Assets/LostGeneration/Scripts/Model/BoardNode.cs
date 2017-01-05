using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class BoardNode : IGraphNode {
        public Board Board { get; private set; }
        public Point Point { get; protected set; }

        public BoardNode(Board board, Point point) {
            Board = board;
            Point = point;
        }

        public abstract int GetEdgeCost(BoardNode neighbor);
        public abstract IEnumerable<IGraphNode> GetNeighbors();

        public int GetEdgeCost(IGraphNode neighbor) {
            BoardNode boardNeighbor = neighbor as BoardNode;
            
            if (boardNeighbor != null) {
                return GetEdgeCost(boardNeighbor);    
            } else {
                throw new ArgumentException("Given GraphNode was not a BoardNode", "neighbor");
            }
        }
        
        public bool IsMatch(IGraphNode other) {
            BoardNode boardOther = other as BoardNode;
            if (boardOther != null) {
                return boardOther.Point == Point;    
            } else {
                throw new ArgumentException("Given GraphNode was not a BoardNode", "neighbor");
            }
        }
    }
}