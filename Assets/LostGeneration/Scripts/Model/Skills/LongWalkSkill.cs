using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen.Model {
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
    public class LongWalkSkill : RangedSkill {
        public override int ActionPoints { get { return 0; } }
        public override bool IsUsable {
            get {
                WalkNode currentNode = new WalkNode(_board, Pawn.Position, CanWalkDiagonally, true);
                return
                    currentNode
                        .GetNeighbors()
                        .Where(n => currentNode.GetEdgeCost(n) <= _ownerPoints.Current)
                        .Any();
            }
        }
        public bool CanWalkDiagonally = true;

        protected Board _board;
        private int _actionPoints = 0;

        private Point _prevOrigin;
        private HashSet<Point> _range;
        private List<Point> _path;
        private ActionPoints _ownerPoints;

        public LongWalkSkill()
        : base("Walk", "Move across tiles within a limited range") { }

        protected override void Awake() {
            _board = Pawn.Board;
            _ownerPoints = Pawn.RequireComponent<ActionPoints>();
        }

        #region PointCollections

        public override IEnumerable<Point> GetRange() {
            ReinitializeRange(Pawn.Position);
            return _range;
        }

        public override bool InRange(Point point) {
            ReinitializeRange(Pawn.Position);
            return _range.Contains(point);
        }

        public override IEnumerable<Point> GetAreaOfEffect() {
            yield return Target;
        }

        public override IEnumerable<Point> GetPath() {
            return FindPath(Pawn.Position, Target);
        }

        #endregion PointCollections

        public override IEnumerable<PawnAction> Fire() {
            _path = FindPath(Pawn.Position, Target);
            if (_path != null) {
                for (int i = 1; i < _path.Count; i++) {
                    Point from = _path[i - 1];
                    Point to = _path[i];
                    yield return new MoveAction(Pawn, from, to, WalkNode.GetCost(from, to), true);
                }

                // Clear the range for the next step
                _range = null;
                _path = null;
            }
        }

        public override string ToString() {
            return "WalkSkill : { Owner: " + Pawn + ", Range: " + _range.Count + "}";
        }

        private List<Point> FindPath(Point origin, Point target) {
            WalkNode end = new WalkNode(_board, target, CanWalkDiagonally, true);
            List<Point> path = new List<Point>();

            if (end != null) {
                WalkNode start = new WalkNode(_board, target, CanWalkDiagonally, true);

                if (start == null) {
                    throw new Exception("This Skill's owner is positioned outside the graph");
                }

                if (Point.TaxicabDistance(Pawn.Position, target) == 1) {
                    path.Add(origin);
                    path.Add(target);
                } else {
                    path = new List<Point>(
                        GraphMethods.FindPath<BlockNode>(
                            new WalkNode(_board, origin, CanWalkDiagonally, true),
                            end,
                            Heuristic
                        ).Select(n => n.Point)
                    );
                }
                
                _actionPoints = 0;
                for (int i = 1; i < path.Count; i++) {
                    _actionPoints += WalkNode.GetCost(path[i-1], path[i]);
                }
            }

            return path;
        }

        private void ReinitializeRange(Point origin) {
            // If the cached range collection doesn't exist yet, or the origin has changed,
            // then create the range
            if (_range == null || _prevOrigin != origin) {
                WalkNode startNode = new WalkNode(Pawn.Board, origin, CanWalkDiagonally, true);
                _prevOrigin = origin;
                _range = new HashSet<Point>();
                if (startNode != null) {
                    // GraphMethods include the origin in their result sets.  Make sure we skip the first one. 
                    bool firstSkipped = false;
                    foreach (WalkNode node in GraphMethods.FloodFill<BlockNode>(startNode, _ownerPoints.Current, -1)) {
                        if (firstSkipped) {
                            _range.Add(node.Point);
                        } else {
                            firstSkipped = true;
                        }
                    }
                }
            }
        }

        protected virtual int Heuristic(BlockNode start, BlockNode end) {
            return Point.TaxicabDistance(start.Point, end.Point);
        }
    }
}
