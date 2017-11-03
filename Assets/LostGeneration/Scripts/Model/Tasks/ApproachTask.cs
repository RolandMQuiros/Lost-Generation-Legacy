using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    // Rolls downhill until it hits a local minima
    public class ApproachTask : PrimitiveTask {
        private Pawn _pawn;
        private ActionPoints _actionPoints;
        private LongWalkSkill _moveSkill;

        private Pawn _target;
        private DijkstraMap _approachMap;
        private string _inRangeKey;

        public override WorldState Preconditions { get; }
        public override WorldState Postconditions { get; }

        public ApproachTask(Pawn pawn, DijkstraMap approachMap) {
            _pawn = pawn;
            _approachMap = approachMap;

            _actionPoints = _pawn.RequireComponent<ActionPoints>();
            _moveSkill = _pawn.RequireComponent<LongWalkSkill>();

            _inRangeKey = pawn.InstanceID + " in range of target";
            Preconditions = new WorldState() {
                { _inRangeKey, false } 
            };
            Postconditions = new WorldState() {
                { _inRangeKey, true }
            };
        }

        public override bool IsValid() {
            return true;
        }

        public override IEnumerator Do() {
            // Roll downhill until out of range
            Queue<Point> path = new Queue<Point>(_approachMap.Downhill(_pawn.Position));
            if (path.Count == 0) {
                yield break;
            }
            Point destination = path.Last();
            int destinationValue = _approachMap.GetValue(destination);

            // Every time the planner evaluates this Task, push the actions until all actions for that turn are
            // depleted, or we run out of actions to push.
            Point previous = _pawn.Position;
            int runningCost = 0;
            while (path.Count > 0) {
                MoveAction nextMove = new MoveAction(
                    _pawn,
                    previous,
                    path.Dequeue(),
                    1,
                    true
                );
                previous = nextMove.End;
                runningCost += nextMove.Cost;

                // Check if the current plan is still viable
                if (_pawn.Board.IsSolid(nextMove.End) ||                      // Fail if the next move is blocked by something
                    destinationValue < _approachMap.GetValue(destination)) {  // Fail if the Dijkstra map value has increased since the last call
                    yield return null;
                } else if (_actionPoints.Current - runningCost > 0) {
                    _pawn.PushAction(nextMove);
                } else {
                    yield return true;
                    runningCost = 0;
                }
            }
        }
    }
}