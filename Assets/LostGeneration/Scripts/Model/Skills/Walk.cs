using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LostGen.Skills {

    public class Walk : Skill {
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
            _path = FindPath(Source.Position, _destination);
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

        protected virtual int Heuristic(Point start, Point end) {
            // just use manhattan distance lol
            return Point.TaxicabDistance(start, end);
        }

        protected virtual int TileCost(Board board, Point point) {
            int cost = 1;
            return cost;
        }

        protected virtual List<Point> FindPath(Point start, Point end) {

            HashSet<Point> closedSet = new HashSet<Point>();
            HashSet<Point> openSet = new HashSet<Point>();
            openSet.Add(start);

            Dictionary<Point, int> gScore = new Dictionary<Point, int>();
            gScore.Add(start, 0);

            List<KeyValuePair<int, Point>> fScore = new List<KeyValuePair<int, Point>>();
            fScore.Add(new KeyValuePair<int, Point>(Heuristic(start, end), start));
            Point lowest = start;
            
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            Point neighbor;
            int tentativeGScore = 0;
            int tentativeFScore = 0;
            List<Point> finalPath = null;

            while (openSet.Count > 0) {
                fScore.Sort((x, y) => x.Key.CompareTo(y.Key));
                lowest = fScore[0].Value;
                if (lowest.Equals(end)) {
                    // reconstruct path
                    finalPath = new List<Point>();
                    while (cameFrom.ContainsKey(lowest)) {
                        lowest = cameFrom[lowest];
                        finalPath.Add(lowest);
                    }
                    break;
                }

                openSet.Remove(lowest);
                closedSet.Add(lowest);
                for (int i = 0; i < Point.OctoNeighbors.Length; i++) {
                    neighbor = lowest + Point.OctoNeighbors[i];

                    if (_board.InBounds(neighbor) &&
                        closedSet.Contains(neighbor) ||
                        _board.GetTile(neighbor) == Board.WALL_TILE) {
                        continue;
                    }

                    tentativeGScore = gScore[lowest] + TileCost(_board, lowest);

                    if (!gScore.ContainsKey(neighbor)) {
                        gScore.Add(neighbor, Int32.MaxValue);
                    }

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    } else if (tentativeGScore >= gScore[neighbor]) {
                        continue;
                    }

                    cameFrom.Add(neighbor, lowest);
                    gScore[neighbor] = tentativeGScore;
                    tentativeFScore = tentativeGScore + Heuristic(neighbor, start);
                    fScore.Add(new KeyValuePair<int, Point>(tentativeFScore, neighbor));
                }
            }

            return finalPath;
        }
    }

}
