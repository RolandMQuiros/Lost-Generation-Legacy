using System;
using System.Collections.Generic;

namespace LostGen {

    public abstract class AbstractGraphNode<T> : IEquatable<AbstractGraphNode<T>> where T : IEquatable<T> {
        public abstract T GetData();
        public abstract int GetEdgeCost(AbstractGraphNode<T> to);
        public abstract IEnumerator<AbstractGraphNode<T>> GetNeighborIter();

        public virtual bool Equals(AbstractGraphNode<T> other) {
            return GetData().Equals(other.GetData());
        }
    }

}