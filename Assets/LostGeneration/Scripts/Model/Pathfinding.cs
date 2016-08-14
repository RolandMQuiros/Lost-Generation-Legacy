using System;
using System.Collections.Generic;

namespace LostGen {

    public class Pathfinder<T> where T : IEquatable<T> {
        public delegate int Heuristic(T start, T end);

        private class SortNode : IComparable<SortNode> {
            public GraphNode<T> Node;
            public int GScore = 0;
            public int FScore = 0;

            public int CompareTo(SortNode other) {
                int compare = 0;

                if (FScore < other.FScore) { compare = -1; }
                else if (FScore > other.FScore) { compare = 1; }

                return compare;
            }
        }

        public static IEnumerable<T> FindPath(GraphNode<T> start, GraphNode<T> end, Heuristic heuristic) {
            HashSet<T> visited = new HashSet<T>();
            List<SortNode> open = new List<SortNode>();
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();

            Stack<T> path = new Stack<T>();

            T startData = start.GetData();
            T endData = end.GetData();

            open.Add(new SortNode() { Node = start });
            
            while (open.Count > 0) {
                open.Sort();
                SortNode current = open[0];
                T currentData = current.Node.GetData();

                if (current.Node.Equals(end)) {
                    T pathData = currentData;
                    path.Push(pathData);
                    while (cameFrom.ContainsKey(pathData)) {
                        pathData = cameFrom[pathData];
                        path.Push(pathData);
                    }
                    break;
                }

                open.RemoveAt(0);

                foreach (GraphNode<T> neighbor in current.Node.GetNeighbors()) {
                    T neighborData = neighbor.GetData();

                    if (visited.Contains(neighborData)) {
                        continue;
                    }

                    // Calculate GScore by current node's accumulated cost plus the neighbor's
                    // entrance edge cost
                    int tentativeGScore = current.GScore + current.Node.GetEdgeCost(neighbor);

                    // If the recorded distance is less than the neighbor distance, replace it
                    SortNode oldNeighbor = open.Find(node => node.Node.Equals(neighbor));
                    
                    if (oldNeighbor == null) {
                        oldNeighbor = new SortNode() {
                            Node = neighbor,
                            GScore = Int32.MaxValue
                        };

                        open.Add(oldNeighbor);
                    }

                    T oldNeighborData = oldNeighbor.Node.GetData();
                    if (tentativeGScore < oldNeighbor.GScore) {
                        oldNeighbor.GScore = tentativeGScore;
                        oldNeighbor.FScore = tentativeGScore + heuristic(oldNeighborData, endData);
                        cameFrom[oldNeighborData] = currentData;
                    }
                }

                visited.Add(currentData);
            }

            return path;
        }
    }
}
