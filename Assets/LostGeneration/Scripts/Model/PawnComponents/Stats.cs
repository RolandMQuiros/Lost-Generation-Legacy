using System.Collections.Generic;

namespace LostGen {
    public class PawnStats : PawnComponent {
        public Stats Base {
            get { return _baseStats; }
            set {
                _baseStats = value;
                _didStatsChange = true;
            }
        }
        public Stats Effective {
            get {
                if (_didStatsChange) {
                    Stats newStats = _baseStats;    
                    for (int i = 0; i < _modifiers.Count; i++) {
                        newStats += _modifiers[i];
                    }
                    _effectiveStats = newStats;
                }

                return _effectiveStats;
            }
        }

        private Stats _baseStats;
        private Stats _effectiveStats;
        private bool _didStatsChange = false;
        private List<Stats> _modifiers = new List<Stats>();

        public PawnStats(Stats? baseStats) {
            _baseStats = baseStats ?? new Stats();
        }

        public void AddModifier(Stats modifier) {
            _modifiers.Add(modifier);
            _didStatsChange = true;
        }

        public void ClearModifiers() {
            _modifiers.Clear();
            _didStatsChange = true;
        }
    }
}