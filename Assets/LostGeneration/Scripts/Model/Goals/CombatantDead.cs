using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Goal {
    public class CombatantDead : IGoal {
        private Combatant _target;
        private string _healthKey;

        public CombatantDead(Combatant target) {
            _target = target;
            _healthKey = BoardState.CombatantHealthKey(_target);
        }

        public bool IsAchieved(BoardState boardState) {
            int health = boardState.GetStateValue(_healthKey, _target.Health);

            return health <= 0;
        }

        public int Heuristic(BoardState from) {
            int health = from.GetStateValue(_healthKey, _target.Health);
            return health;
        }
    }
}
