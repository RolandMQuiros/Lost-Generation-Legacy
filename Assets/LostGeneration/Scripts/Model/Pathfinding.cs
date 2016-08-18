using System;
using System.Collections.Generic;

namespace LostGen {

    public class Pathfinder<T> {
        public delegate int Heuristic(IGraphNode start, IGraphNode end);

        private class SortNode : IComparable<SortNode> {
            public IGraphNode Node;
            public int GScore = 0;
            public int FScore = 0;

            public int CompareTo(SortNode other) {
                int compare = 0;

                if (FScore < other.FScore) { compare = -1; }
                else if (FScore > other.FScore) { compare = 1; }

                return compare;
            }
        }

        public static IEnumerable<IGraphNode> FloodFill(IGraphNode start, int maxCost = -1, int maxDepth = -1) {
            HashSet<IGraphNode> domain = new HashSet<IGraphNode>();
            Stack<SortNode> open = new Stack<SortNode>();

            open.Push(new SortNode() { Node = start });

            while (open.Count > 0) {
                SortNode current = open.Peek();
                domain.Add(current.Node);

                bool leafFound = true;
                if (maxDepth == -1 || open.Count < maxDepth) {
                    foreach (IGraphNode neighbor in current.Node.GetNeighbors()) {
                        if (domain.Contains(neighbor)) {
                            continue;
                        }

                        int tentativeGScore = current.GScore + current.Node.GetEdgeCost(neighbor);

                        if (maxCost == -1 || tentativeGScore < maxCost) {
                            open.Push(new SortNode() {
                                Node = neighbor,
                                GScore = tentativeGScore
                            });
                            leafFound = false;
                        }
                    }
                }

                if (leafFound) {
                    open.Pop();
                }
            }

            return domain;
        }

        public static IEnumerable<IGraphNode> FindPath(IGraphNode start, IGraphNode end, Heuristic heuristic) {
            HashSet<IGraphNode> visited = new HashSet<IGraphNode>();
            List<SortNode> open = new List<SortNode>();
            Dictionary<IGraphNode, IGraphNode> cameFrom = new Dictionary<IGraphNode, IGraphNode>();

            Stack<IGraphNode> path = new Stack<IGraphNode>();

            open.Add(new SortNode() { Node = start });
            
            while (open.Count > 0) {
                open.Sort();
                SortNode current = open[0];

                if (current.Node.IsMatch(end)) {
                    IGraphNode pathNode = current.Node;
                    path.Push(pathNode);
                    while (cameFrom.ContainsKey(pathNode)) {
                        pathNode = cameFrom[pathNode];
                        path.Push(pathNode);
                    }
                    break;
                }

                open.RemoveAt(0);

                foreach (IGraphNode neighbor in current.Node.GetNeighbors()) {
                    if (visited.Contains(neighbor)) {
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

                    if (tentativeGScore < oldNeighbor.GScore) {
                        oldNeighbor.GScore = tentativeGScore;
                        oldNeighbor.FScore = tentativeGScore + heuristic(oldNeighbor.Node, end);
                        cameFrom[oldNeighbor.Node] = current.Node;
                    }
                }

                visited.Add(current.Node);
            }

            return path;
        }
    }
}
