using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class BlockNode : IGraphNode {
        public Board Board { get; private set; }
        public Point Point { get; protected set; }

        public BlockNode(Board board, Point point) {
            Board = board;
            Point = point;
        }

        public abstract int GetEdgeCost(BlockNode neighbor);
        public abstract IEnumerable<IGraphNode> GetNeighbors();

        public int GetEdgeCost(IGraphNode neighbor) {
            BlockNode boardNeighbor = neighbor as BlockNode;
            
            if (boardNeighbor != null) {
                return GetEdgeCost(boardNeighbor);    
            } else {
                throw new ArgumentException("Given GraphNode was not a BoardNode", "neighbor");
            }
        }
        
        public bool IsMatch(IGraphNode other) {
            BlockNode boardOther = other as BlockNode;
            if (boardOther != null) {
                return boardOther.Point == Point;    
            } else {
                throw new ArgumentException("Given GraphNode was not a BoardNode", "neighbor");
            }
        }
    }
}