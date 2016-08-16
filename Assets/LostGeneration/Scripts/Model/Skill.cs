using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    /// <summary>
    /// Base class for Combatant Skills.  Skills generate the Actions that Pawns execute with every step of the board.
    /// 
    /// </summary>
    public abstract class Skill : IEquatable<Skill> {
        /// <summary>Name displayed to users</summary>
        public string Name;
        /// <summary>Description of Skill displayed to users</summary>
        public string Description;
        /// <summary>Combatant that owns and executes this Skill</summary>
        public Combatant Owner { get; private set; }
        /// <summary>The number of Action Points this Skill will consume when fired</summary>
        public virtual int ActionPoints { get; protected set; }

        protected BoardState _postCondition = new BoardState();
        public BoardState PostCondition {
            get { return _postCondition; }
        }

        /// <summary>
        /// Creates a new Skill
        /// </summary>
        /// <param name="owner">Reference to owning Combatant</param>
        /// <param name="name">User-readable name for this Skill</param>
        /// <param name="description">User-readable description for this Skill</param>
        public Skill(Combatant owner, string name, string description) {
            Owner = owner;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Used by AIPlanner to set state values before evaluating cost.
        /// </summary>
        public virtual void Setup() { }

        /// <summary>
        /// Generates the Actions created by this Skill and enqueues them on the target Pawns
        /// </summary>
        public abstract void Fire();

        /// <summary>
        /// Edge cost in AI planning system.  Calculated using a variety of metrics, specifically the
        /// combatant's personality values and action point cost.
        /// </summary>
        /// <returns></returns>
        public abstract int GetDecisionCost();

        /// <summary>
        /// Performs a reference equality check.  This is here just to make Skills compatible with the Pathfinder. 
        /// </summary>
        /// <param name="other">Reference to other Skill</param>
        /// <returns>true, if reference is the same as this</returns>
        public bool Equals(Skill other) {
            return this.Equals(other);
        }
    }
}
