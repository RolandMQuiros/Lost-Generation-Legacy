using System;
using System.Collections.Generic;

namespace LostGen {
    public class AttackAction : PawnAction {
        private Combatant _attacker;
        private List<Point> _areaOfEffect;

        public AttackAction(Combatant source, Point point)
            : base(source, false, true) {
            _attacker = source;
            _areaOfEffect = new List<Point>();
            _areaOfEffect.Add(point);
        }

        public AttackAction(Combatant source, IEnumerable<Point> area)
            :base(source, false, true) {
            _attacker = source;
            _areaOfEffect = new List<Point>(area);
        }

        public override void Run() {
            for (int i = 0; i < _areaOfEffect.Count; i++) {
                int damage = _attacker.EffectiveStats.Attack;

                // Apply damage to all combatants in the area of effect
                // Friendly fire is always on!
                foreach (Pawn pawn in _attacker.Board.PawnsAt(_areaOfEffect[i])) {
                    Combatant target = pawn as Combatant;
                    if (target != null) {
                        target.Health = target.Health - damage;

                        target.EmitMessage(
                            new DamageMessage(target, damage, _attacker)
                        );
                    }
                }

            }
        }
    }
}
