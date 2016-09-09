using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public interface IDecision {
        int Cost { get; }

        StateOffset GetPostcondition(StateOffset previous);
        bool ArePreconditionsMet(StateOffset offset);

        void Setup();
        void Run();
    }
}
