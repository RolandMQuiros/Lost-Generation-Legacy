using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : PawnComponent {
        #region Stats
        public Stats BaseStats {
            get { return _baseStats; }
            set {
                _baseStats = value;
                _didStatsChange = true;
            }
        }

        public Stats EffectiveStats {
            get {
                if (_didStatsChange) {
                    Stats newStats = _baseStats;    
                    for (int i = 0; i < _gear.Count; i++) {
                        newStats += _gear[i].Modifier;
                    }
                    _effectiveStats = newStats;

                    Pawn.Priority = _effectiveStats.Agility;
                }

                return _effectiveStats;
            }
        }
        public int Health {
            get { return _health; }
            set {
                _health = Math.Max(0, Math.Min(_effectiveStats.Health, value));
            }
        }
        public int ActionPoints {
            get { return _actionPoints; }
            set { _actionPoints = value; }
        }
        public int ActionQueueCost { get { return _queueCost; } }

        public Team Team;
        #endregion Stats

        #region CollectionProperties
        public IEnumerable<Gear> Gear { get { return _gear; } }
        public int GearCount { get { return _gear.Count; } }
        #endregion CollectionProperties

        #region PrivateMembers
        private bool _didStatsChange;
        private Stats _baseStats;
        private Stats _effectiveStats;
        private int _health;
        private int _actionPoints;
        private int _queueCost;

        private List<Gear> _gear = new List<Gear>();
        
        #endregion PrivateMembers

        public void AddGear(Gear gear) {
            _gear.Add(gear);
        }

        public void RemoveGear(Gear gear) {
            _gear.Remove(gear);
        }

        #region PawnOverrides
        public override void OnPushAction(PawnAction action) {
            _queueCost += action.Cost;
        }

        public override void BeforeClearActions() {
            _queueCost = 0;
        }

        public override void BeginTurn() {
            _actionPoints = EffectiveStats.Stamina;
        }
        #endregion PawnOverrides

        private void WhenLandedUpon(Gravity by) {
            int damage = Math.Max(by.Weight - EffectiveStats.Defense, 0);
            Health -= damage;
            Pawn.PushMessage(new DamageMessage(by.Pawn, Pawn, damage));
        }
    }
}