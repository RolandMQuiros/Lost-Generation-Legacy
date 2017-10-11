using System;
using System.Collections.Generic;

namespace LostGen {
    public class DamageAction : PawnAction {
        public int Amount { get; private set; }
        public Pawn Target { get; private set; }
        public DamageAction(Pawn attacker, Pawn target, int amount)
            : base(attacker) {
            Amount = amount;
            Target = target;
        }

        public override bool Commit() {
            Health target = Target.GetComponent<Health>();
            if (Amount > 0) {
                target.Current = target.Current - Amount;
                Owner.PushMessage(new DamageMessage(Owner, Target, Amount));
            }
            return true;
        }
    }
}
