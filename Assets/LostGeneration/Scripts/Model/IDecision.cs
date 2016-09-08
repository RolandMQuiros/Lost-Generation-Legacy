using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public interface IDecision {
        StateOffset PostCondition { get; }
        int Cost { get; }
        bool ArePreConditionsMet(StateOffset offset);

        void Setup();
        void Run();
    }
}
