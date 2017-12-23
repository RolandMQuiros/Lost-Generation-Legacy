using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Model {
    public class DamageMessage : IPawnMessage {
        public Pawn Source { get; private set; }
        public Pawn Target { get; private set; }
        public String Text { get; private set; }
        public bool IsCritical {get; private set; }
        public int Amount { get; private set; }

        public DamageMessage(Pawn source, Pawn target, int amount) {
            Source = source;
            Target = target;
            Amount = amount;

            if (Source != null) {
                Text = Source.Name + " inflicted " + amount + " damage on " + Target.Name;
            } else {
                Text = Target.Name + " took " + amount + " damage";
            }
        }
    }
}
