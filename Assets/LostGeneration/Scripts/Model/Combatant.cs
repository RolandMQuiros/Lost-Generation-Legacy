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
        public int ActionPoints { get; private set; }

        private List<Gear> _gear = new List<Gear>();
        private Dictionary<string, Skill> _skills = new Dictionary<string, Skill>();

        public Combatant(string name, Board board, Point position, bool isOpaque = true, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = true)
            : base(name, board, position, footprint, isCollidable, isSolid, isOpaque){
        }

        public void AddSkill(Skill skill, string alias = null) {
            if (alias != null) {
                _skills.Add(alias, skill);
            } else {
                _skills.Add(skill.Name, skill);
            }
        }

        public Skill GetSkill(string alias) {
            Skill skill;
            _skills.TryGetValue(alias, out skill);

            return skill;
        }

        public bool FireSkill(string skillName) {
            bool fired = false;
            Skill skill = _skills[skillName];

            if (skill.Cost <= _actionPoints) {
                _actionPoints -= skill.Cost;
                skill.Fire();
                fired = true;
            }

            return fired;
        }
    }
}
