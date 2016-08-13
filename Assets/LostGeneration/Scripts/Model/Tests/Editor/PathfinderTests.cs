using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LostGen.Test {
    public class PathfinderTests {

        private class TestGraphNode : AbstractGraphNode<int> {
            private int _value;
            public Dictionary<AbstractGraphNode<int>, int> _neighbors = new Dictionary<TestGraphNode, int>();

            public TestGraphNode(int value) {
                _value = value;
            }

            public void AddNeighbor(TestGraphNode neighbor, int edgeCost) {
                _neighbors[neighbor] = edgeCost;
            }

            public override int GetData() {
                return _value;
            }

            public override int GetEdgeCost(AbstractGraphNode<int> to) {
                TestGraphNode neighbor = to as TestGraphNode;
                int cost;

                if (neighbor == null) {
                    throw new InvalidCastException("Given node is not a valid TestNode");
                } else {
                    cost = _neighbors[neighbor];
                }

                return cost;
            }

            public override IEnumerator<AbstractGraphNode<int>> GetNeighborIter() {
                return _neighbors.Keys.GetEnumerator();
            }


        }

        [Test]
        public void StraightShot() {


            //Arrange
            var gameObject = new GameObject();

            //Act
            //Try to rename the GameObject
            var newGameObjectName = "My game object";
            gameObject.name = newGameObjectName;

            //Assert
            //The object has a new name
            Assert.AreEqual(newGameObjectName, gameObject.name);
        }

        [Test]
        public void NoPath() {
            Dictionary<Point, int> dict = new Dictionary<Point, int>();
        }

        [Test]
        public void SimpleBranches() {

        }

        [Test]
        public void StartNotInGraphError() {

        }

        [Test]
        public void EndNotInGraphError() {

        }
    }
}