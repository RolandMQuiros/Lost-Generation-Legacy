namespace LostGen {
    public abstract class Action {
        public Pawn Target { get; protected set; }
        public bool IsMessageSuppressed;

        public Action(Pawn source) {
            Target = source;
        }

        protected void SendMessage(MessageArgs message) {
            if (!IsMessageSuppressed) {
                Target.EmitMessage(message);
            }
        }

        public abstract void Run();
    }
}