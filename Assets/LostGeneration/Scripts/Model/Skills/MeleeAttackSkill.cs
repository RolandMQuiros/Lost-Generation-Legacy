using System;
using System.Collections.Generic;

namespace LostGen {
    public class MeleeAttackSkill : RangedSkill {
        private Combatant _attacker;
        private List<Point> _areaOfEffect;
        private Dictionary<CardinalDirection, Point[]> _transforms = new Dictionary<CardinalDirection, Point[]>();
        private HashSet<Point> _fullAreaOfEffect = new HashSet<Point>();
        public CardinalDirection Direction;

        /// <summary>
        /// Contruct a new MeleeAttackSkill.
        /// </summary>
        /// <param name="attacker">Reference to Attacking Combatant</param>
        /// <param name="areaOfEffect">
        /// Collection of Point offsets indicating which tiles around the attacker are affected by the attack.
        /// These offsets are rotated based on this skill's Direction attribute, and are defined based on the
        /// attacker facing east.
        /// </param>
        public MeleeAttackSkill(Combatant attacker, IEnumerable<Point> areaOfEffect = null)
            : base(attacker, "Melee Attack", "Attack an adjacent space") {
            _attacker = attacker;

            if (areaOfEffect == null) {
                _areaOfEffect = new List<Point>();
            } else {
                _areaOfEffect = new List<Point>(areaOfEffect);
            }

            for (CardinalDirection d = CardinalDirection.East; d < CardinalDirection.Count; d++) {
                _transforms[d] = new Point[_areaOfEffect.Count];
            }

            for (int i = 0; i < _areaOfEffect.Count; i++) {
                Point east = _areaOfEffect[i];
                Point south = new Point(-east.Y, east.X);
                Point west = new Point(-east.X, -east.Y);
                Point north = new Point(east.Y, east.X);

                _transforms[CardinalDirection.East][i] = east;
                _transforms[CardinalDirection.South][i] = south;
                _transforms[CardinalDirection.West][i] = west;
                _transforms[CardinalDirection.North][i] = north;

                _fullAreaOfEffect.Add(east);
                _fullAreaOfEffect.Add(south);
                _fullAreaOfEffect.Add(west);
                _fullAreaOfEffect.Add(north);
            }
        }

        public IEnumerable<Point> GetAreaOfEffect(CardinalDirection direction) {
            return _transforms[direction];
        }

        public bool InAreaOfEffect(CardinalDirection direction, Point point) {
            bool found = false;
            for (int i = 0; !found && i < _transforms[direction].Length; i++) {
                found = _transforms[direction][i].Equals(_attacker.Position - point);
            }
            return found;
        }

        public bool InFullAreaOfEffect(Point point) {
            Point offset = point - _attacker.Position;
            return _fullAreaOfEffect.Contains(offset);
        }

        public override HashSet<Point> GetRange() {
            return new HashSet<Point> {
                _attacker.Position + Point.Neighbors[0],
                _attacker.Position + Point.Neighbors[1],
                _attacker.Position + Point.Neighbors[2],
                _attacker.Position + Point.Neighbors[3]
            };
        }

        public override void Fire() {
            AttackAction attack = null;
            for (int i = 0; i < _transforms[Direction].Length; i++) {
                attack = new AttackAction(_attacker, _transforms[Direction]);
                _attacker.PushAction(attack);
            }
        }
    }
}
