using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public abstract class BlockNode : IGraphNode<BlockNode> {
        public Board Board { get; private set; }
        public Point Point { get; protected set; }

        public BlockNode(Board board, Point point) {
            Board = board;
            Point = point;
        }

        public abstract IEnumerable<BlockNode> GetNeighbors();

        public abstract int GetEdgeCost(BlockNode neighbor);
        
        public virtual bool IsMatch(BlockNode other) {
            BlockNode boardOther = other as BlockNode;
            if (boardOther != null) {
                return boardOther.Point == Point;    
            } else {
                throw new ArgumentException("Given GraphNode was not a BoardNode", "neighbor");
            }
        }

        public override int GetHashCode() {
            return Point.GetHashCode();
        }

        public override bool Equals(object other) {
            BlockNode otherNode = other as BlockNode;
            return otherNode != null &&
                   otherNode.Board == Board &&
                   otherNode.Point == Point;
        }
    }
}