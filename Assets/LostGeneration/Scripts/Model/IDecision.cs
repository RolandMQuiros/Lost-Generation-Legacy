using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public interface IDecision {
        int Cost { get; }

        StateOffset ApplyPreconditions(StateOffset next = null);
        StateOffset ApplyPostconditions(StateOffset previous = null);

        void Setup();
        void Run();
    }
}
