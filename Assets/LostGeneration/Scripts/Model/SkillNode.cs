using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    /// <summary>
    /// Represents a skill in a goal-oriented action plan (GOAP) graph
    /// </summary>
    public class SkillNode : GraphNode<Skill> {
        private Skill _skill;
        private List<Skill> _neighbors = new List<Skill>();

        public SkillNode(Skill skill) {
            _skill = skill;
        }

        public override Skill GetData() {
            return _skill;
        }

        public override int GetEdgeCost(GraphNode<Skill> neighbor) {
            Skill neighborSkill = neighbor.GetData();

            if (!_neighbors.Contains(neighborSkill)) {
                throw new ArgumentException("Given neighbor SkillNode is not connected to this SkillNode", neighborSkill.Name);
            }

            return neighborSkill.ActionPoints;
        }
    }
}
