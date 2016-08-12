using System;
using System.Collections.Generic;

namespace LostGen {
    public class Pathfinder<TNodeData> where TNodeData : IComparable<TNodeData> {
        public delegate int Heuristic(TNodeData start, TNodeData end);

        private class GraphNode : IComparable<TNodeData>, IComparable<GraphNode>, IEquatable<TNodeData>, IEquatable<GraphNode> {
            public TNodeData Data;
            public Dictionary<GraphNode, int> Neighbors = new Dictionary<GraphNode, int>();
            private IComparer<TNodeData> _comparer;

            public GraphNode(TNodeData data, IComparer<TNodeData> comparer = null) {
                Data = data;
                _comparer = comparer;
            }

            public int CompareTo(TNodeData other) {
                int compare = 0;
                if (_comparer == null) {
                    compare = Data.CompareTo(other);
                } else {
                    _comparer.Compare(Data, other);
                }
                return compare;
            }

            public int CompareTo(GraphNode other) {
                return CompareTo(other.Data);
            }

            public bool Equals(TNodeData other) {
                return Data.Equals(other);
            }

            public bool Equals(GraphNode other) {
                return Equals(other.Data);
            }
        }


        private IComparer<TNodeData> _comparer;
        private Dictionary<TNodeData, GraphNode> _nodes = new Dictionary<TNodeData, GraphNode>();

        public Pathfinder(IComparer<TNodeData> comparer = null) {
            _comparer = comparer;
        }

        public void AddNode(TNodeData node) {
            _nodes.Add(node, new GraphNode(node, _comparer));
        }

        public bool Contains(TNodeData node) {
            GraphNode asNode;
            _nodes.TryGetValue(node, out asNode);

            return asNode == null;
        }

        public void SetNeighbor(TNodeData node, TNodeData neighbor, int cost) {
            GraphNode asNode, asNeighbor;
            _nodes.TryGetValue(node, out asNode);
            _nodes.TryGetValue(neighbor, out asNeighbor);

            if (asNode == null) {
                throw new KeyNotFoundException("Given node was not found in the graph");
            }

            if (asNeighbor == null) {
                throw new KeyNotFoundException("Neighbor node was not found in the graph");
            }

            asNode.Neighbors[asNeighbor] = cost;
        }

        public IEnumerable<TNodeData> FindPath(TNodeData start, TNodeData end, Heuristic heuristic) {
            GraphNode startNode = _nodes[start];
            GraphNode endNode = _nodes[end];

            HashSet<GraphNode> visited = new HashSet<GraphNode>();
            List<GraphNode> open = new List<GraphNode>();
            Dictionary<GraphNode, int> gScores = new Dictionary<GraphNode, int>();
            Dictionary<GraphNode, int> fScores = new Dictionary<GraphNode, int>();
            Dictionary<TNodeData, TNodeData> cameFrom = new Dictionary<TNodeData, TNodeData>();

            Stack<TNodeData> path = new Stack<TNodeData>();

            open.Add(startNode);
            gScores[startNode] = 0;
            
            while (open.Count > 0) {
                open.Sort();
                GraphNode current = open[0];

                if (current.Equals(endNode)) {
                    TNodeData pathData = current.Data;
                    path.Push(pathData);
                    while (cameFrom.ContainsKey(pathData)) {
                        pathData = cameFrom[pathData];
                        path.Push(pathData);
                    }
                    break;
                }

                open.RemoveAt(0);

                foreach (GraphNode neighbor in current.Neighbors.Keys) {
                    if (visited.Contains(neighbor)) {
                        continue;
                    }

                    // Calculate GScore by current node's accumulated cost plus the neighbor's
                    // entrance edge cost
                    int tentativeGScore = gScores[current] + current.Neighbors[neighbor];

                    // If the recorded distance is less than the neighbor distance, replace it
                    int neighborIdx = open.IndexOf(neighbor);

                    GraphNode oldNeighbor;
                    if (neighborIdx == -1) {
                        open.Add(neighbor);
                        oldNeighbor = neighbor;
                    } else {
                        oldNeighbor = open[neighborIdx];
                    }

                    int oldGScore = gScores[oldNeighbor];

                    if (neighborIdx == -1 || tentativeGScore < oldGScore) {
                        gScores[oldNeighbor] = tentativeGScore;
                        fScores[oldNeighbor] = tentativeGScore + heuristic(oldNeighbor.Data, end);
                        cameFrom[oldNeighbor.Data] = current.Data;
                    }
                }

                visited.Add(current);
            }

            return path;
        }
    }
}
