using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LostGen.Test {
    [TestFixture]
    public class WalkTests {

        private void ArrangeBoard(int[,] grid, Point start, Point end, out Board board, out Combatant pawn) {
            board = new Board(grid);
            pawn = new Combatant("Walker", board, Point.One);

            Stats stats = new Stats() { Stamina = 100 };
            pawn.BaseStats = stats;

            board.AddPawn(pawn);

            Skills.Walk walk = new Skills.Walk(pawn);
            pawn.AddSkill(walk);

            walk.SetDestination(end);

            board.BeginTurn();
        }

        [Test]
        public void CornerToCorner() {
            Board board;
            Combatant combatant;
            Point start = new Point(1, 1);
            Point end = new Point(10, 6);
            ArrangeBoard(BoardCommon.GRID_12X8, start, end, out board, out combatant);

            Skills.Walk walk = combatant.GetSkill("Walk") as Skills.Walk;

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

            combatant.FireSkill("Walk");

            board.Turn();

            Console.Write(BoardCommon.PrintGrid(BoardCommon.GRID_12X8, walk.GetPath()));
            Assert.AreEqual(end, combatant.Position);
        }

        [Test]
        public void CenterWall() {
            int[,] grid = new int[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            Board board;
            Combatant combatant;
            Point start = new Point(1, 1);
            Point end = new Point(10, 6);
            ArrangeBoard(grid, start, end, out board, out combatant);

            Skills.Walk walk = combatant.GetSkill("Walk") as Skills.Walk;

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);
            
            combatant.FireSkill("Walk");

            board.Turn();

            Console.Write(BoardCommon.PrintGrid(grid, walk.GetPath()));
            Assert.AreEqual(end, combatant.Position);
        }

        [Test]
        public void Maze() {
            int[,] grid = new int[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0 },
                { 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0 },
                { 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            Board board;
            Combatant combatant;
            Point start = new Point(1, 1);
            Point end = new Point(10, 6);
            ArrangeBoard(grid, start, end, out board, out combatant);

            Skills.Walk walk = combatant.GetSkill("Walk") as Skills.Walk;

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

            combatant.FireSkill("Walk");

            board.Turn();

            Console.Write(BoardCommon.PrintGrid(grid, walk.GetPath()));
            
            Assert.AreEqual(end, combatant.Position);
        }
    }

}