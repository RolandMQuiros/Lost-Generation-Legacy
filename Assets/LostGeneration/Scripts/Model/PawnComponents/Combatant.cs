using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : PawnComponent {
        #region Stats
        public ActionPoints ActionPoints { get { return _actionPoints; } }
        public PawnStats Stats { get { return _stats; } }
        public Health Health { get { return _health; } }
        public SkillSet SkillSet { get { return _skillSet; } }

        public Team Team;
        #endregion Stats

        #region CollectionProperties
        public IEnumerable<Gear> Gear { get { return _gear; } }
        public int GearCount { get { return _gear.Count; } }
        #endregion CollectionProperties

        #region PrivateMembers
        private bool _didStatsChange;
        private int _queueCost;
        
        private ActionPoints _actionPoints;
        private PawnStats _stats;
        private Health _health;
        private SkillSet _skillSet;

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
            _actionPoints = Pawn.RequireComponent<ActionPoints>();
            _health = Pawn.RequireComponent<Health>();
            _stats = Pawn.RequireComponent<PawnStats>();
            _skillSet = Pawn.RequireComponent<SkillSet>();

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