using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : Pawn {
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

                    Priority = _effectiveStats.Agility;
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

        public ISkill ActiveSkill {
            get { return _activeSkill;  }
        }

        public Team Team;
        #endregion Stats

        #region CollectionProperties
        public IEnumerable<ISkill> Skills { get { return _skills.Values; } }
        public int SkillCount { get { return _skills.Count; } }

        public IEnumerable<Pawn> KnownPawns { get { return _knownPawns; } }
        public int KnownPawnsCount { get { return _knownPawns.Count; } }

        public IEnumerable<Pawn> PawnsInView { get { return _visiblePawns; } }
        public int PawnsInViewCount { get { return _visiblePawns.Count; } }

        public IEnumerable<Gear> Gear { get { return _gear; } }
        public int GearCount { get { return _gear.Count; } }
        #endregion CollectionProperties

        #region Events
        public event Action<Combatant, ISkill> SkillAdded;
        public event Action<Combatant, ISkill> SkillRemoved;
        public event Action<Combatant, ISkill> SkillFired;
        public event Action<Combatant, ISkill> SkillActivated;
        public event Action<Combatant, ISkill> SkillDeactivated;
        public event Action<Combatant, Gear> GearEquipped;
        public event Action<Combatant, Gear> GearRemoved;
        #endregion Events

        #region PrivateMembers
        private bool _didStatsChange;
        private Stats _baseStats;
        private Stats _effectiveStats;
        private int _health;
        private int _actionPoints;
        private int _queueCost;
        private ISkill _activeSkill;

        private List<Gear> _gear = new List<Gear>();
        private Dictionary<Type, ISkill> _skills = new Dictionary<Type, ISkill>();
        private HashSet<Pawn> _visiblePawns = new HashSet<Pawn>();
        private HashSet<Pawn> _knownPawns = new HashSet<Pawn>();
        #endregion PrivateMembers

        public Combatant(string name, Board board, Point position, bool isOpaque = true, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = true)
            : base(name, board, position, footprint, isCollidable, isSolid, isOpaque){
        }

        public void AddSkill(ISkill skill) {
            _skills.Add(skill.GetType(), skill);
        }

        public T GetSkill<T>() where T : ISkill {
            ISkill skill;
            _skills.TryGetValue(typeof(T), out skill);
            return (T)skill;
        }

        public bool HasSkill(ISkill skill) {
            return _skills.ContainsValue(skill);
        }

        public void SetActiveSkill(ISkill skill) {
            if (skill == null || _skills.ContainsValue(skill)) {
                if (_activeSkill != skill) {
                    ISkill oldSkill = _activeSkill;
                    _activeSkill = skill;

                    if (_activeSkill == null) {
                        if (SkillDeactivated != null) {
                            SkillDeactivated(this, oldSkill);
                        }
                    } else if (SkillActivated != null) {
                        SkillActivated(this, _activeSkill);
                    }
                }
            } else {
                throw new ArgumentException("No Skill " + skill + " is assigned to this Combatant " + ToString());
            }
        }

        public void SetActiveSkill<T>() where T : ISkill {
            ISkill skill = GetSkill<T>();
            if (skill == null) {
                throw new NullReferenceException("No Skill of type " + typeof(T) + " is assigned to this Combatant " + ToString());
            }
            SetActiveSkill(skill);
        }

        public void ClearActiveSkill() {
            if (_activeSkill != null) {
                ISkill oldSkill = _activeSkill;
                _activeSkill = null;

                if (SkillDeactivated != null) {
                    SkillDeactivated(this, oldSkill);
                }
            }
        }

        public bool FireActiveSkill() {
            bool fired = false;
            if (_activeSkill != null && _actionPoints > _activeSkill.ActionPoints) {
                _activeSkill.Fire();
                if (SkillFired != null) { SkillFired(this, _activeSkill); }
                ClearActiveSkill();
                fired = true;
            }

            return fired;
        }

        public IEnumerable<Pawn> GetKnownPawns() {
            return _knownPawns;
        }

        public bool AddPawnToView(Pawn pawn) {
            _knownPawns.Add(pawn);
            return _visiblePawns.Add(pawn);
        }

        public void RemovePawnFromView(Pawn pawn) {
            _visiblePawns.Remove(pawn);
        }

        public bool IsPawnInView(Pawn pawn) {
            return _visiblePawns.Contains(pawn);
        }

        public void AddGear(Gear gear) {
            _gear.Add(gear);

            if (GearEquipped != null) {
                GearEquipped(this, gear);
            }
        }

        public void RemoveGear(Gear gear) {
            _gear.Remove(gear);

            if (GearRemoved != null) {
                GearRemoved(this, gear);
            }
        }

        #region PawnOverrides

        public override void PushAction(PawnAction action) {
            base.PushAction(action);

            CombatantAction combatantAction;
            if ((combatantAction = action as CombatantAction) != null)  {
                _queueCost += combatantAction.ActionPoints;
            }
        }

        public override void PushActions(IEnumerable<PawnAction> actions) {
            base.PushActions(actions);

            foreach (PawnAction action in actions) {
                CombatantAction combatantAction;
                if ((combatantAction = action as CombatantAction) != null) {
                    _queueCost += combatantAction.ActionPoints;
                }
            }
        }

        public override void ClearActions() {
            base.ClearActions();
            _queueCost = 0;
        }

        public override void BeginTurn() {
            _actionPoints = EffectiveStats.Stamina;
        }

        protected override void PreprocessAction(PawnAction action) {
            CombatantAction combatantAction = action as CombatantAction;
            if (combatantAction != null) {
                _actionPoints -= combatantAction.ActionPoints;
            }
        }

        #endregion PawnOverrides
    }
}
