using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DirectionalSkill : AreaOfEffectSkill {
        public CardinalDirection Direction {
            get { return _direction; }
            set {
                if (_direction != value) {
                    _direction = value;
                    if (DirectionChanged != null) {
                        DirectionChanged(_direction);
                    }
                }
            }
        }

        public event Action<CardinalDirection> DirectionChanged;

        private CardinalDirection _direction;

        public DirectionalSkill(Combatant owner, string name, string description) : base(owner, name, description) { }
        public abstract IEnumerable<Point> GetAreaOfEffect(Point origin, CardinalDirection direction);
        public override IEnumerable<Point> GetAreaOfEffect(Point origin) {
            return GetAreaOfEffect(origin, Direction);
        }
        public virtual IEnumerable<Point> GetAreaOfEffect(CardinalDirection direction) {
            return GetAreaOfEffect(Owner.Position, direction);
        }
        public virtual IEnumerable<Point> GetAreaOfEffect() {
            return GetAreaOfEffect(Owner.Position, Direction);
        }
    }
}
