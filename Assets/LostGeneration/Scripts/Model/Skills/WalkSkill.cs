using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen
{
    public class WalkSkill : RangedSkill
    {
        public override int ActionPoints
        {
            get
            {
                return Math.Abs(Target.X - Owner.Position.X) +
                       Math.Abs(Target.Z - Owner.Position.Z) +
                       Math.Max(Target.Y - Owner.Position.Y, 0); 
            }
        }

        private WalkNode _walkNode;
        private HashSet<Point> _neighbors;

        public WalkSkill(Pawn owner)
        : base(owner, "Walk", "Move to an adjacent tile") { }

        public override IEnumerable<Point> GetRange()
        {
            BuildRange();
            return _neighbors;
        }

        public override bool InRange(Point point)
        {
            BuildRange();
            _neighbors.Contains(point);
            return false;
        }

        public override IEnumerable<Point> GetAreaOfEffect()
        {
            yield return Target;
        }

        public override IEnumerable<Point> GetPath()
        {
            yield return Target;
        }

        public override PawnAction Fire()
        {
            if (!InRange(Target))
            {
                throw new IndexOutOfRangeException("WalkSkill's target is outside its range");
            }
            return new MoveAction(Owner, Owner.Position, Target, true);
        }

        private void BuildRange()
        {
            if (_walkNode == null || _walkNode.Point != Owner.Position)
            {
                _walkNode = new WalkNode(Owner.Board, Owner.Position, true);
                _neighbors = new HashSet<Point>
                (
                    _walkNode.GetNeighbors().Cast<WalkNode>()
                                            .Select(x => x.Point)
                );
            }
        }
    }
}