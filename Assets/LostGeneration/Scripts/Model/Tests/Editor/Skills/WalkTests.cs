using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using System.Collections.Generic;

namespace LostGen.Test {
    [TestFixture]
    public class WalkTests {
        
        [SetUp]
        public void Init() {

        }

        [Test]
        public void PathfindingTest() {
            Board board = new Board(BoardCommon.GRID_12X8);
            Combatant combatant = new Combatant("Walker", board, Point.One);

            board.AddPawn(combatant);

            Skills.Walk walk = new Skills.Walk(combatant);
            combatant.AddSkill(walk);

            Point destination = new Point(10, 6);
            walk.SetDestination(destination);

            combatant.FireSkill("Walk");

            board.Turn();

            
            List<Point> points = new List<Point>(walk.GetPath());
            string str = string.Empty;
            for (int i = 0; i < points.Count; i++) {
                str += points[i] + "\n";
            }
            Assert.Pass("points: " + str);

            Assert.AreEqual(destination, combatant.Position);
        }

        [Test]
        public void NodeTest() {
            Assert.AreEqual(0, LostGen.Skills.Walk.NodeTest());
        }
    }

}