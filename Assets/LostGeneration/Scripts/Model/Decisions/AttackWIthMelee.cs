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

        public AttackWithMelee(Combatant source) {
            _source = source;
            _melee = _source.GetSkill<MeleeAttackSkill>();
        }

        public StateOffset ApplyPostconditions(StateOffset state) {
            int targetHealth = state.Get(StateKey.Health(_target), _target.Health);
            int damage = state.Get(StateOffset.CombatantKey(_target, "attack"), _source.EffectiveStats.Attack);

            state.Set(StateOffset.CombatantHealthKey(_target), targetHealth - damage);

            return state;
        }

        public bool ArePreconditionsMet(StateOffset state) {
            Point _targetPos = state.Get(StateKey.Position(_target.Pawn), _target.Pawn.Position);
            return _melee.InFullAreaOfEffect(_targetPos);
        }

        public void Run() {
            CardinalDirection direction;
            bool directionFound = false;
            for (direction = CardinalDirection.East; !directionFound && direction < CardinalDirection.Count; direction++) {
                _melee.SetDirection(direction); // TODO: undo this
                directionFound = _melee.GetAreaOfEffect().Contains(_target.Pawn.Position);
            }
            _melee.Fire();
        }
    }
}
