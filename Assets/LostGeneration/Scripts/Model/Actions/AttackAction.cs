using System;
using System.Collections.Generic;

namespace LostGen {
    public class AttackAction : PawnAction {
        public override int Cost { get { return 2; } } // TODO: Change this shit when Inventory is implemented
        private List<Point> _areaOfEffect;

        public AttackAction(Pawn source, Point point)
            : base(source) {
            _areaOfEffect = new List<Point>();
            _areaOfEffect.Add(point);
        }

        public AttackAction(Pawn source, IEnumerable<Point> area)
            :base(source) {
            _areaOfEffect = new List<Point>(area);
        }

        public override void Commit() {
            for (int i = 0; i < _areaOfEffect.Count; i++) {
                int damage = Owner.RequireComponent<PawnStats>().Effective.Attack;

                // Apply damage to all Pawns in the area of effect
                // Friendly fire is always on!
                foreach (Pawn pawn in Owner.Board.Pawns.At(_areaOfEffect[i])) {
                    Health targetHealth = pawn.GetComponent<Health>();
                    if (targetHealth != null) {
                        targetHealth.Current -= damage;

                        Owner.PushMessage(new DamageMessage(Owner, pawn, damage));
                    }
                }

            }
        }
    }
}
