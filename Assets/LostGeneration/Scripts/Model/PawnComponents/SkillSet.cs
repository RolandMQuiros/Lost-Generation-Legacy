using System.Collections.Generic;

namespace LostGen
{
    public class SkillSet : PawnComponent
    {
        public IEnumerable<ISkill> Skills
        {
            get { return _skills; }
        }

        private HashSet<ISkill> _skills;

        public SkillSet(IEnumerable<ISkill> skills = null) {
            if (skills != null) {
                _skills = new HashSet<ISkill>(skills);
            } else {
                _skills = new HashSet<ISkill>();
            }
        }

        public void AddSkill(ISkill skill)
        {
            _skills.Add(skill);
        }

        public void RemoveSkill(ISkill skill)
        {
            _skills.Remove(skill);
        }

        public bool HasSkill(ISkill skill)
        {
            return _skills.Contains(skill);
        }
    }
}