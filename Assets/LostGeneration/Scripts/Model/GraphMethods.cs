using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LostGen {
    public static class GraphMethods {
        public delegate int Heuristic<T>(T start, T end);

        private class SortNode<T> : IComparable<SortNode<T>> {
            public T Node;
            public int GScore = 0;
            public int FScore = 0;
            public int Level = 0;

            public int CompareTo(SortNode<T> other) {
                int compare = 0;
                
                if (FScore < other.FScore) { compare = -1; }
                else if (FScore > other.FScore) { compare = 1; }

                return compare;
            }
        }

        public static IEnumerable<T> FloodFill<T>(T start, int maxCost = -1, int maxDepth = -1) where T : IGraphNode<T> {
            HashSet<T> domain = new HashSet<T>();
            HashSet<SortNode<T>> open = new HashSet<SortNode<T>>();

            SortNode<T> first = new SortNode<T>() { Node = start, Level = 0 };
            open.Add(first);
            domain.Add(first.Node);

            while (open.Count > 0) {
                SortNode<T> current = open.Min();
                open.Remove(current);

                if ((maxDepth == -1 || current.Level < maxDepth)) {
                    foreach (T neighbor in current.Node.GetNeighbors()) {
                        if (domain.Contains(neighbor)) {
                            continue;
                        }

                        int tentativeGScore = current.GScore + current.Node.GetEdgeCost(neighbor);

                        if (maxCost == -1 || tentativeGScore <= maxCost) {
                            domain.Add(neighbor);                            
                            open.Add(new SortNode<T>() {
                                Node = neighbor,
                                GScore = tentativeGScore,
                                FScore = tentativeGScore,
                                Level = current.Level + 1
                            });
                        }
                    }
                }
            }
            
            return domain;
        }

        public static IEnumerable<T> FindPath<T>(T start, T end, Heuristic<T> heuristic) where T : IGraphNode<T> {
            HashSet<T> visited = new HashSet<T>();
            
            HashSet<SortNode<T>> openSet = new HashSet<SortNode<T>>();
            Dictionary<T, SortNode<T>> openGraphNodes = new Dictionary<T, SortNode<T>>();
            
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();

            Stack<T> path = new Stack<T>();
            SortNode<T> startNode = new SortNode<T>() { Node = start };
            openSet.Add(startNode);
            openGraphNodes.Add(start, startNode);
            
            while (openSet.Count > 0) {
                SortNode<T> current = openSet.Min();
                
                if (current.Node.IsMatch(end)) {
                    T pathNode = current.Node;
                    path.Push(pathNode);
                    while (cameFrom.ContainsKey(pathNode)) {
                        pathNode = cameFrom[pathNode];
                        path.Push(pathNode);
                    }
                    break;
                }
                
                if (!openSet.Remove(current) || !openGraphNodes.Remove(current.Node)) {
                    throw new Exception("Current node was somehow not in the open sets. " +
                        "Check your GetHashCode() implementation.");
                }

                foreach (T neighbor in current.Node.GetNeighbors()) {
                    if (visited.Contains(neighbor)) {
                        continue;
                    }

                    // Calculate GScore by current node's accumulated cost plus the neighbor's
                    // entrance edge cost
                    int tentativeGScore = current.GScore + current.Node.GetEdgeCost(neighbor);

                    // If the recorded distance is less than the neighbor distance, replace it
                    SortNode<T> oldNeighbor;
                    if (!openGraphNodes.TryGetValue(neighbor, out oldNeighbor)) {
                        oldNeighbor = new SortNode<T>() {
                            Node = neighbor,
                            GScore = Int32.MaxValue
                        };

                        openSet.Add(oldNeighbor);
                        openGraphNodes.Add(oldNeighbor.Node, oldNeighbor);
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
