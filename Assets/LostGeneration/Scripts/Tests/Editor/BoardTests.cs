using System;
using NUnit.Framework;

namespace LostGen.Test {
    public class BoardCommon {
        public static readonly int[,] GRID_12X8 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
    }

    [TestFixture]
    public class BoardTests {
        [Test]
        public void GridWidthAndHeight() {
            int[,] grid = {
                {0, 0, 0, 0 },
                {0, 0, 0, 0 },
                {0, 0, 0, 0 }
            };

            Board board = new Board(grid);

            Assert.AreEqual(board.Width, 4);
            Assert.AreEqual(board.Height, 3);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPawn() {
            Board board = new Board(BoardCommon.GRID_12X8);
            Pawn pawn = new Pawn("Add", board, new Point(board.Width / 2, board.Height / 2));

            pawn.Offset(Point.One);
        }

        [Test]
        public void AreaOfEffect() {
            
        }
    }
}