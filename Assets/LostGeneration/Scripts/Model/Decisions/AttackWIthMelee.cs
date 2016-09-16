using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Decision {
    public class AttackWithMelee : IDecision {
        public int Cost { get { return _melee.ActionPoints; } }
        public Combatant Target {
            get { return _target; }
            set { _target = value; }
        }

        private Combatant _source;
        private MeleeAttackSkill _melee;
        private Combatant _target;
        private CardinalDirection _direction;

        public StateOffset ApplyPostconditions(StateOffset state) {
            int targetHealth = state.Get(StateOffset.CombatantHealthKey(_target), _target.Health);
            int damage = state.Get(StateOffset.CombatantKey(_target, "attack"), _source.EffectiveStats.Attack);

            state.Set(StateOffset.CombatantHealthKey(_target), targetHealth - damage);

            return state;
        }

        public bool ArePreconditionsMet(StateOffset state) {
            Point position = state.Get(StateOffset.CombatantKey(_source, "position"), _source.Position);
            return _melee.InFullAreaOfEffect(position);
        }

        public void Run() {
            
        }

        public void Setup() {
            throw new NotImplementedException();
        }
    }
}
