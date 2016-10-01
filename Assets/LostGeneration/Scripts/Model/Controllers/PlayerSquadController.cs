using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class PlayerSquadController : IPawnController {
        public event Action<IPawnController> Ready;

        private struct SquadUnit {
            public Combatant Unit;
            public StateOffset Goal;
            public bool IsManual;
            public Planner Planner;

            public SquadUnit(Combatant unit) {
                Unit = unit;
                Goal = new StateOffset();
                IsManual = true;
                Planner = new Planner(StateOffset.Heuristic);
            }
        }

        private Dictionary<Combatant, SquadUnit> _units = new Dictionary<Combatant, SquadUnit>();

        public void AddUnit(Combatant unit) {
            if (!_units.ContainsKey(unit)) {
                _units[unit] = new SquadUnit(unit);
            }
        }

        public IEnumerable<Combatant> GetCombatants() {
            foreach (Combatant combatant in _units.Keys) {
                yield return combatant;
            }
        }

        public void BeginTurn() { }
    }
}
