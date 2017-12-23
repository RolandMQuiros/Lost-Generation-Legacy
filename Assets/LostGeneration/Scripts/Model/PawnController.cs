using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public interface IPawnController {
        event Action<IPawnController> Ready;
        void BeginTurn();
    }
}
