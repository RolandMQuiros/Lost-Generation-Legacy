using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DirectionalSkill : AreaOfEffectSkill {
        public CardinalDirection Direction { get; set; }
        public DirectionalSkill(Combatant owner, string name, string description) : base(owner, name, description) { }
        public abstract IEnumerable<Point> GetAreaOfEffect(CardinalDirection direction);
    }
}
