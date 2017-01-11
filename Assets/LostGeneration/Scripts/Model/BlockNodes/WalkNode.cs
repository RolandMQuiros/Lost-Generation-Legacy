using System;
using System.Collections.Generic;

namespace LostGen {
    public class WalkNode : BlockNode {
        public bool CanWalkDiagonally;
        private Dictionary<Point, WalkNode> _neighbors = new Dictionary<Point, WalkNode>();
        private bool _neighborsBuilt = false;
        
        public WalkNode(Board board, Point point, bool canWalkDiagonally)
            : base(board, point) {
                CanWalkDiagonally = canWalkDiagonally;
            }
        
        public override int GetEdgeCost(BlockNode neighbor) {
            // Moving to adjacent tiles on the same plane or lower costs 1 point
            // Moving to adjacent tiles from a lower to a higher plane costs the height difference
            return Math.Abs(neighbor.Point.X - Point.X) + Math.Max(neighbor.Point.Y - Point.Y, 0);
        }

        public override IEnumerable<IGraphNode> GetNeighbors() {
            BuildNeighbors();
            foreach (WalkNode neighbor in _neighbors.Values) {
                yield return neighbor;
            }
        }

        private void BuildNeighbors() {
            if (!_neighborsBuilt) {
                Stack<Point> open = new Stack<Point>();
                
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

                    if (!_neighbors.ContainsKey(adjacent)) {
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
                                    WalkNode neighborNode = new WalkNode(Board, adjacent, CanWalkDiagonally);
                                    neighborNode._neighbors.Add(Point, this);
                                    _neighbors.Add(adjacent, neighborNode);
                                } else {
                                    open.Push(below);
                                }
                            }
                        }
                    }
                }
                _neighborsBuilt = true;
            }
        }
        
    }
}