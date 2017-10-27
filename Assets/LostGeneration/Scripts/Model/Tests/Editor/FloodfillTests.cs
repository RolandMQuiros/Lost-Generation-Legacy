using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LostGen;

namespace Tests {
    public class FloodfillTests {
        private class TestGridNode : IGraphNode<TestGridNode> {
            public Point Point { get; private set; }

            private int[,] _grid;
            private Dictionary<Point, TestGridNode> _neighbors = new Dictionary<Point, TestGridNode>();
            private bool _neighborsInitialized = false;

            public TestGridNode(int[,] grid, Point point) {
                _grid = grid;
                Point = point;
            }

            public int GetEdgeCost(TestGridNode neighbor) {
                return Point.TaxicabDistance(Point, neighbor.Point);
            }

            public IEnumerable<TestGridNode> GetNeighbors() {
                //Profiler.BeginSample("TestGridNode.GetNeighbors");
                if (!_neighborsInitialized) {
                    for (int i = 0; i < Point.NeighborsXY.Length; i++) {
                        Point point = Point + Point.NeighborsXY[i];
                        if (!_neighbors.ContainsKey(point) && _grid[point.Y, point.X] == 1) {
                            TestGridNode neighbor = new TestGridNode(_grid, point);
                            neighbor._neighbors.Add(Point, this);
                            _neighbors.Add(point, neighbor);
                        }
                    }
                    _neighborsInitialized = true;
                }
                //Profiler.EndSample();
                return _neighbors.Values;
            }

            public bool IsMatch(TestGridNode other) {
                return other.Point == Point;
            }

            public override int GetHashCode() {
                return Point.GetHashCode();
            }
        }

        private string FillString(int [,] grid, Point point, int cost = -1, int depth = -1) {
            TestGridNode root = new TestGridNode(grid, point);

            HashSet<Point> filled = new HashSet<Point>();
            foreach (TestGridNode node in GraphMethods.FloodFill<TestGridNode>(root, cost, depth)) {
                filled.Add(node.Point);
            }

            string gridStr = string.Empty;
            for (int y = 0; y < grid.GetLength(0); y++) {
                string line = string.Empty;
                for (int x = 0; x < grid.GetLength(1); x++) {
                    if (filled.Contains(new Point(x, y))) {
                        line += '╬';
                    } else {
                        switch (grid[y, x]) {
                            case 0: line += '█'; break;
                            case 1: line += '░'; break;
                        }
                    }
                }
                gridStr += line + '\n';
            }

            return gridStr;
        }

        [Test]
        public void CostEqualsTwo() {
            int[,] grid = new int[,] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

            string fillStr = FillString(grid, new Point(4, 3), 2);
            Console.Write(fillStr);

            string expected =
                "█████████\n" +
                "█░░░╬░░░█\n" +
                "█░░╬╬╬░░█\n" +
                "█░╬╬╬╬╬░█\n" +
                "█░░╬╬╬░░█\n" +
                "█░░░╬░░░█\n" +
                "█████████\n";

            Assert.AreEqual(expected, fillStr, "Expected:\n" + expected + "\nActual:\n" + fillStr);
        }

        [Test]
        public void CostEqualsThreeWithObstacles() {
            int[,] grid = new int[,] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 0, 1, 1, 0},
                {0, 1, 1, 1, 1, 0, 1, 1, 0},
                {0, 1, 1, 0, 1, 1, 1, 1, 0},
                {0, 1, 1, 1, 1, 1, 1, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

            string fillStr = FillString(grid, new Point(4, 3), 3);
            Console.Write(fillStr);

            string expected =
                "█████████\n" +
                "█░░╬╬╬░░█\n" +
                "█░╬╬╬█░░█\n" +
                "█╬╬╬╬█░░█\n" +
                "█░╬█╬╬╬░█\n" +
                "█░░╬╬╬░░█\n" +
                "█████████\n";
            
            Assert.AreEqual(expected, fillStr, "Expected:\n" + expected + "\nActual:\n" + fillStr);
        }

        [Test]
        public void BigBoard() {
            int[,] grid = new int[,] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            string fillStr = FillString(grid, new Point(3, 14), 6);
            Console.Write(fillStr);
        }
    }
}