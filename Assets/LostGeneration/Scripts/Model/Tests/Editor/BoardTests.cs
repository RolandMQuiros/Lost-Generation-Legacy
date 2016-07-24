using System;
using NUnit.Framework;

namespace LostGen.Test {
    [TestFixture]
    public class BoardTests {
        private readonly int[,] _GRID_12X8 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

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
            Board board = new Board(_GRID_12X8);
            Pawn pawn = new Pawn("Add", board, new Point(board.Width / 2, board.Height / 2));

            pawn.Offset(Point.One);
        }

        
    }
}