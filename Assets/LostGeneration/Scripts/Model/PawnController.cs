using System;
using System.Collections.Generic;

namespace LostGen {
    public interface IPawnController {
        event Action<IPawnController> Ready;
        void BeginTurn();
    }
}
