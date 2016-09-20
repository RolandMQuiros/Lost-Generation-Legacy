namespace LostGen {
    public abstract class IPawnAction {
        public virtual Pawn Owner { get; protected set; }

        public IPawnAction(Pawn owner) {
            Owner = owner;
        }

        protected void SendMessage(MessageArgs message) {
            Owner.EmitMessage(message);
        }

        public abstract void Run();
    }
}