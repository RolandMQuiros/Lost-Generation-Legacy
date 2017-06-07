using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class RangedSkill : AreaOfEffectSkill {
        public virtual Point Target {
            get { return _target; }
        }
        public event Action<Point> TargetChanged;
        private Point _target;

        public RangedSkill(Combatant owner, string name, string description)
            : base(owner, name, description) {
        }

        public abstract IEnumerable<Point> GetRange();
        public abstract bool InRange(Point point);
        public abstract override IEnumerable<Point> GetAreaOfEffect();
        public abstract IEnumerable<Point> GetPath();

        public void SetTarget(Point target) {
            if (_target != target) {
                _target = target;
                InvokeAreaOfEffectChange();
                if (TargetChanged != null) {
                    TargetChanged(_target);
                }
            }
        }
    }
}
