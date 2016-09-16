using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace LostGen.Test {
    public class ApproachAndAttack {

        [Test]
        public void StationaryMelee() {
            Board board = new Board(BoardCommon.GRID_12X8);
            Combatant attacker = new Combatant("Attacker", board, new Point(5, 4));
            Combatant defender = new Combatant("Defender", board, new Point(7, 4));
            board.AddPawn(attacker);
            board.AddPawn(defender);

            attacker.Health = 10;
            defender.Health = 10;

            attacker.BaseStats = new Stats() {
                Attack = 10,
                Stamina = 10
            };

            MeleeAttackSkill attack = new MeleeAttackSkill(attacker, new Point[] { Point.Right, 2 * Point.Right }) {
                ActionPoints = 3,
                Direction = CardinalDirection.East
            };
            attacker.AddSkill(attack);
            attack.Fire();

            board.BeginTurn();
            Assert.AreEqual(10, attacker.ActionPoints);

            board.Turn();
            Assert.AreEqual(0, defender.Health);
            Assert.AreEqual(7, attacker.ActionPoints);
        }

        [Test]
        public void ApproachMeleeRange() {
            Board board = new Board( new int[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            });
            Combatant attacker = new Combatant("Attacker", board, new Point(1, 7));
            Combatant defender = new Combatant("Defender", board, new Point(10, 1));
            board.AddPawn(attacker);
            board.AddPawn(defender);

            attacker.Health = 10;
            defender.Health = 10;

            attacker.BaseStats = new Stats() {
                Attack = 10,
                Stamina = 25
            };

            attacker.AddPawnToView(defender);

            MeleeAttackSkill attack = new MeleeAttackSkill(attacker, new Point[] { Point.Right, 2 * Point.Right }) {
                ActionPoints = 3
            };
            attacker.AddSkill(attack);
            WalkSkill walk = new WalkSkill(attacker);
            attacker.AddSkill(walk);

            Decision.ApproachMeleeRange approach = new Decision.ApproachMeleeRange(attacker);
            approach.Target = defender;

            approach.Setup();
            approach.Run();

            board.BeginTurn();
            Assert.AreEqual(25, attacker.ActionPoints);

            board.Turn();
            Assert.AreEqual(new Point(10, 3), attacker.Position);
        }
    }
}