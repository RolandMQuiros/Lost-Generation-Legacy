using System;
using System.Collections.Generic;

namespace LostGen {
    public class WalkNode : BlockNode {
        private bool _canWalkDiagonally;
        private Dictionary<Point, WalkNode> _neighbors = new Dictionary<Point, WalkNode>();
        private bool _neighborsBuilt = false;
        private bool _ignorePawns = false;
        public bool CanWalkDiagonally { get { return _canWalkDiagonally; } }
        public WalkNode(Board board, Point point, bool canWalkDiagonally, bool ignorePawns)
        : base(board, point) {
            _canWalkDiagonally = canWalkDiagonally;
            _ignorePawns = ignorePawns;
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
                if (_canWalkDiagonally) {
                    neighborPoints = Point.NeighborsFullXZ; 
                } else {
                    neighborPoints = Point.NeighborsXZ;
                }

                foreach (Point offset in neighborPoints) {
                    Point neighbor = offset + Point;
                    if (Board.Blocks.InBounds(neighbor)) {
                        open.Push(neighbor);
                    }
                }

                while (open.Count > 0) {
                    Point adjacent = open.Pop();
                    BoardBlock block = Board.Blocks.At(adjacent);

                    if (!_neighbors.ContainsKey(adjacent)) {
                        // Check if there is either a solid Pawn at the latest location, or if there's a solid block
                        if ((_ignorePawns && block.IsSolid) || (!_ignorePawns && Board.IsSolid(adjacent))) {
                            // If there is a solid block, then push the point above the solid one onto the stack
                            Point above = adjacent + Point.Up;
                            if (block.IsSolid && Board.Blocks.InBounds(above)) {
                                open.Push(above);
                            }
                        } else {
                            // If the space is clear, check if it's on top of a solid block
                            Point below = adjacent + Point.Down;
                            if (Board.Blocks.InBounds(below)) {
                                BoardBlock blockBelow = Board.Blocks.At(below);
                                if (blockBelow.IsSolid) {
                                    WalkNode neighborNode = new WalkNode(Board, adjacent, _canWalkDiagonally, _ignorePawns);
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