using System;
using System.Collections.Generic;

namespace LostGen {
    public class WalkNode : BlockNode {
        public bool CanWalkDiagonally;
        private Dictionary<WalkNode, int> _neighbors = null;
        
        public WalkNode(Board board, Point point, bool canWalkDiagonally = true)
            : base(board, point) {
                CanWalkDiagonally = canWalkDiagonally;
            }
        
        public override int GetEdgeCost(BlockNode neighbor) {
            BuildNeighbors();
            return _neighbors[neighbor as WalkNode];
        }

        public override IEnumerable<IGraphNode> GetNeighbors() {
            BuildNeighbors();
            foreach (WalkNode neighbor in _neighbors.Keys) {
                yield return neighbor;
            }
        }

        private void BuildNeighbors() {
            if (_neighbors == null) {
                Stack<Point> open = new Stack<Point>();
                _neighbors = new Dictionary<WalkNode, int>();
                
                Point[] neighborPoints;
                if (CanWalkDiagonally) {
                    neighborPoints = Point.TransverseOctoNeighbors; 
                } else {
                    neighborPoints = Point.TransverseNeighbors;
                }

                foreach (Point offset in neighborPoints) {
                    Point neighbor = offset + Point;
                    if (Board.InBounds(neighbor)) {
                        open.Push(neighbor);
                    }
                }

                while (open.Count > 0) {
                    Point adjacent = open.Pop();
                    BoardBlock block = Board.GetBlock(adjacent);
                    // Check if there is either a solid Pawn at the latest location, or if there's a solid block
                    if (Board.IsSolid(adjacent)) {
                        // If there is a solid block, then push the point above the solid one onto the stack
                        Point above = adjacent + Point.Up;
                        if (block.IsSolid && Board.InBounds(above)) {
                            open.Push(above);
                        }
                    } else {
                        // If the space is clear, check if it's on top of a solid block
                        Point below = adjacent + Point.Down;
                        if (Board.InBounds(below)) {
                            BoardBlock blockBelow = Board.GetBlock(below);
                            if (blockBelow.IsSolid) {
                                // Moving to adjacent tiles on the same plane costs 1 point
                                // Moving to tiles offset on the Y-axis adds the difference to the cost
                                int cost = Math.Abs(adjacent.Y - Point.Y) + 1;
                                _neighbors.Add(new WalkNode(Board, adjacent), cost);
                            } else {
                                open.Push(below);
                            }
                        }
                    }
                }
            }
        }
        
    }
}