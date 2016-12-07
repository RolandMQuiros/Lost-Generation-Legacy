using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LostGen {
    public static class GraphMethods<T> where T : IGraphNode {
        public delegate int Heuristic(T start, T end);

        private class SortNode : IComparable<SortNode> {
            public T Node;
            public int GScore = 0;
            public int FScore = 0;
            public int Level = 0;

            public int CompareTo(SortNode other) {
                int compare = 0;
                
                if (FScore < other.FScore) { compare = -1; }
                else if (FScore > other.FScore) { compare = 1; }

                return compare;
            }
        }

        public static IEnumerable<T> FloodFill(T start, int maxCost = -1, int maxDepth = -1) {
            HashSet<T> domain = new HashSet<T>();
            List<SortNode> open = new List<SortNode>();

            open.Add(new SortNode() { Node = start, Level = 1 });
            domain.Add(open[0].Node);

            //Profiler.BeginSample("GraphMethods.FloodFill");
            while (open.Count > 0) {
                open.Sort();
                SortNode current = open[0];
                open.RemoveAt(0);
                //domain.Add(current.Node);

                if (maxDepth == -1 || current.Level < maxDepth) {
                    foreach (T neighbor in current.Node.GetNeighbors()) {
                        if (domain.Contains(neighbor)) {
                            continue;
                        }

                        int tentativeGScore = current.GScore + current.Node.GetEdgeCost(neighbor);

                        if (maxCost == -1 || tentativeGScore < maxCost) {
                            domain.Add(neighbor);
                            open.Add(new SortNode() {
                                Node = neighbor,
                                GScore = tentativeGScore,
                                FScore = tentativeGScore,
                                Level = current.Level + 1
                            });
                        }
                    }
                }
            }
            //Profiler.EndSample();

            return domain;
        }

        public static IEnumerable<T> FindPath(T start, T end, Heuristic heuristic) {
            HashSet<T> visited = new HashSet<T>();
            List<SortNode> open = new List<SortNode>();
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();

            Stack<T> path = new Stack<T>();

            open.Add(new SortNode() { Node = start });
            
            while (open.Count > 0) {
                open.Sort();
                SortNode current = open[0];
                
                if (current.Node.IsMatch(end)) {
                    T pathNode = current.Node;
                    path.Push(pathNode);
                    while (cameFrom.ContainsKey(pathNode)) {
                        pathNode = cameFrom[pathNode];
                        path.Push(pathNode);
                    }
                    break;
                }

                open.RemoveAt(0);

                foreach (T neighbor in current.Node.GetNeighbors()) {
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
