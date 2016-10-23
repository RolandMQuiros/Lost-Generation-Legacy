using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
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
    public class WalkSkill : RangedSkill {
        public override int ActionPoints { get { return _actionPoints; } }

        protected Board _board;
        private int _actionPoints;

        private Point _prevOrigin;
        private HashSet<Point> _range;

        public WalkSkill(Combatant owner)
            : base(owner, "Walk", "Move across tiles within a limited range") {
            _board = Owner.Board;
        }

        #region PointCollections

        public override IEnumerable<Point> GetRange() {
            ReinitializeRange(Owner.Position);
            return _range;
        }

        public override bool InRange(Point point) {
            ReinitializeRange(Owner.Position);
            return _range.Contains(point);
        }

        public override IEnumerable<Point> GetAreaOfEffect() {
            yield return Target;
        }

        public override IEnumerable<Point> GetPath() {
            List<Board.Node> nodePath = FindPath(Owner.Position, Target);
            List<Point> path = new List<Point>();

            if (nodePath != null) {
                for (int i = 0; i < nodePath.Count; i++) {
                    path.Add(nodePath[i].Point);
                }
            }

            return path;
        }

        #endregion

        public override void Fire() {
            MoveAction move = null;
            List<Board.Node> path = FindPath(Owner.Position, Target);

            if (path != null) {
                Debug.Log("Repathed");
                for (int i = 0; i < path.Count; i++) {
                    move = new MoveAction(Owner, path[i].Point, true);
                    Owner.PushAction(move);
                }

                // Clear the range for the next step
                _range = null;
            }
        }

        private List<Board.Node> FindPath(Point origin, Point target) {
            Board.Node end = _board.GetNode(target, TileCost);
            List<Board.Node> path = null;

            if (end != null) {
                Board.Node start = _board.GetNode(origin, TileCost);

                if (start == null) {
                    throw new Exception("This Skill's owner is positioned outside the graph");
                }

                if (Point.TaxicabDistance(Owner.Position, target) == 1) {
                    path = new List<Board.Node>();
                    path.Add(_board.GetNode(target, TileCost));
                } else {
                    path = new List<Board.Node>(
                        GraphMethods<Board.Node>.FindPath(
                            new Board.Node(_board, origin, TileCost),
                            end,
                            Heuristic
                        )
                    );
                }

                _actionPoints = 0;
                for (int i = 0; i < path.Count; i++) {
                    _actionPoints += TileCost(path[i].Point);
                }
            }

            return path;
        }

        private void ReinitializeRange(Point origin) {
            if (_range == null || _prevOrigin != origin) {
                Board.Node startNode = Owner.Board.GetNode(origin, TileCost);
                _prevOrigin = origin;
                _range = new HashSet<Point>();
                if (startNode != null) {
                    foreach (Board.Node node in GraphMethods<Board.Node>.FloodFill(startNode, Owner.ActionPoints)) {
                        _range.Add(node.Point);
                    }
                }
            }
        }

        protected virtual int Heuristic(Board.Node start, Board.Node end) {
            return Point.TaxicabDistance(start.Point, end.Point);
        }

        protected virtual int TileCost(Point point) {
            int cost = 1;
            return cost;
        }
    }
}
