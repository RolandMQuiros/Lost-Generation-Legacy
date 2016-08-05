namespace LostGen {
    public abstract class Action {
        public Pawn Owner { get; protected set; }
        public bool IsMessageSuppressed;

        public Action(Pawn owner) {
            Owner = owner;
        }

        protected void SendMessage(MessageArgs message) {
            if (!IsMessageSuppressed) {
                Owner.EmitMessage(message);
            }
        }

        public abstract void Run();
    }
}