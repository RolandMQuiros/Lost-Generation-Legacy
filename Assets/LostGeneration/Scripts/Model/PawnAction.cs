namespace LostGen {
    public abstract class PawnAction {
        public virtual Pawn Owner { get; protected set; }
        public abstract Point PostRunPosition { get; }

        public PawnAction(Pawn owner) {
            Owner = owner;
        }

        protected void SendMessage(MessageArgs message) {
            Owner.EmitMessage(message);
        }

        /// <summary>
        /// Performs any effects this PawnAction has on the Owner. This is called both when actually
        /// executing the PawnAction for the turn, and during planning.
        /// </summary>
        public virtual void Do() { };
        /// <summary>
        /// Rolls back the effects this PawnAction has on the owner.  This is called during the planning
        /// phase if the user decides to cancel some actions.
        /// </summary>
        public virtual void Undo() { };
        /// <summary>
        /// Performs effects this PawnAction has on other Pawns.  This is only called during the actual
        /// Turn execution, and not during the planning phase.
        /// </summary>
        public virtual void React() { }
    }
}