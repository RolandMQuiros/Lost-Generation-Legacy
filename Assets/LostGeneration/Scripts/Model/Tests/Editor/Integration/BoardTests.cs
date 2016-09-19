using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.Integration {
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
        public void PawnMovedButNotInBoard() {
            Board board = new Board(BoardCommon.GRID_12X8);
            Pawn pawn = new Pawn("Add", board, new Point(board.Width / 2, board.Height / 2));

            // Expect an ArgumentException because the pawn doesn't exist on the board yet
            Assert.Throws<ArgumentException>(delegate () {
                pawn.Offset(Point.One);
            }, "Offset was called on a Pawn that was not added to a Board");
        }
    }
}