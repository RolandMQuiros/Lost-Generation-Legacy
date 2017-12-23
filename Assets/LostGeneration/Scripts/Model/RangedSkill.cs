using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public abstract class RangedSkill : AreaOfEffectSkill {
        public virtual Point Target {
            get { return _target; }
        }
        public event Action<Point, Point> TargetChanged;
        private Point _target;

        public RangedSkill(string name, string description)
        : base(name, description) { }

        public abstract IEnumerable<Point> GetRange();
        public abstract bool InRange(Point point);
        public abstract override IEnumerable<Point> GetAreaOfEffect();
        public abstract IEnumerable<Point> GetPath();

        public virtual bool SetTarget(Point target) {
            bool targetChanged = false;
            if (_target != target && InRange(target)) {
                Point oldTarget = _target;
                _target = target;

                targetChanged = true;
                InvokeAreaOfEffectChange();

                if (TargetChanged != null) {
                    TargetChanged(oldTarget, _target);
                }
            }
            return targetChanged;
        }
    }
}
