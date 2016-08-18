using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LostGen.Test {
    public class PathfinderTests {

        private class TestNode : IGraphNode, IComparable<TestNode> {
            public char Value { get; private set; }
            public Dictionary<IGraphNode, int> _neighbors = new Dictionary<IGraphNode, int>();

            public TestNode(char value) {
                Value = value;
            }

            public void AddNeighbor(TestNode neighbor, int edgeCost) {
                _neighbors[neighbor] = edgeCost;
            }

            public int GetEdgeCost(IGraphNode to) {
                TestNode neighbor = (TestNode)to;
                int cost;

                if (neighbor == null) {
                    throw new InvalidCastException("Given node is not a valid TestNode");
                } else {
                    cost = _neighbors[neighbor];
                }

                return cost;
            }

            public IEnumerable<IGraphNode> GetNeighbors() {
                return _neighbors.Keys;
            }

            public bool IsMatch(IGraphNode other) {
                TestNode otherNode = (TestNode)other;
                return Value == otherNode.Value;
            }

            public static string PathToString(IEnumerable<TestNode> path) {
                string pathStr = string.Empty;
                foreach (TestNode node in path) {
                    pathStr += node.Value;
                }
                return pathStr;
            }

            public int CompareTo(TestNode other) {
                return Value.CompareTo(other.Value);
            }
        }

        [Test]
        public void StraightShot() {
            //Arrange
            TestNode nodeA = new TestNode('A');
            TestNode nodeB = new TestNode('B');
            TestNode nodeC = new TestNode('C');
            TestNode nodeD = new TestNode('D');
            TestNode nodeE = new TestNode('E');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            nodeD.AddNeighbor(nodeE, 10);
            nodeE.AddNeighbor(nodeD, 10);

            List<TestNode> path = new List<TestNode>(
                Pathfinder<TestNode>.FindPath(nodeA, nodeE, delegate (TestNode start, TestNode end) {
                    return 10 * Math.Abs(end.Value - start.Value);
                })
            );

            string pathStr = string.Empty;
            for (int i = 0; i < path.Count; i++) {
                pathStr += path[i].Value;
            }

            Assert.AreEqual(pathStr, "ABCDE");
        }

        [Test]
        public void NoPath() {
            //Arrange
            TestNode nodeA = new TestNode('A');
            TestNode nodeB = new TestNode('B');
            TestNode nodeC = new TestNode('C');
            TestNode nodeD = new TestNode('D');
            TestNode nodeE = new TestNode('E');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            //nodeD.AddNeighbor(nodeE, 10);
            nodeE.AddNeighbor(nodeD, 10);

            string pathStr = TestNode.PathToString(
                Pathfinder<TestNode>.FindPath(nodeA, nodeE, delegate (TestNode start, TestNode end) {
                    return 10 * Math.Abs(end.Value - start.Value);
                })
            );

            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void SimpleBranches() {
            //Arrange
            TestNode nodeA = new TestNode('A');
            TestNode nodeB = new TestNode('B');
            TestNode nodeC = new TestNode('C');
            TestNode nodeD = new TestNode('D');
            TestNode nodeE = new TestNode('E');

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

            string pathStr = TestNode.PathToString(
                Pathfinder<TestNode>.FindPath(nodeA, nodeE, delegate (TestNode start, TestNode end) {
                    return 10 * Math.Abs(end.Value - start.Value);
                })
            );
            
            Assert.AreEqual(pathStr, "ABDE");
        }

        [Test]
        public void StartNotInGraph() {
            //Arrange
            TestNode nodeA = new TestNode('A');
            TestNode nodeB = new TestNode('B');
            TestNode nodeC = new TestNode('C');
            TestNode nodeD = new TestNode('D');

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            nodeC.AddNeighbor(nodeD, 10);
            nodeD.AddNeighbor(nodeC, 10);

            string pathStr = TestNode.PathToString(
                Pathfinder<TestNode>.FindPath(nodeA, nodeD, delegate (TestNode start, TestNode end) {
                    return 10 * Math.Abs(end.Value - start.Value);
                })
            );
            
            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void EndNotInGraph() {
            TestNode nodeA = new TestNode('A');
            TestNode nodeB = new TestNode('B');
            TestNode nodeC = new TestNode('C');
            TestNode nodeD = new TestNode('D');

            nodeA.AddNeighbor(nodeB, 10);
            nodeB.AddNeighbor(nodeA, 10);

            nodeB.AddNeighbor(nodeC, 10);
            nodeC.AddNeighbor(nodeB, 10);

            string pathStr = TestNode.PathToString(
                Pathfinder<TestNode>.FindPath(nodeA, nodeD, delegate (TestNode start, TestNode end) {
                    return 10 * Math.Abs(end.Value - start.Value);
                })
            );

            Assert.IsNullOrEmpty(pathStr);
        }

        [Test]
        public void FloodFillDepthStraightLine() {
            TestNode[] nodes = new TestNode[26];
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = new TestNode((char)((int)'A' + i));
                if (i > 0) {
                    nodes[i].AddNeighbor(nodes[i - 1], 10);
                    nodes[i - 1].AddNeighbor(nodes[i], 10);
                }
            }

            List<TestNode> depthDomain = new List<TestNode>(
                Pathfinder<TestNode>.FloodFill(nodes[0], -1, 10)
            );
            depthDomain.Sort();
            string depthString = TestNode.PathToString(depthDomain);

            List<TestNode> costDomain = new List<TestNode>(Pathfinder<TestNode>.FloodFill(nodes[0], 100, -1));
            costDomain.Sort();
            string costString = TestNode.PathToString(costDomain);

            Assert.AreEqual("ABCDEFGHIJ", depthString);
            Assert.AreEqual("ABCDEFGHIJ", costString);
        }
    }
}