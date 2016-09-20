using System;
using System.Collections.Generic;

namespace LostGen {
    public abstract class PawnController {
        public EventHandler Ready;
        public abstract void BeginTurn();
    }
}
