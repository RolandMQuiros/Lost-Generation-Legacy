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
        private List<GraphNode<Skill>> _neighbors = new List<GraphNode<Skill>>();

        public SkillNode(Skill skill) {
            _skill = skill;
        }

        public bool TryToSetNeighbor(SkillNode other) {
            bool eligible = false;
            Skill otherSkill = other._skill;
            BoardState ourPostconditions = new BoardState();

            _skill.GetPostconditions(ourPostconditions);

            if (otherSkill.ArePreconditionsMet(ourPostconditions)) {
                eligible = true;
                _neighbors.Add(other);
            }

            return eligible;
        }

        public override Skill GetData() {
            return _skill;
        }

        public override int GetEdgeCost(GraphNode<Skill> neighbor) {
            Skill neighborSkill = neighbor.GetData();

            GraphNode<Skill> neighborNode = _neighbors.Find(node => node.GetData().Equals(neighborSkill));

            if (neighborNode == null) {
                throw new ArgumentException("Given neighbor SkillNode is not connected to this SkillNode", neighborSkill.Name);
            }

            return neighborSkill.ActionPoints;
        }

        public override IEnumerable<GraphNode<Skill>> GetNeighbors() {
            return _neighbors;
        }
    }
}
