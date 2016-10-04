using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LostGen;

namespace Tests {
    public class FloodfillTests {
        private class TestGridNode : IGraphNode {
            public Point Point { get; private set; }

            private int[,] _grid;
            private List<IGraphNode> _neighbors;

            public TestGridNode(int[,] grid, Point point) {
                _grid = grid;
                Point = point;
            }

            public int GetEdgeCost(IGraphNode neighbor) {
                return Point.TaxicabDistance(Point, ((TestGridNode)neighbor).Point);
            }

            public IEnumerable<IGraphNode> GetNeighbors() {
                Profiler.BeginSample("TestGridNode.GetNeighbors");
                if (_neighbors == null) {
                    _neighbors = new List<IGraphNode>();
                    for (int i = 0; i < Point.Neighbors.Length; i++) {
                        Point point = Point + Point.Neighbors[i];
                        if (_grid[point.Y, point.X] == 1) {
                            _neighbors.Add(new TestGridNode(_grid, point));
                        }
                    }
                }
                Profiler.EndSample();
                return _neighbors;
            }

            public bool IsMatch(IGraphNode other) {
                return ((TestGridNode)other).Point == Point;
            }

            public override int GetHashCode() {
                return Point.GetHashCode();
            }
        }

        private string FillString(int [,] grid, Point point, int cost = -1, int depth = -1) {
            TestGridNode root = new TestGridNode(grid, point);

            HashSet<Point> filled = new HashSet<Point>();
            foreach (TestGridNode node in GraphMethods<TestGridNode>.FloodFill(root, cost, depth)) {
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
                "█░░░░░░░█\n" +
                "█░░░╬░░░█\n" +
                "█░░╬╬╬░░█\n" +
                "█░░░╬░░░█\n" +
                "█░░░░░░░█\n" +
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
                "█░░░╬░░░█\n" +
                "█░░╬╬█░░█\n" +
                "█░╬╬╬█░░█\n" +
                "█░░█╬╬░░█\n" +
                "█░░░╬░░░█\n" +
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