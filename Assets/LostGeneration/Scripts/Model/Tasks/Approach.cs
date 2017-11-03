using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen.Task {
    // Rolls downhill until it hits a local minima
    public class Approach : PrimitiveTask {
        private Pawn _pawn;
        private ActionPoints _actionPoints;
        private LongWalkSkill _moveSkill;
        private Queue<MoveAction> _moves = null;
        private MoveAction _nextMove = null; 

        private Pawn _target;
        private DijkstraMap _approachMap;
        private string _inRangeKey;

        public override WorldState Preconditions { get; }
        public override WorldState Postconditions { get; }

        public Approach(Pawn pawn, Pawn target, DijkstraMap approachMap) {
            _pawn = pawn;
            _approachMap = approachMap;
            _target = target;
            _moveSkill = _pawn.GetComponent<LongWalkSkill>();

            _inRangeKey = pawn.InstanceID + " in range of target";
            Preconditions = new WorldState() {
                { _inRangeKey, false } 
            };
            Postconditions = new WorldState() {
                { _inRangeKey, true }
            };
        }

        public override bool IsValid() {
            return _nextMove == null ||
                   _pawn.Board.IsSolid(_nextMove.End);
        }

        public override IEnumerator Do(WorldState start, WorldState goal) {
            HashSet<Point> range = new HashSet<Point>(_moveSkill.GetAreaOfEffect());
            // Roll downhill until out of range
            Point target = _approachMap.DownhillUntil(_pawn.Position, p => !range.Contains(p))
                                       .Last();
            // Set the target, then fire the skill to obtain the movement actions
            _moveSkill.SetTarget(target);
            _moves = new Queue<MoveAction>(_moveSkill.Fire().Cast<MoveAction>());

            // Every time the planner evaluates this Task, push the actions until all actions for that turn are
            // depleted, or we run out of actions to push.
            while (_moves.Count > 0) {
                _nextMove = _moves.Dequeue();
                if (_actionPoints.Current - _nextMove.Cost > 0) {
                    _pawn.PushAction(_nextMove);
                } else {
                    yield return true;
                }
            }
        }
    }
}