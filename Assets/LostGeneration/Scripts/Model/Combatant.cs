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

        public int ActionPoints { get { return _actionPoints; } }

        private bool _didStatsChange;
        private Stats _baseStats;
        private Stats _effectiveStats;
        private int _health;
        private int _actionPoints;

        private List<Gear> _gear = new List<Gear>();
        private Dictionary<Type, ISkill> _skills = new Dictionary<Type, ISkill>();
        private Dictionary<string, ISkill> _aliasedSkills = new Dictionary<string, ISkill>();

        private HashSet<Pawn> _knownPawns = new HashSet<Pawn>();

        public Combatant(string name, Board board, Point position, bool isOpaque = true, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = true)
            : base(name, board, position, footprint, isCollidable, isSolid, isOpaque){
        }

        public void AddSkill(ISkill skill, string alias = null) {
            _skills.Add(skill.GetType(), skill);
            if (alias != null) {
                _aliasedSkills.Add(alias, skill);
            } else {
                _aliasedSkills.Add(skill.Name, skill);
            }
        }

        public ISkill GetSkill(string alias) {
            ISkill skill;
            _aliasedSkills.TryGetValue(alias, out skill);

            return skill;
        }

        public T GetSkill<T>() where T : ISkill {
            ISkill skill;
            _skills.TryGetValue(typeof(T), out skill);
            return (T)skill;
        }

        public IEnumerable<Pawn> GetPawnsInView() {
            foreach (Pawn pawn in _knownPawns) {
                yield return pawn;
            }
        }

        public bool AddPawnToView(Pawn pawn) {
            return _knownPawns.Add(pawn);
        }

        public void RemovePawnFromView(Pawn pawn) {
            _knownPawns.Remove(pawn);
        }

        public bool IsPawnInView(Pawn pawn) {
            return _knownPawns.Contains(pawn);
        }

        public override void BeginTurn() {
            _actionPoints = EffectiveStats.Stamina;
        }
    }
}
