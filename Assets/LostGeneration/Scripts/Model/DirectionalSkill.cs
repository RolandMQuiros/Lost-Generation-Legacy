using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DirectionalSkill : AreaOfEffectSkill {
        public CardinalDirection Direction { get; set; }
        public DirectionalSkill(Combatant owner, string name, string description) : base(owner, name, description) { }
        public abstract IEnumerable<Point> GetAreaOfEffect(CardinalDirection direction, Point target);
        public override IEnumerable<Point> GetAreaOfEffect(Point target) {
            return GetAreaOfEffect(Direction, target);
        }
        public virtual IEnumerable<Point> GetAreaOfEffect(CardinalDirection direction) {
            return GetAreaOfEffect(direction, Owner.Position);
        }
        public virtual IEnumerable<Point> GetAreaOfEffect() {
            return GetAreaOfEffect(Direction, Owner.Position);
        }
    }
}
