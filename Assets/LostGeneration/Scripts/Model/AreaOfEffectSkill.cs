using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class AreaOfEffectSkill : ISkill {
        public Combatant Owner { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public virtual int ActionPoints { get; set; }
        public bool IsReadyToFire { get; set; }

        public AreaOfEffectSkill(Combatant owner, string name, string description) {
            Owner = owner;
            Name = name;
            Description = description;
        }

        public abstract IEnumerable<Point> GetAreaOfEffect(Point target);
        public abstract void Fire();
    }
}
