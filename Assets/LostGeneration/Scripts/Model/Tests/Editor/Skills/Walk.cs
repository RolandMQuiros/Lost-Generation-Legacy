using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.Skills {
    [TestFixture]
    public class Walk {
        private void ArrangeBoard(int[,,] grid, Point start, Point end, out Board board, out Combatant pawn) {
            board = BoardCommon.ArrayToBoard(grid);
            pawn = new Combatant("Walker", board, start);

            Stats stats = new Stats() { Stamina = 100 };
            pawn.BaseStats = stats;

            board.AddPawn(pawn);

            WalkSkill walk = new WalkSkill(pawn);
            pawn.AddSkill(walk);

            walk.SetTarget(end);

            board.BeginTurn();
        }

        [Test]
        public void WalkNodeNeighborsOnLevel() {
            Board board = BoardCommon.ArrayToBoard(new int[,,] {
                { // Solid floor
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                },
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 1, 1, 1, 1, 1, 0},
                    {0, 1, 0, 1, 1, 1, 0},
                    {0, 1, 0, 1, 1, 1, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                }
            });
            

            Point origin = new Point(3, 1, 2);
            WalkNode walkNode = new WalkNode(board, origin);

            HashSet<Point> actualNeighbors = new HashSet<Point>(
                walkNode.GetNeighbors().Select(node => (node as BlockNode).Point)
            );

            Point[] expectedNeighbors = {
                origin + Point.Forward + Point.Right,
                origin + Point.Forward,
                origin + Point.Right,
                origin + Point.Backward + Point.Right,
                origin + Point.Backward,
                origin + Point.Backward + Point.Left
            };

            Console.WriteLine(
                "Expected Neighbors: " +
                String.Join("; ", expectedNeighbors.Select(pt => pt.ToString()).ToArray())
            );
            Console.WriteLine(
                "Actual neighbors: " +
                String.Join("; ", actualNeighbors.Select(pt => pt.ToString()).ToArray())
            );
            Console.WriteLine(BoardCommon.PrintBoard(board, actualNeighbors));
        
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Count);
            for (int i = 0; i < expectedNeighbors.Length; i++) {
                Assert.IsTrue(actualNeighbors.Contains(expectedNeighbors[i]), "Neighbors list did not contain " + expectedNeighbors[i]);
            }
        }

        [Test]
        public void WalkNodeNeighborsLedges() {
            Board board = BoardCommon.ArrayToBoard(new int[,,] {
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                },
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 1, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                },
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 1, 1, 1, 1, 1, 0},
                    {0, 1, 0, 1, 0, 1, 0},
                    {0, 1, 0, 1, 1, 1, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                },
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 1, 1, 1, 1, 1, 0},
                    {0, 1, 1, 1, 0, 1, 0},
                    {0, 1, 0, 1, 1, 1, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                },
                {
                    {0, 0, 0, 0, 0, 0, 0},
                    {0, 1, 1, 1, 1, 1, 0},
                    {0, 1, 1, 1, 0, 1, 0},
                    {0, 1, 1, 1, 1, 1, 0},
                    {0, 0, 0, 0, 0, 0, 0},
                }
            });

            Point origin = new Point(3, 2, 2);
            WalkNode walkNode = new WalkNode(board, origin);

            HashSet<Point> actualNeighbors = new HashSet<Point>(
                walkNode.GetNeighbors().Select(node => (node as BlockNode).Point)
            );

            Point[] expectedNeighbors = {
                origin + Point.Forward + Point.Right,
                origin + Point.Forward,
                origin + Point.Forward + Point.Left + (2 * Point.Up),
                origin + Point.Left + Point.Up,
                origin + Point.Backward + Point.Right,
                origin + Point.Backward + Point.Down,
                origin + Point.Backward + Point.Left
            };

            Console.WriteLine(
                "Expected Neighbors: " +
                String.Join("; ", expectedNeighbors.Select(pt => pt.ToString()).ToArray())
            );
            Console.WriteLine(
                "Actual neighbors: " +
                String.Join("; ", actualNeighbors.Select(pt => pt.ToString()).ToArray())
            );

            Console.WriteLine(BoardCommon.PrintBoard(board, actualNeighbors));

            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Count, "Number of actual neighbors did not match expected neighbors");
            for (int i = 0; i < expectedNeighbors.Length; i++) {
                Assert.IsTrue(actualNeighbors.Contains(expectedNeighbors[i]), "Neighbors list did not contain " + expectedNeighbors[i]);
            }
        }

        public void CornerToCorner() {
            Board board;
            Combatant combatant;
            Point start = new Point(1, 1, 1);
            Point end = new Point(10, 1, 6);
            ArrangeBoard(BoardCommon.GRID_12X1X8, start, end, out board, out combatant);

            WalkSkill walk = combatant.GetSkill<WalkSkill>();

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

            walk.Fire();

            board.Turn();

            Console.Write(BoardCommon.PrintBoard(board, walk.GetPath()));
            Assert.AreEqual(end, combatant.Position);
        }

        [Test]
        public void CenterWall() {
            int[,,] space = new int[,,] {{
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }};
            int[,,] grid = new int[space.GetLength(0) + 1, space.GetLength(1), space.GetLength(2)];
            Array.Copy(space, 0, grid, space.Length, space.GetLength(1) * space.GetLength(2));

            Board board;
            Combatant combatant;
            Point start = new Point(1, 1, 1);
            Point end = new Point(10, 1, 6);
            ArrangeBoard(grid, start, end, out board, out combatant);

            WalkSkill walk = combatant.GetSkill<WalkSkill>();

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);
            
            walk.Fire();
            board.Turn();

            Console.Write(BoardCommon.PrintBoard(board, walk.GetPath()));
            Assert.AreEqual(end, combatant.Position);
        }

        [Test]
        public void Maze() {
            int[,,] space = new int[,,] {{
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0 },
                { 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 },
                { 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0 },
                { 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }};
            int[,,] grid = new int[space.GetLength(0) + 1, space.GetLength(1), space.GetLength(2)];
            Array.Copy(space, 0, grid, space.Length, space.GetLength(1) * space.GetLength(2));

            Board board;
            Combatant combatant;
            Point start = new Point(1, 1, 1);
            Point end = new Point(10, 1, 6);
            ArrangeBoard(grid, start, end, out board, out combatant);

            WalkSkill walk = combatant.GetSkill<WalkSkill>();

            Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

            walk.Fire();

            board.Turn();

            Console.Write(BoardCommon.PrintBoard(board, walk.GetPath()));
            
            Assert.AreEqual(end, combatant.Position);
        }
    }

}