namespace LostGen {
    public abstract class Action {
        public Pawn Owner { get; protected set; }
        public bool IsMessageSuppressed;
        public bool PauseAfterRun { get; private set; }

        public Action(Pawn owner, bool suppressMessage = false, bool pauseAfterRun = true) {
            Owner = owner;
            IsMessageSuppressed = suppressMessage;
            PauseAfterRun = pauseAfterRun;
        }

        protected void SendMessage(MessageArgs message) {
            if (!IsMessageSuppressed) {
                Owner.EmitMessage(message);
            }
        }

        public abstract void Run();
    }
}