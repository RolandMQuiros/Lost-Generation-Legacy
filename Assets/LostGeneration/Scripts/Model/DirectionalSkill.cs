using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class DirectionalSkill : AreaOfEffectSkill {
        public CardinalDirection Direction {
            get { return _direction; }
        }
        public event Action<CardinalDirection> DirectionChanged;
        private CardinalDirection _direction;

        public DirectionalSkill(Combatant owner, string name, string description) : base(owner, name, description) { }

        public void SetDirection(CardinalDirection direction) {
            if (_direction != direction) {
                _direction = direction;
                if (DirectionChanged != null) {
                    DirectionChanged(_direction);
                }
            }
        }
    }
}
