using System;
using System.Collections.Generic;

namespace LostGen {
    public interface IGraphNode<T> {
        int GetEdgeCost(T neighbor);
        IEnumerable<T> GetNeighbors();
        bool IsMatch(T other);
    }
}