using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class RangedSkill : ISkill {
        public Combatant Owner { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public virtual int ActionPoints { get; set; }

        public RangedSkill(Combatant owner, string name, string description) {
            Owner = owner;
            Name = name;
            Description = description;
        }

        public abstract HashSet<Point> GetRange();
        public abstract void Fire();
    }
}
