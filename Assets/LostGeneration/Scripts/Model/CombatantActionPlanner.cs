using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    /*
    public class CombatantActionPlanner {
        private HashSet<Skill> _skills = new HashSet<Skill>();
        private List<SkillNode> _skillNodes = new List<SkillNode>();

        private bool _rebuildGraph = false;

        /// <summary>
        /// Runs the Setup method on each Skill
        /// </summary>
        public void SetupSkills() {

        }

        public bool AddSkill(Skill skill) {
            bool success = _skills.Add(skill);
            _rebuildGraph = success;
            return success;
        }

        public void FindPlan(DecisionNode goal) {
            if (_rebuildGraph) {
                BuildGraph();
            }

            BoardState targetState = new BoardState();

            //
        }

        private void BuildGraph() {
            foreach (SkillNode skill in _skillNodes) {
                foreach (SkillNode other in _skillNodes) {
                    if (other != skill) {
                        skill.TryToSetNeighbor(other);
                    }
                }
            }
        }
    }*/
}
