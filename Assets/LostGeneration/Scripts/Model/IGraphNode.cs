using System;
using System.Collections.Generic;

namespace LostGen.Model {
    public interface IGraphNode<T> {
        int GetEdgeCost(T neighbor);
        IEnumerable<T> GetNeighbors();
        bool IsMatch(T other);
    }
}