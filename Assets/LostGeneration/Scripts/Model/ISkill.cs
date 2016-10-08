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
    public interface ISkill {
        /// <summary>Name displayed to users</summary>
        string Name { get; }
        /// <summary>Description of Skill displayed to users</summary>
        string Description { get; }
        /// <summary>Combatant that owns and executes this Skill</summary>
        Combatant Owner { get; }
        /// <summary>The number of Action Points this Skill will consume when fired</summary>
        int ActionPoints { get; }
        /// <summary>If this Skill is ready to Fire</summary>
        bool IsReadyToFire { get; }
        bool IsActiveSkill { get; }

        /// <summary>
        /// Generates this Skill's Actions and pushes them onto the owning Combatant's Action queue,
        /// either on the front or back.
        /// </summary>
        void Fire();
    }
}
