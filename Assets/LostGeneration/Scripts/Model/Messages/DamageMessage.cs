using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class DamageMessage : MessageArgs {
        public Combatant Target { get; private set; }
        public Combatant Source { get; private set; }
        public int Amount { get; private set; }

        public DamageMessage(Combatant target, int amount, Combatant source = null) {
            Target = target;
            Source = source;
            Amount = amount;

            if (Source != null) {
                Text = Source.Name + " inflicted " + amount + " damage on " + Target.Name;
            } else {
                Text = Target.Name + " took " + amount + " damage";
            }
        }
    }
}
