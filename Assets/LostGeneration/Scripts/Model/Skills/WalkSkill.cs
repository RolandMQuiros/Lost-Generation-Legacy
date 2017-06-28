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
            List<Point> neighbors = new List<Point>(_neighbors);
            neighbors.Add(Owner.Position);
            return neighbors;
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

        public override PawnAction Fire()
        {
            PawnAction move = null;
            if (InRange(Target))
            {
                move = new MoveAction(Owner, Owner.Position, Target, true);
            }
            return move;
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