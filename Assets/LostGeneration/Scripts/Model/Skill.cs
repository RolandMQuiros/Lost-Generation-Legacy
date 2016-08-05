using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class Skill {
        public string Name;
        public string Description;

        public Combatant Owner { get; private set; }
        public virtual int Cost { get; protected set; }

        public Skill(Combatant owner, string name, string description, int cost = 0) {
            Owner = owner;
            Name = name;
            Description = description;
            Cost = cost;
        }

        public abstract void Fire();
    }
}
