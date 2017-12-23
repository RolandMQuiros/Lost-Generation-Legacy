using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen.Model;

namespace Tests.Integration {
    [TestFixture]
    public class BoardTests {
        [Test]
        public void GridWidthAndHeight() {
            int[,,] grid = {{
                {0, 0, 0, 0 },
                {0, 0, 0, 0 },
                {0, 0, 0, 0 }
            }};

            Board board = BoardCommon.ArrayToBoard(grid);

            Assert.AreEqual(4, board.Blocks.Size.X);
            Assert.AreEqual(3, board.Blocks.Size.Z);
        }

        [Test]
        public void PawnMovedButNotInBoard() {
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);
            Pawn pawn = new Pawn("Add", board, board.Blocks.Size / 2);

            // Expect an ArgumentException because the pawn doesn't exist on the board yet
            Assert.Throws<ArgumentException>(delegate () {
                pawn.Offset(Point.One);
            }, "Offset was called on a Pawn that was not added to a Board");
        }
    }
}