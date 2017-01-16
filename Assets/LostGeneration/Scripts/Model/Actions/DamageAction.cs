using System;
using System.Collections.Generic;

namespace LostGen {
    public class DamageAction : PawnAction {
        public override Point PostRunPosition {
            get { return Owner.Position; }
        }
        public int Amount { get; private set; }
        public Combatant Target { get; private set; }
        public Combatant Source { get; private set; }

        public DamageAction(Combatant target, int amount, Combatant source = null)
            : base(target) {
            Amount = amount;
            Source = source;
        }

        public override void Commit(Queue<IPawnMessage> messages) {
            if (Amount > 0) {
                Target.Health -= Amount;

                 messages.Enqueue(new DamageMessage(Target, Amount, Source));
            }
        }
    }
}
