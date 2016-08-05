using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LostGen.Skills {

    public class Walk : Skill {
        private class Node : IComparable<Node>, IEquatable<Node> {
            public Point Point;
            public int GScore;
            public int FScore;

            public Node() {
                Point = Point.Zero;
                GScore = 0;
                FScore = 0;
            }

            public int CompareTo(Node other) {
                int compare = 0;
                if (FScore > other.FScore) {
                    compare = 1;
                } else if (FScore < other.FScore) {
                    compare = -1;
                }

                return compare;
            }

            public bool Equals(Node other) {
                return Point.Equals(other.Point);
            }
        }

        private int _cost;
        public override int Cost {
            get { return _cost; }
        }

        private Point _destination;
        public Point Destination { get { return _destination; } }

        private List<Point> _path;
        private Board _board;

        public Walk(Combatant source)
            : base(source, "Walk", "Move across tiles within a limited range", 1) {
            _board = Source.Board;
        }

        public void SetDestination(Point destination) {
            _destination = destination;
            _path = new List<Point>(FindPath(_board, Source.Position, _destination));
        }

        public ReadOnlyCollection<Point> GetPath() {
            return _path.AsReadOnly();
        }

        public override void Fire() {
            if (_path != null) {
                Actions.Move move;
                for (int i = 0; i < _path.Count; i++) {
                    move = new Actions.Move(Source, _path[i], true);
                    Source.PushAction(move);
                }
            }
        }

        public static int NodeTest() {
            Node node1 = new Node() { Point = new Point(1, 1), FScore = 5 };
            Node node2 = new Node() { Point = new Point(1, 1), FScore = 10 };

            List<Node> list = new List<Node>();
            list.Add(node1);

            return list.IndexOf(node2);
        }

        protected virtual int Heuristic(Point start, Point end) {
            // just use manhattan distance lol
            return Point.TaxicabDistance(start, end);
        }

        protected virtual int TileCost(Board board, Point point) {
            int cost = 1;
            return cost;
        }

        protected virtual IEnumerable<Point> FindPath(Board board, Point start, Point end) {
            HashSet<Point> visited = new HashSet<Point>();
            List<Node> open = new List<Node>();
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
            
            Stack<Point> path = new Stack<Point>();

            open.Add(new Node() { Point = start, FScore = Heuristic(start, end) } );

            int iterations = 0;
            int maxIterations = 1000;

            while (open.Count > 0) {
                open.Sort();
                Node current = open[0];

                if (current.Point.Equals(end)) {
                    Point pathPoint = current.Point;
                    path.Push(pathPoint);
                    while (cameFrom.ContainsKey(pathPoint)) {
                        pathPoint = cameFrom[pathPoint];
                        path.Push(pathPoint);
                    }
                    break;
                }

                open.RemoveAt(0);

                for (int i = 0; i < Point.OctoNeighbors.Length; i++) {
                    Node neighbor = new Node();
                    neighbor.Point = current.Point + Point.OctoNeighbors[i];

                    if (visited.Contains(neighbor.Point) ||
                        !_board.InBounds(neighbor.Point) ||
                        _board.GetTile(neighbor.Point) == Board.WALL_TILE) {
                        continue;
                    }

                    neighbor.GScore = current.GScore + TileCost(board, neighbor.Point);

                    // If the recorded distance is less than the neighbor distance, replace it
                    int neighborIdx = open.IndexOf(neighbor);

                    Node oldNeighbor;
                    if (neighborIdx == -1) {
                        open.Add(neighbor);
                        oldNeighbor = neighbor;
                    } else {
                        oldNeighbor = open[neighborIdx];
                    }

                    if (neighborIdx == -1 || neighbor.GScore < oldNeighbor.GScore) {
                        oldNeighbor.GScore = neighbor.GScore;
                        oldNeighbor.FScore = neighbor.GScore + Heuristic(oldNeighbor.Point, end);
                        cameFrom[oldNeighbor.Point] = current.Point;
                    }
                }

                visited.Add(current.Point);

                iterations++;
                if (iterations > maxIterations) {
                    break;
                }
            }

            return path;
        }
    }

}
