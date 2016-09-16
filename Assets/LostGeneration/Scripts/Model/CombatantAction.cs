using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class CombatantAction : PawnAction {
        public new Combatant Owner { get { return _combatantOwner; } }
        public abstract int ActionPoints { get; }
        private Combatant _combatantOwner;

        public CombatantAction(Combatant owner, bool suppressMessage = true) : base(owner, suppressMessage) {
            _combatantOwner = owner;
        }

        public abstract override void Run();
    }
}
