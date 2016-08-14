using System;
using System.Collections.Generic;

namespace LostGen {

    public abstract class GraphNode<T> : IEquatable<GraphNode<T>> where T : IEquatable<T> {
        public abstract T GetData();
        public abstract int GetEdgeCost(GraphNode<T> neighbor);
        public abstract IEnumerable<GraphNode<T>> GetNeighbors();

        public virtual bool Equals(GraphNode<T> other) {
            return GetData().Equals(other.GetData());
        }
    }

}