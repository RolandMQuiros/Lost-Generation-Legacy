using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public class Combatant : PawnComponent {
        #region Stats
        public ActionPoints ActionPoints { get { return _actionPoints; } }
        public PawnStats Stats { get { return _stats; } }
        public Health Health { get { return _health; } }

        public Team Team;
        #endregion Stats

        #region PrivateMembers
        private bool _didStatsChange;
        private int _queueCost;
        
        private ActionPoints _actionPoints;
        private PawnStats _stats;
        private Health _health;        
        #endregion PrivateMembers

        #region PawnOverrides
        public override void Start() {
            _actionPoints = Pawn.RequireComponent<ActionPoints>();
            _health = Pawn.RequireComponent<Health>();
            _stats = Pawn.RequireComponent<PawnStats>();

            _health.Maximum = _stats.Base.Health;
            // _supplies = Pawn.RequireComponent<Supplies>();
            // _loadout = Pawn.RequireComponent<Loadout>();
        }

        public override void OnActionInterrupted(PawnAction action) {
            Pawn.ClearActions();
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