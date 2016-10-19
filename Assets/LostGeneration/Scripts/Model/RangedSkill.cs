using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class RangedSkill : AreaOfEffectSkill {
        public virtual Point Target {
            get { return _target; }
            set {
                if (_target != value) {
                    _target = value;
                    if (TargetChanged != null) {
                        TargetChanged(_target);
                    }
                }
            }
        }

        public event Action<Point> TargetChanged;

        private Point _target;

        public RangedSkill(Combatant owner, string name, string description)
            : base(owner, name, description) {
        }

        public abstract IEnumerable<Point> GetRange(Point origin);
        public virtual IEnumerable<Point> GetRange() {
            return GetRange(Owner.Position);
        }
        public abstract bool InRange(Point point);
        public virtual IEnumerable<Point> GetAreaOfEffect() {
            return GetAreaOfEffect(Target);
        }

        public abstract IEnumerable<Point> GetPath();
    }
}
