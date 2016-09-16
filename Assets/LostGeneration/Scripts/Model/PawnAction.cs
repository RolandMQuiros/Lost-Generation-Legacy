namespace LostGen {
    public abstract class PawnAction {
        public virtual Pawn Owner { get; protected set; }
        public bool IsMessageSuppressed;

        public PawnAction(Pawn owner, bool suppressMessage = false) {
            Owner = owner;
            IsMessageSuppressed = suppressMessage;
        }

        protected void SendMessage(MessageArgs message) {
            if (!IsMessageSuppressed) {
                Owner.EmitMessage(message);
            }
        }

        public abstract void Run();
    }
}