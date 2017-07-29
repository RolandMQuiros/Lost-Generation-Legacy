using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : PawnComponent {
        #region Stats
        public int ActionPoints {
            get { return _actionPoints; }
            set { _actionPoints = value; }
        }
        public int ActionQueueCost { get { return _queueCost; } }

        public PawnStats Stats { get { return _stats; } }
        public Health Health { get { return _health; } }

        public Team Team;
        #endregion Stats

        #region CollectionProperties
        public IEnumerable<Gear> Gear { get { return _gear; } }
        public int GearCount { get { return _gear.Count; } }
        #endregion CollectionProperties

        #region PrivateMembers
        private bool _didStatsChange;
        private int _actionPoints;
        private int _queueCost;
        
        private PawnStats _stats;
        private Health _health;

        private List<Gear> _gear = new List<Gear>();
        
        #endregion PrivateMembers

        public void AddGear(Gear gear) {
            _gear.Add(gear);
        }

        public void RemoveGear(Gear gear) {
            _gear.Remove(gear);
        }

        #region PawnOverrides
        public override void Start() {
            _health = Pawn.RequireComponent<Health>();
            _stats = Pawn.RequireComponent<PawnStats>();
            // _supplies = Pawn.RequireComponent<Supplies>();
            // _loadout = Pawn.RequireComponent<Loadout>();
        }

        public override void OnPushAction(PawnAction action) {
            _queueCost += action.Cost;
        }

        public override void BeforeClearActions() {
            _queueCost = 0;
        }

        public override void BeginTurn() {
            _actionPoints = _stats.Effective.Stamina;
        }

        public override void PostAction(PawnAction action) {
            _actionPoints -= action.Cost;
        }

        public override void PreStep() {
            Pawn.Priority = _stats.Effective.Agility;
        }
        #endregion PawnOverrides

        private void WhenLandedUpon(Gravity by) {
            int damage = Math.Max(by.Weight - _stats.Effective.Defense, 0);
            _health.Current -= damage;
            Pawn.PushMessage(new DamageMessage(by.Pawn, Pawn, damage));
        }
    }
}