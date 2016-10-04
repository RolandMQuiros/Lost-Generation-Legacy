using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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

        private HashSet<Point> _range;

        public WalkSkill(Combatant owner)
            : base(owner, "Walk", "Move across tiles within a limited range") {
            _board = Owner.Board;
        }

        /// <summary>
        /// Only use this for debugging
        /// </summary>
        /// <returns></returns>
        public Queue<Point> GetPath() {
            Queue<Point> pointPath = new Queue<Point>();
            List<Board.Node> nodePath = FindPath();
            for (int i = 0; i < nodePath.Count; i++) {
                pointPath.Enqueue(nodePath[i].Point);
            }

            return pointPath;
        }

        public override IEnumerable<Point> GetRange() {
            InitializeRange();
            return _range;
        }

        public override bool InRange(Point point) {
            InitializeRange();
            return _range.Contains(point);
        }

        public override IEnumerable<Point> GetAreaOfEffect() {
            yield return Target;
        }

        public override void Fire() {
            MoveAction move = null;
            List<Board.Node> path = FindPath();

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    move = new MoveAction(Owner, path[i].Point, true);
                    Owner.PushAction(move);
                }

                // Clear the range for the next step
                _range = null;
            }
        }

        private List<Board.Node> FindPath() {
            Board.Node end = _board.GetNode(Target, TileCost);
            List<Board.Node> path = null;

            if (end != null) {
                Board.Node start = _board.GetNode(Owner.Position, TileCost);

                if (start == null) {
                    throw new Exception("This Skill's owner is positioned outside the graph");
                }

                if (Point.TaxicabDistance(Owner.Position, Target) == 1) {
                    path = new List<Board.Node>();
                    path.Add(_board.GetNode(Target, TileCost));
                } else {
                    path = new List<Board.Node>(
                        GraphMethods<Board.Node>.FindPath(
                            new Board.Node(_board, Owner.Position, TileCost),
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

        private void InitializeRange() {
            if (_range == null) {
                _range = new HashSet<Point>();
                Board.Node startNode = Owner.Board.GetNode(Owner.Position, TileCost);
                foreach (Board.Node node in GraphMethods<Board.Node>.FloodFill(startNode, Owner.ActionPoints)) {
                    _range.Add(node.Point);
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
