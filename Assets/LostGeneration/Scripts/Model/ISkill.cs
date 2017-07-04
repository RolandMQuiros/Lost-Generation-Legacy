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
        /// <summary>Pawn that owns and executes this Skill</summary>
        Pawn Pawn { get; }
        /// <summary>The number of Action Points this Skill will consume when fired</summary>
        int ActionPoints { get; }

        /// <summary>
        /// Generates this Skill's Actions and pushes them onto the owning Combatant's Action queue,
        /// either on the front or back.
        /// </summary>
        PawnAction Fire();

        /// <summary>
        /// Whether or not this Skill is usable based on the Pawn's current state.
        /// </summary>
        /// <returns>True if this Skill is usable, false otherwise.</returns>
        bool IsUsable();
    }
}
