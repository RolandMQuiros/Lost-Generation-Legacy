using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class RangedSkill : AreaOfEffectSkill {
        public Point Target { get; protected set; }

        public RangedSkill(Combatant owner, string name, string description)
            : base(owner, name, description) {
        }

        public abstract IEnumerable<Point> GetRange();
    }
}
