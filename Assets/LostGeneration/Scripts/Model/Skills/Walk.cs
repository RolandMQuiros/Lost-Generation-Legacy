using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LostGen.Skills {

    public class Walk : Skill {
        private class WalkNode : GraphNode<Point> {
            public delegate int TileCostLookup(int tile);

            private Point _point;
            private Board _board;
            private List<WalkNode> _neighbors = new List<WalkNode>();
            private TileCostLookup _tileCostLookup;

            public WalkNode(Board board, Point point, TileCostLookup lookup) {
                if (board == null) { throw new ArgumentNullException("board"); }
                if (lookup == null) { throw new ArgumentNullException("lookup"); }

                _board = board;
                _point = point;
                _tileCostLookup = lookup;
            }

            public override Point GetData() {
                return _point;
            }

            public override int GetEdgeCost(GraphNode<Point> neighbor) {
                Point toPoint = neighbor.GetData();
                int tile = _board.GetTile(toPoint);

                return _tileCostLookup(tile);
            }

            /// <summary>
            /// Coroutine 
            /// </summary>
            /// <returns></returns>
            public override IEnumerable<GraphNode<Point>> GetNeighbors() {
                for (int i = 0; i < Point.Neighbors.Length; i++) {
                    Point neighborPoint = _point + Point.Neighbors[i];

                    if (_board.InBounds(neighborPoint) && _board.GetTile(neighborPoint) != Board.WALL_TILE) {
                        WalkNode neighbor = _neighbors.Find(node => node._point.Equals(neighborPoint));
                        if (neighbor == null) {
                            neighbor = new WalkNode(_board, neighborPoint, _tileCostLookup);
                            _neighbors.Add(neighbor);

                            yield return neighbor;
                        } else {
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
            : base(owner, "Walk", "Move across tiles within a limited range", 1) {
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
                    int tile = _board.GetTile(_path[i]);
                    _cost += TileCost(tile);
                }
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

        protected virtual int TileCost(int tile) {
            int cost = 1;
            return cost;
        }
    }

}
