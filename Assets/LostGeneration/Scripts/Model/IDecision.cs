using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public interface IDecision {
        int Cost { get; }
        bool ArePreconditionsMet(StateOffset state);
        StateOffset ApplyPostconditions(StateOffset state);
    }
}
