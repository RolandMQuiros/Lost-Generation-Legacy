using System;

namespace LostGen {
    public class MessageArgs : EventArgs {
        // game-specific code
        public string Text;
        public MessageArgs() { }
    }
}