using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.Decisions {
    public class ApproachAndAttack {
        [Test]
        public void StationaryMelee() {
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);
            Pawn attackerPawn = new Pawn("Attacker", board, new Point(5, 4));
            Combatant attacker = attackerPawn.AddComponent<Combatant>();

            Pawn defenderPawn = new Pawn("Defender", board, new Point(7, 4));
            Combatant defender = defenderPawn.AddComponent<Combatant>();

            board.AddPawn(attackerPawn);
            board.AddPawn(defenderPawn);

            attacker.Health = 10;
            defender.Health = 10;

            attacker.BaseStats = new Stats() {
                Attack = 10,
                Stamina = 10
            };

            MeleeAttackSkill attack = new MeleeAttackSkill(attacker, new Point[] { Point.Right, 2 * Point.Right }) {
                ActionPoints = 3
            };
            attack.SetDirection(CardinalDirection.East);

            attacker.AddSkill(attack);
            attack.Fire();

            board.BeginTurn();
            Assert.AreEqual(10, attacker.ActionPoints);

            Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
            board.Turn(messages);
            Assert.AreEqual(0, defender.Health);
            Assert.AreEqual(7, attacker.ActionPoints);
        }

        [Test]
        public void ApproachMeleeRange() {
            Board board = BoardCommon.ArrayToBoard( new int[,,] {{
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }});
            Pawn attackerPawn = new Pawn("Attacker", board, new Point(1, 7));
            Combatant attacker = attackerPawn.AddComponent<Combatant>();

            Pawn defenderPawn = new Pawn("Defender", board, new Point(10, 1));
            Combatant defender = defenderPawn.AddComponent<Combatant>();

            board.AddPawn(attackerPawn);
            board.AddPawn(defenderPawn);

            attacker.Health = 10;
            defender.Health = 10;

            attacker.BaseStats = new Stats() {
                Attack = 10,
                Stamina = 25
            };

            attacker.AddPawnToView(defenderPawn);

            MeleeAttackSkill attack = new MeleeAttackSkill(attacker, new Point[] { Point.Right, 2 * Point.Right }) {
                ActionPoints = 3
            };
            attacker.AddSkill(attack);
            WalkSkill walk = new WalkSkill(attacker);
            attacker.AddSkill(walk);

            LostGen.Decision.ApproachMeleeRange approach = new LostGen.Decision.ApproachMeleeRange(attacker);
            approach.Target = defender;

            approach.Setup();
            approach.Run();

            board.BeginTurn();
            Assert.AreEqual(25, attacker.ActionPoints);

            Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
            board.Turn(messages);
            Assert.AreEqual(new Point(10, 3), attackerPawn.Position);
        }
    }
}