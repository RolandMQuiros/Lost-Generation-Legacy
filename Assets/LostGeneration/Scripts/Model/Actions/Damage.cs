using System;

namespace LostGen.Actions {
    public class Damage : Action {
        public class Message : MessageArgs {
            public Combatant Target { get; private set; }
            public Combatant Source { get; private set; }
            public int Amount { get; private set; }

            public Message(Combatant target, int amount, Combatant source = null) {
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

        public int Amount { get; private set; }
        public Combatant TargetCombatant { get; private set; }
        public Combatant SourceCombatant { get; private set; }

        public Damage(Combatant target, int amount, Combatant source = null)
            : base(target) {
            Amount = amount;
            SourceCombatant = source;
        }

        public override void Run() {
            if (Amount > 0) {
                TargetCombatant.Health -= Amount;
                TargetCombatant.EmitMessage(
                    new Message(TargetCombatant, Amount, SourceCombatant)
                );
            }
        }
    }
}
