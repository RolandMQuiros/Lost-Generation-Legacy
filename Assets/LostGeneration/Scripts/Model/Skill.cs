using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class Skill : IEquatable<Skill> {
        public string Name;
        public string Description;

        public Combatant Owner { get; private set; }
        public virtual int ActionPoints { get; protected set; }

        public Skill(Combatant owner, string name, string description, int actionPoints = 0) {
            Owner = owner;
            Name = name;
            Description = description;
            ActionPoints = actionPoints;
        }

        public abstract void Fire();

        /// <summary>
        /// Edge cost in AI planning system.  Calculated using a variety of metrics, specifically the
        /// combatant's personality values and action point cost.
        /// </summary>
        /// <returns></returns>
        public abstract int GetDecisionCost();

        public bool Equals(Skill other) {
            return Name.Equals(other);
        }
    }
}
