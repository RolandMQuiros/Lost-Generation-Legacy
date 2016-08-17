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
        protected int _cost = 0;
        public override int ActionPoints {
            get { return _cost; }
        }

        protected Point _destination;
        public Point Destination { get { return _destination; } }

        private List<Point> _path;
        protected Board _board;

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
                            new Board.Node(_board, Owner.Position, TileCost),
                            new Board.Node(_board, destination, TileCost),
                            Heuristic
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

        protected virtual int Heuristic(Point start, Point end) {
            return Point.TaxicabDistance(start, end);
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
