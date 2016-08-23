using System;
using System.Collections.Generic;

namespace LostGen {
    public class Combatant : Pawn {
        private bool _didStatsChange;

        private Stats _baseStats;
        public Stats BaseStats
        {
            get { return _baseStats; }
            set {
                _baseStats = value;
                _didStatsChange = true;
            }
        }

        private Stats _effectiveStats;
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

        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = Math.Max(0, Math.Min(_effectiveStats.Health, value));
            }
        }

        private int _actionPoints;
        public int ActionPoints { get { return _actionPoints; } }
        
        private List<Gear> _gear = new List<Gear>();
        private Dictionary<string, ISkill> _skills = new Dictionary<string, ISkill>();

        private HashSet<Pawn> _knownPawns = new HashSet<Pawn>();

        public Combatant(string name, Board board, Point position, bool isOpaque = true, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = true)
            : base(name, board, position, footprint, isCollidable, isSolid, isOpaque){
        }

        public void AddSkill(ISkill skill, string alias = null) {
            if (alias != null) {
                _skills.Add(alias, skill);
            } else {
                _skills.Add(skill.Name, skill);
            }
        }

        public ISkill GetSkill(string alias) {
            ISkill skill;
            _skills.TryGetValue(alias, out skill);

            return skill;
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
