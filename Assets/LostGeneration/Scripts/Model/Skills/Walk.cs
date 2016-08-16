using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LostGen.Skills {
    /// <summary>
    /// A Skill that allows Combatants to traverse the Board in a continuous path.
    /// This class provides a graph model of the board and a method of setting the destination,
    /// and is intended to be directly used by human-controlled Combatants.
    /// 
    /// AI Combatants will use child classes of Skills.Walk, which choose a destination based
    /// on some goal objective.  For example, Skills.ApproachWithCaution chooses a destination that
    /// is closest to a target Pawn, but has minimal possible lines of sight with the enemy.
    /// 
    /// Keep this distinction in mind when designing Skills, in general.
    /// A human player can set Skill parameters more freely than the AI, so we need to derive
    /// more specific skills to cover smaller possibility spaces.
    /// </summary>
    public class Walk : Skill {
        /// <summary>
        /// Graph node representation of a Board coordinate. This connects Points on the Board with adjacent
        /// Points, and sets edge values depending on at those points, or Tile types.
        /// </summary>
        protected class WalkNode : GraphNode<Point> {
            /// <summary>
            /// A function that takes a Point on the Board, and evaluates the AP cost of moving to that Point, often
            /// depending on the Tile type or what Pawns occupy that Point.
            /// 
            /// Having a delegate handle the cost evaluation lets different Pawns decide their movement strategy differently.
            /// </summary>
            /// <param name="point">Point to move to. Does not necessarily need to be adjacent to this WalkNode.</param>
            /// <returns>Cost of moving to point</returns>
            public delegate int EdgeCostLookup(Point point);

            /// <summary>Point on the Board</summary>
            private Point _point;
            /// <summary>Reference to the Board</summary>
            private Board _board;
            /// <summary>List of adjacent, non-Wall, in-bounds Points on the Board</summary>
            private List<WalkNode> _neighbors = new List<WalkNode>();
            /// <summary>Edge cost callback</summary>
            private EdgeCostLookup _edgeCostLookup;

            /// <summary>
            /// Construct a new WalkNode
            /// </summary>
            /// <param name="board"></param>
            /// <param name="point"></param>
            /// <param name="lookup"></param>
            public WalkNode(Board board, Point point, EdgeCostLookup lookup) {
                if (board == null) { throw new ArgumentNullException("board"); }
                if (lookup == null) { throw new ArgumentNullException("lookup"); }

                _board = board;
                _point = point;
                _edgeCostLookup = lookup;
            }

            public override Point GetData() {
                return _point;
            }

            /// <summary>
            /// Returns the cost of moving from the current WalkNode to an adjacent WalkNode
            /// </summary>
            /// <param name="neighbor">Another WalkNode. Doesn't actually have to be adjacent to this.</param>
            /// <returns></returns>
            public override int GetEdgeCost(GraphNode<Point> neighbor) {
                Point toPoint = neighbor.GetData();
                return _edgeCostLookup(toPoint);
            }

            /// <summary>
            /// Iterates through adjacent Points.  To avoid having to construct the entire Board as a graph,
            /// this function acts as an iterator, and creates neighbor WalkNodes only as needed.
            /// </summary>
            /// <returns>An IEnumerable that iterates through this WalkNode's neighboring Points</returns>
            public override IEnumerable<GraphNode<Point>> GetNeighbors() {
                for (int i = 0; i < Point.OctoNeighbors.Length; i++) {
                    Point neighborPoint = _point + Point.Neighbors[i];

                    if (_board.InBounds(neighborPoint) && _board.GetTile(neighborPoint) != Board.WALL_TILE) {
                        // Check if a WalkNode already exists for the Point
                        WalkNode neighbor = _neighbors.Find(node => node._point.Equals(neighborPoint));
                        if (neighbor == null) {
                            // If no WalkNode exists, create one
                            neighbor = new WalkNode(_board, neighborPoint, _edgeCostLookup);
                            _neighbors.Add(neighbor);

                            yield return neighbor;
                        } else {
                            // If a WalkNode exists, return the stored one
                            yield return neighbor;
                        }
                    }
                }
            }
        }

        private int _cost = 0;
        public override int ActionPoints {
            get { return _cost; }
        }

        private Point _destination;
        public Point Destination { get { return _destination; } }

        private List<Point> _path;
        private Board _board;

        public Walk(Combatant owner)
            : base(owner, "Walk", "Move across tiles within a limited range") {
            _board = Owner.Board;
        }

        public void SetDestination(Point destination) {
            if (_board.GetTile(destination) != Board.WALL_TILE) {
                _destination = destination;
                if (Point.TaxicabDistance(Owner.Position, destination) == 1) {
                    _path = new List<Point>();
                    _path.Add(destination);
                } else {
                    _path = new List<Point>(
                        Pathfinder<Point>.FindPath(
                            new WalkNode(_board, Owner.Position, TileCost),
                            new WalkNode(_board, destination, TileCost),
                            Point.TaxicabDistance
                        )
                    );
                }

                _cost = 0;
                for (int i = 0; i < _path.Count; i++) {
                    _cost += TileCost(_path[i]);
                }

                SetPostcondition();
            } else {
                _destination = Owner.Position;
            }
        }

        public ReadOnlyCollection<Point> GetPath() {
            return _path.AsReadOnly();
        }

        public override void Fire() {
            if (_path != null) {
                Actions.Move move;
                for (int i = 0; i < _path.Count; i++) {
                    move = new Actions.Move(Owner, _path[i], true);
                    Owner.PushAction(move);
                }
            }
        }

        protected void SetPostcondition() {
            _postCondition.SetStateValue(BoardState.CombatantKey(Owner, "position"), Destination);
        }

        protected virtual int TileCost(Point point) {
            int cost = 1;
            return cost;
        }

        public override int GetDecisionCost() {
            return ActionPoints;
        }
    }

}
