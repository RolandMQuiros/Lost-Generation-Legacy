using System;
using System.Collections.Generic;

namespace LostGen {
    public class AttackAction : PawnAction {
        public override int Cost { get { return 3; } } // TODO: Change this shit when Inventory is implemented
        private List<Point> _areaOfEffect;
        private Combatant _combatant;

        public AttackAction(Pawn source, Point point)
            : base(source) {
            _areaOfEffect = new List<Point>();
            _areaOfEffect.Add(point);
            _combatant = Owner.GetComponent<Combatant>();
        }

        public AttackAction(Pawn source, IEnumerable<Point> area)
            :base(source) {
            _areaOfEffect = new List<Point>(area);
        }

        public override void Commit() {
            for (int i = 0; i < _areaOfEffect.Count; i++) {
                int damage = _combatant.EffectiveStats.Attack;

                // Apply damage to all combatants in the area of effect
                // Friendly fire is always on!
                foreach (Pawn pawn in Owner.Board.PawnsAt(_areaOfEffect[i])) {
                    Combatant target = pawn.GetComponent<Combatant>();
                    if (target != null) {
                        target.Health = target.Health - damage;

                        Owner.PushMessage(new DamageMessage(Owner, target.Pawn, damage));
                    }
                }

            }
        }
    }
}
