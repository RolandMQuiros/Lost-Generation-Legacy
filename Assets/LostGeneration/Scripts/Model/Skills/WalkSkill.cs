using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen
{
    public class WalkSkill : RangedSkill
    {
        public override int ActionPoints
        {
            get { return Math.Max(Target.Y - Pawn.Position.Y, 1); }
        }

        private WalkNode _walkNode;
        private HashSet<Point> _neighbors;

        public WalkSkill(Pawn owner)
        : base(owner, "Walk", "Move to an adjacent tile") { }

        public override IEnumerable<Point> GetRange() {
            yield return Pawn.Position;
            BuildRange();
            foreach (Point point in _neighbors) {
                yield return point;
            }
        }

        public override bool InRange(Point point)
        {
            BuildRange();
            return _neighbors.Contains(point);
        }

        public override IEnumerable<Point> GetAreaOfEffect()
        {
            yield return Target;
        }

        public override IEnumerable<Point> GetPath()
        {
            return Enumerable.Empty<Point>();
        }

        public override IEnumerable<PawnAction> Fire()
        {
            PawnAction move = null;
            if (InRange(Target))
            {
                move = new MoveAction(Pawn, Pawn.Position, Target, MoveCost(Pawn.Position, Target), true);
            }
            yield return move;
        }

        public static int MoveCost(Point from, Point to) {
            return Point.ChebyshevDistance(from.XZ, to.XZ) + Math.Abs(to.Y - from.Y);
        }

        private void BuildRange()
        {
            if (_walkNode == null || _walkNode.Point != Pawn.Position)
            {
                _walkNode = new WalkNode(Pawn.Board, Pawn.Position, false, true);
                ActionPoints actionPoints = Pawn.GetComponent<ActionPoints>();
                _neighbors = new HashSet<Point>
                (
                    _walkNode.GetNeighbors().Cast<WalkNode>()
                                            .Select(x => x.Point)
                                            .Where(point => // Filter out nodes that cost too much to move to
                                                actionPoints == null ||
                                                MoveCost(Pawn.Position, point) <= actionPoints.Current
                                            )
                );
            }
        }
    }
}