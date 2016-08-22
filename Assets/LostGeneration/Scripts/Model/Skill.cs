using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    /// <summary>
    /// Base class for Combatant Skills.  Skills generate the Actions that Pawns execute with every step of the board.
    /// 
    /// While Actions can be freely added to Pawns, the Skills class acts as a nice constraint on which Actions a Combatant
    /// is capable of.
    /// </summary>
    public abstract class Skill {
        /// <summary>Name displayed to users</summary>
        public string Name;
        /// <summary>Description of Skill displayed to users</summary>
        public string Description;
        /// <summary>Combatant that owns and executes this Skill</summary>
        public Combatant Owner { get; private set; }
        /// <summary>The number of Action Points this Skill will consume when fired</summary>
        public virtual int ActionPoints { get; protected set; }

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
        /// Generates this Skill's Actions and pushes them onto the owning Combatant's Action queue,
        /// either on the front or back.
        /// </summary>
        /// <returns>Reference to the last Action pushed. Can be used to determine when a Skill is finished.</returns>
        public abstract Action Fire();
    }
}
