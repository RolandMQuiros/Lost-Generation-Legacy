using System;

namespace LostGen {
    public abstract class MessageArgs : EventArgs {
        public Pawn Source;
        public Pawn Target;
        public string Text;
        public bool IsCritical;
    }
}