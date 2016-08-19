using System;
using System.Collections.Generic;

namespace LostGen {
    public interface IGraphNode {
        int GetEdgeCost(IGraphNode neighbor);
        IEnumerable<IGraphNode> GetNeighbors();
        bool IsMatch(IGraphNode other);
    }
}