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
        protected Point? _destination;
        public Point Destination { get { return _destination.Value; } }

        private List<Board.Node> _path;
        protected Board _board;

        public WalkSkill(Combatant owner)
            : base(owner, "Walk", "Move across tiles within a limited range") {
            _board = Owner.Board;
        }

        public void SetDestination(Point destination) {
            Board.Node end = _board.GetNode(destination, TileCost);

            if (end != null) {
                _destination = destination;
                Board.Node start = _board.GetNode(Owner.Position, TileCost);

                if (start == null) {
                    throw new Exception("This Skill's owner is positioned outside the graph");
                }

                if (Point.TaxicabDistance(Owner.Position, destination) == 1) {
                    _path = new List<Board.Node>();
                    _path.Add(_board.GetNode(_destination.Value, TileCost));
                } else {
                    _path = new List<Board.Node>(
                        Pathfinder<Board.Node>.FindPath(
                            new Board.Node(_board, Owner.Position, TileCost),
                            end,
                            Heuristic
                        )
                    );
                }

                ActionPoints = 0;
                for (int i = 0; i < _path.Count; i++) {
                    ActionPoints += TileCost(_path[i].Point);
                }
            } else {
                _destination = Owner.Position;
            }
        }

        public Queue<Point> GetPath() {
            if (_destination == null) {
                throw new NullReferenceException("Destination has not been specified");
            }

            Queue<Point> pointPath = new Queue<Point>();
            for (int i = 0; i < _path.Count; i++) {
                pointPath.Enqueue(_path[i].Point);
            }

            return pointPath;
        }

        public override HashSet<Point> GetRange() {
            Board.Node startNode = Owner.Board.GetNode(Owner.Position);
            HashSet<Point> range = new HashSet<Point>();
            foreach (Board.Node node in Pathfinder<Board.Node>.FloodFill(startNode, Owner.ActionPoints)) {
                range.Add(node.Point);
            }

            return range;
        }

        public override void Fire() {
            MoveAction move = null;

            if (_path != null) {
                for (int i = 0; i < _path.Count; i++) {
                    move = new MoveAction(Owner, _path[i].Point, true);
                    Owner.PushAction(move);
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
