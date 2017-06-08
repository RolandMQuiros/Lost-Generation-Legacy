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
    public class LongWalkSkill : RangedSkill {
        public override int ActionPoints { get { return _actionPoints; } }
        public bool CanWalkDiagonally = true;

        protected Board _board;
        private int _actionPoints = 0;

        private Point _prevOrigin;
        private HashSet<Point> _range;
        private Combatant _combatant;

        public LongWalkSkill(Pawn owner)
            : base(owner, "Walk", "Move across tiles within a limited range") {
            _board = Owner.Board;
            _combatant = Owner.GetComponent<Combatant>();
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
            List<WalkNode> nodePath = FindPath(Owner.Position, Target);
            List<Point> path = new List<Point>();

            if (nodePath != null) {
                for (int i = 1; i < nodePath.Count; i++) {
                    path.Add(nodePath[i].Point);
                }
            }

            return path;
        }

        #endregion PointCollections

        public override PawnAction Fire() {
            MoveAction move = null;
            List<WalkNode> path = FindPath(Owner.Position, Target);

            if (path != null) {
                Debug.Log("Repathed");
                for (int i = 1; i < path.Count; i++) {
                    move = new MoveAction(Owner, path[i - 1].Point, path[i].Point, true);
                    Owner.PushAction(move);
                }

                // Clear the range for the next step
                _range = null;
            }

            return move;
        }

        public override string ToString() {
            return "WalkSkill : { Owner: " + Owner + ", Range: " + _range.Count + "}";
        }

        private List<WalkNode> FindPath(Point origin, Point target) {
            WalkNode end = new WalkNode(_board, target, CanWalkDiagonally);
            List<WalkNode> path = null;

            if (end != null) {
                WalkNode start = new WalkNode(_board, target, CanWalkDiagonally);

                if (start == null) {
                    throw new Exception("This Skill's owner is positioned outside the graph");
                }

                if (Point.TaxicabDistance(Owner.Position, target) == 1) {
                    path = new List<WalkNode>();
                    path.Add(new WalkNode(_board, origin, CanWalkDiagonally));
                    path.Add(new WalkNode(_board, target, CanWalkDiagonally));
                } else {
                    path = new List<WalkNode>(
                        GraphMethods<WalkNode>.FindPath(
                            new WalkNode(_board, origin, CanWalkDiagonally),
                            end,
                            Heuristic
                        )
                    );
                }
                
                _actionPoints = 0;
                for (int i = 1; i < path.Count; i++) {
                    _actionPoints += ActionCostBetweenNodes(path[i-1], path[i]);
                }
            }

            return path;
        }

        private void ReinitializeRange(Point origin) {
            // If the cached range collection doesn't exist yet, or the origin has changed,
            // then create the range
            if (_range == null || _prevOrigin != origin) {
                WalkNode startNode = new WalkNode(Owner.Board, origin, CanWalkDiagonally);
                _prevOrigin = origin;
                _range = new HashSet<Point>();
                if (startNode != null) {
                    // GraphMethods include the origin in their result sets.  Make sure we skip the first one. 
                    bool firstSkipped = false;
                    foreach (WalkNode node in GraphMethods<WalkNode>.FloodFill(startNode, _combatant.ActionPoints + 1)) {
                        if (firstSkipped) {
                            _range.Add(node.Point);
                        } else {
                            firstSkipped = true;
                        }
                    }
                }
            }
        }
        
        /// <summary>Retrieves both the ActionPoint and Pathfinding cost between two nodes</summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private int ActionCostBetweenNodes(BlockNode from, BlockNode to) {
            int actionPoints = Math.Abs(to.Point.X - from.Point.Y);
            int heightDifference = to.Point.Y - from.Point.Y;
            
            // Climbing costs 2 points per block. Dropping only costs the horizontal component.
            if (heightDifference > 0) {
                actionPoints += heightDifference * 2;
            }

            return actionPoints;
        }

        protected virtual int Heuristic(WalkNode start, WalkNode end) {
            return Point.TaxicabDistance(start.Point, end.Point);
        }
    }
}
