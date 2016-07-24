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

        private Board _board;

        private List<Point> _path = null;

        public Walk(Combatant source, Point destination)
            : base(source, 1) {
            _destination = destination;
            _board = Source.Board;
        }

        private int Heuristic(Point start, Point end) {
            // just use manhattan distance lol
            return Point.TaxicabDistance(start, end);
        }

        private List<Point> FindPath(Point start, Point end) {

            HashSet<Point> closedSet = new HashSet<Point>();
            HashSet<Point> openSet = new HashSet<Point>();
            openSet.Add(start);

            Dictionary<Point, int> gScore = new Dictionary<Point, int>();
            gScore.Add(start, 0);

            SortedList<int, Point> fScore = new SortedList<int, Point>();
            fScore.Add(Heuristic(start, end), start);
            Point lowest = start;

            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            Point neighbor;
            int tentativeGScore = 0;
            int tentativeFScore = 0;
            List<Point> finalPath = null;

            while (openSet.Count > 0) {
                lowest = fScore.Values[0];
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

                    if (closedSet.Contains(neighbor) ||
                        _board.GetTile(neighbor) == Board.WALL_TILE) {
                        continue;
                    }

                    tentativeGScore = gScore[lowest] + 1; // replace 1 with obstruction costs, etc

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
                    fScore.Add(tentativeFScore, neighbor);
                }
            }

            return finalPath;
        }

        public override void Fire() {
            
        }
    }

}
