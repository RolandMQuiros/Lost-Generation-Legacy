using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : Pawn {
        public Stats BaseStats
        {
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

        public int Health
        {
            get { return _health; }
            set
            {
                _health = Math.Max(0, Math.Min(_effectiveStats.Health, value));
            }
        }

        public ISkill ActiveSkill {
            get { return _activeSkill; }
            set {
                if (value == null || _skills.ContainsValue(value)) {
                    _activeSkill = value;
                } else {
                    throw new KeyNotFoundException("Skill " + value + " was not assigned to this Combatant");
                }
            }
        }

        public int ActionPoints { get { return _actionPoints; } }

        public Team Team;

        private bool _didStatsChange;
        private Stats _baseStats;
        private Stats _effectiveStats;
        private int _health;
        private int _actionPoints;

        private List<Gear> _gear = new List<Gear>();
        private Dictionary<Type, ISkill> _skills = new Dictionary<Type, ISkill>();
        private HashSet<Pawn> _visiblePawns = new HashSet<Pawn>();
        private HashSet<Pawn> _knownPawns = new HashSet<Pawn>();
        private ISkill _activeSkill;

        public Combatant(string name, Board board, Point position, bool isOpaque = true, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = true)
            : base(name, board, position, footprint, isCollidable, isSolid, isOpaque){
        }

        public void AddSkill(ISkill skill) {
            _skills.Add(skill.GetType(), skill);
        }

        public void SetActiveSkill<T>() where T : ISkill {
            ISkill skill;
            _skills.TryGetValue(typeof(T), out skill);

            if (skill == null) {
                throw new KeyNotFoundException("Combatant " + ToString() + " does not have a Skill of type " + typeof(T));
            }

            _activeSkill = skill;
        }

        public T GetSkill<T>() where T : ISkill {
            ISkill skill;
            _skills.TryGetValue(typeof(T), out skill);
            return (T)skill;
        }

        public IEnumerable<ISkill> GetSkills() {
            return _skills.Values;
        }

        public bool HasSkill(ISkill skill) {
            return _skills.ContainsValue(skill);
        }

        public IEnumerable<Pawn> GetKnownPawns() {
            return _knownPawns;
        }

        public IEnumerable<Pawn> GetPawnsInView() {
            return _visiblePawns;
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

        public IEnumerable<Gear> GetGear() {
            return _gear;
        }

        public void AddGear(Gear gear) {
            _gear.Add(gear);
        }

        public void RemoteGear(Gear gear) {
            _gear.Remove(gear);
        }

        public override void BeginTurn() {
            _actionPoints = EffectiveStats.Stamina;
        }

        protected override void PreprocessAction(IPawnAction action) {
            CombatantAction combatantAction = action as CombatantAction;
            if (combatantAction != null) {
                _actionPoints -= combatantAction.ActionPoints;
            }
        }
    }
}
