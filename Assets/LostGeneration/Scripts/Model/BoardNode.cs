
using System;
using System.Collections.Generic;

namespace LostGen {
    /// <summary>
    /// Graph node representation of a Board coordinate. This connects Points on the Board with adjacent
    /// Points, and sets edge values depending on at those points, or Tile types.
    /// </summary>
    public class BoardNode : IGraphNode {
        /// <summary>
        /// A function that takes a Point on the Board, and evaluates the AP cost of moving to that Point, often
        /// depending on the Tile type or what Pawns occupy that Point.
        /// 
        /// Having a delegate handle the cost evaluation lets different Pawns decide their movement strategy differently.
        /// </summary>
        /// <param name="point">Point to move to. Does not necessarily need to be adjacent to this Node.</param>
        /// <returns>Cost of moving to point</returns>
        public delegate int EdgeCostLookup(Point point);

        /// <summary>Point on the Board</summary>
        protected Point _point;
        public Point Point { get { return _point; } }
        /// <summary>Reference to the Board</summary>
        protected Board _board;
        /// <summary>List of adjacent, non-Wall, in-bounds Points on the Board</summary>
        protected List<IGraphNode> _neighbors = null;
        /// <summary>Edge cost callback</summary>
        protected EdgeCostLookup _edgeCostLookup;

        /// <summary>
        /// Construct a new Node
        /// </summary>
        /// <param name="board">Reference to the Board</param>
        /// <param name="point">Point on the Board.  Must be within bounds, or else an ArugmentException will be raised.</param>
        /// <param name="lookup"></param>
        public BoardNode(Board board, Point point, EdgeCostLookup lookup = null) {
            if (board == null) { throw new ArgumentNullException("board"); }
            if (lookup == null) { throw new ArgumentNullException("lookup"); }

            _board = board;
            if (!_board.InBounds(point)) {
                throw new ArgumentException("Given point is not on the given board", "point");
            }
            _point = point;
            _edgeCostLookup = lookup;
        }

        /// <summary>
        /// Returns the cost of moving from the current Node to an adjacent Node
        /// </summary>
        /// <param name="neighbor">Another Node. Doesn't actually have to be adjacent to this.</param>
        /// <returns></returns>
        public int GetEdgeCost(IGraphNode neighbor) {
            BoardNode boardNode = (BoardNode)neighbor;
            int cost = 0;
            
            if (_edgeCostLookup != null) {
                cost = _edgeCostLookup(boardNode.Point);
            }

            return cost;
        }

        /// <summary>
        /// Iterates through adjacent Points.  To avoid having to construct the entire Board as a graph,
        /// this function acts as an iterator, and creates neighbor Nodes only as needed.
        /// </summary>
        /// <returns>An IEnumerable that iterates through this Node's neighboring Points</returns>
        public IEnumerable<IGraphNode> GetNeighbors() {
            if (_neighbors == null) {
                _neighbors = new List<IGraphNode>();
                for (int i = 0; i < Point.Neighbors.Length; i++) {
                    Point neighborPoint = _point + Point.Neighbors[i];

                    if (_board.InBounds(neighborPoint) && _board.GetBlock(neighborPoint).IsSolid) {
                        BoardNode neighbor = new BoardNode(_board, neighborPoint, _edgeCostLookup);
                        _neighbors.Add(neighbor);
                    }
                }
            }

            return _neighbors;
        }

        public bool IsMatch(IGraphNode other) {
            BoardNode otherNode = (BoardNode)other;
            return Point.Equals(otherNode.Point);
        }

        public override int GetHashCode() {
            return _point.GetHashCode();
        }

        public override bool Equals(object obj) {
            return _point == ((BoardNode)obj).Point;
        }
    }
}