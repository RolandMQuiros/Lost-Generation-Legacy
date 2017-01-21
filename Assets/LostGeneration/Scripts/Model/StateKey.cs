using System;
using System.Collections.Generic;

namespace LostGen {
    public static class StateKey {
        public static string Key(Pawn pawn, string append) {
            return pawn.InstanceID + append;
        }

        public static string Position(Pawn pawn) {
            return Key(pawn, "xy");
        }

        public static string AP(Combatant combatant) {
            return Key(combatant.Pawn, "ap");
        }

        public static string Health(Combatant combatant) {
            return Key(combatant.Pawn, "health");
        }
    }
}
