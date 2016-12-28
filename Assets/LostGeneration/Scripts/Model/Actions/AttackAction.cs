﻿using System;
using System.Collections.Generic;

namespace LostGen {
    public class AttackAction : CombatantAction {
        public override int ActionPoints { get { return 3; } } // TODO: Change this shit when Inventory is implemented
        public override Point PostRunPosition {
            get { return Owner.Position; }
        }
        private List<Point> _areaOfEffect;

        public AttackAction(Combatant source, Point point)
            : base(source) {
            _areaOfEffect = new List<Point>();
            _areaOfEffect.Add(point);
        }

        public AttackAction(Combatant source, IEnumerable<Point> area)
            :base(source) {
            _areaOfEffect = new List<Point>(area);
        }

        public override void Commit() {
            for (int i = 0; i < _areaOfEffect.Count; i++) {
                int damage = Owner.EffectiveStats.Attack;

                // Apply damage to all combatants in the area of effect
                // Friendly fire is always on!
                foreach (Pawn pawn in Owner.Board.PawnsAt(_areaOfEffect[i])) {
                    Combatant target = pawn as Combatant;
                    if (target != null) {
                        target.Health = target.Health - damage;

                        target.EmitMessage(
                            new DamageMessage(target, damage, Owner)
                        );
                    }
                }

            }
        }
    }
}
