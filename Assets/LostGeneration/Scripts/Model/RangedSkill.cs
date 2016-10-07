using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class RangedSkill : AreaOfEffectSkill {
        public virtual Point Target { get; set; }

        private Point _target;

        public RangedSkill(Combatant owner, string name, string description)
            : base(owner, name, description) {
        }

        public abstract IEnumerable<Point> GetRange();
        public abstract bool InRange(Point point);

        public abstract IEnumerable<Point> GetPath();
    }
}
