using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class Skill {
        public string Name;
        public string Description;

        public Combatant Source { get; private set; }
        public virtual int Cost { get; protected set; }

        public Skill(Combatant source, int cost = 0) {
            Source = source;
            Cost = cost;
        }

        public abstract void Fire();
    }
}
