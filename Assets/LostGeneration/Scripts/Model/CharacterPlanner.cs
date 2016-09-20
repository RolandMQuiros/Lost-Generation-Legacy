using UnityEngine;
using System;
using System.Collections.Generic;

namespace LostGen {
    public class CharacterController {
        private Planner _planner = new Planner(StateOffset.Heuristic);
        private Combatant _owner;
        private Combatant _target;
        private StateOffset _currentGoal;
        private Queue<IDecision> _currentPlan;

        private Decision.AttackWithMelee _attackDecision;
        private Decision.ApproachMeleeRange _approachDecision;

        public CharacterController(Combatant owner) {
            _owner = owner;
            _approachDecision = new Decision.ApproachMeleeRange(_owner);
            _planner.AddDecision(_approachDecision);
            _attackDecision = new Decision.AttackWithMelee(_owner);
            _planner.AddDecision(_attackDecision);
        }

        public void BeginTurn() {
            if (_target == null) {
                FindTarget();
            }

            if (_target != null) {
                Debug.Log("Target found");
                _approachDecision.Setup();
                _approachDecision.Run();
                /*Queue<IDecision> plan = _planner.CreatePlan(_currentGoal);
                if (plan.Count > 0) Debug.Log("Plan formulated");
                while (plan.Count > 0) {
                    CombatantDecision decision = plan.Dequeue() as CombatantDecision;
                    decision.Run();
                }*/
            }
        }

        public void Step() {

        }

        private void FindTarget() {
            foreach (Pawn pawn in _owner.GetPawnsInView()) {
                Combatant target = pawn as Combatant;
                if (target != null && target.Team.IsHostile(_owner.Team)) {
                    _target = target;
                    _currentGoal = new StateOffset();
                    _currentGoal.Set(StateKey.Health(_target), 0);
                    _attackDecision.Target = _target;
                    _approachDecision.Target = _target;
                    break;
                }
            }
        }
    }
}