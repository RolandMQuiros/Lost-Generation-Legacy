using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LostGen.Test {
    public class PathfinderTests {

        private class TestGraphNode : GraphNode<char> {
            private char _value;
            public Dictionary<GraphNode<char>, int> _neighbors = new Dictionary<GraphNode<char>, int>();

            public TestGraphNode(char value) {
                _value = value;
            }

            public void AddNeighbor(TestGraphNode neighbor, int edgeCost) {
                _neighbors[neighbor] = edgeCost;
            }

            public override char GetData() {
                return _value;
            }

            public override int GetEdgeCost(GraphNode<char> to) {
                TestGraphNode neighbor = to as TestGraphNode;
                int cost;

                if (neighbor == null) {
                    throw new InvalidCastException("Given node is not a valid TestNode");
                } else {
                    cost = _neighbors[neighbor];
                }

                return cost;
            }

            public override IEnumerable<GraphNode<char>> GetNeighbors() {
                return _neighbors.Keys;
            }
        }

        [Test]
        public void StraightShot() {
            //Arrange
            TestGraphNode nodeA = new TestGraphNode('A');
            TestGraphNode nodeB = new TestGraphNode('B');
            TestGraphNode nodeC = new TestGraphNode('C');
            TestGraphNode nodeD = new TestGraphNode('D');
            TestGraphNode nodeE = new TestGraphNode('E');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            nodeD.AddNeighbor(nodeE, 10);
            nodeE.AddNeighbor(nodeD, 10);

            List<char> path = new List<char>(
                Pathfinder<char>.FindPath(nodeA, nodeE, delegate (char start, char end) {
                    return 10 * Math.Abs(end - start);
                })
            );

            string pathStr = new string(path.ToArray());
            Assert.AreEqual(pathStr, "ABCDE");
        }

        [Test]
        public void NoPath() {
            //Arrange
            TestGraphNode nodeA = new TestGraphNode('A');
            TestGraphNode nodeB = new TestGraphNode('B');
            TestGraphNode nodeC = new TestGraphNode('C');
            TestGraphNode nodeD = new TestGraphNode('D');
            TestGraphNode nodeE = new TestGraphNode('E');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            //nodeD.AddNeighbor(nodeE, 10);
            nodeE.AddNeighbor(nodeD, 10);

            List<char> path = new List<char>(
                Pathfinder<char>.FindPath(nodeA, nodeE, delegate (char start, char end) {
                    return 10 * Math.Abs(end - start);
                })
            );

            string pathStr = new string(path.ToArray());
            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void SimpleBranches() {
            //Arrange
            TestGraphNode nodeA = new TestGraphNode('A');
            TestGraphNode nodeB = new TestGraphNode('B');
            TestGraphNode nodeC = new TestGraphNode('C');
            TestGraphNode nodeD = new TestGraphNode('D');
            TestGraphNode nodeE = new TestGraphNode('E');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeB.AddNeighbor(nodeD, 5);
            nodeD.AddNeighbor(nodeB, 5);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            nodeD.AddNeighbor(nodeE, 10);
            nodeE.AddNeighbor(nodeD, 10);

            List<char> path = new List<char>(
                Pathfinder<char>.FindPath(nodeA, nodeE, delegate (char start, char end) {
                    return 10 * Math.Abs(end - start);
                })
            );

            string pathStr = new string(path.ToArray());
            Assert.AreEqual(pathStr, "ABDE");
        }

        [Test]
        public void StartNotInGraph() {
            //Arrange
            TestGraphNode nodeA = new TestGraphNode('A');
            TestGraphNode nodeB = new TestGraphNode('B');
            TestGraphNode nodeC = new TestGraphNode('C');
            TestGraphNode nodeD = new TestGraphNode('D');

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            List<char> path = new List<char>(
                Pathfinder<char>.FindPath(nodeA, nodeD, delegate (char start, char end) {
                    return 10 * Math.Abs(end - start);
                })
            );

            string pathStr = new string(path.ToArray());
            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void EndNotInGraph() {
            TestGraphNode nodeA = new TestGraphNode('A');
            TestGraphNode nodeB = new TestGraphNode('B');
            TestGraphNode nodeC = new TestGraphNode('C');
            TestGraphNode nodeD = new TestGraphNode('D');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            List<char> path = new List<char>(
                Pathfinder<char>.FindPath(nodeA, nodeD, delegate (char start, char end) {
                    return 10 * Math.Abs(end - start);
                })
            );

            string pathStr = new string(path.ToArray());
            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void FloodFillDepthStraightLine() {
            TestGraphNode[] nodes = new TestGraphNode[26];
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = new TestGraphNode((char)((int)'A' + i));
                if (i > 0) {
                    nodes[i].AddNeighbor(nodes[i - 1], 10);
                    nodes[i - 1].AddNeighbor(nodes[i], 10);
                }
            }

            List<char> depthDomain = new List<char>(Pathfinder<char>.FloodFill(nodes[0], -1, 10));
            depthDomain.Sort();
            string depthString = new string(depthDomain.ToArray());

            List<char> costDomain = new List<char>(Pathfinder<char>.FloodFill(nodes[0], 100, -1));
            costDomain.Sort();
            string costString = new string(costDomain.ToArray());

            Assert.AreEqual("ABCDEFGHIJ", depthString);
            Assert.AreEqual("ABCDEFGHIJ", costString);
        }
    }
}