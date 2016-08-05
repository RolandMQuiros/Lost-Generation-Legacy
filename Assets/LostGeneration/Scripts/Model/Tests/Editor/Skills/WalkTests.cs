using UnityEngine;
using UnityEditor;
using NUnit.Framework;

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

            Assert.AreEqual(destination, combatant.Position);
        }
    }

}