using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.Integration {
    [TestFixture]
    public class PawnTests {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithoutBoard() {
            Pawn pawn = new Pawn("NoBoard", null, Point.Zero);

            Assert.NotNull(pawn);
        }

        [Test]
        public void MoveIntoWall() {
            Board board = new Board(BoardCommon.GRID_12X8);
            Pawn pawn = new Pawn("Mover", board, Point.One, null, true, true);

            board.AddPawn(pawn);

            Point expectedPosition = new Point(2, 1);

            bool moveRight = pawn.Offset(Point.Right);
            Point position1 = pawn.Position;

            bool moveUp = pawn.Offset(Point.Up);
            Point position2 = pawn.Position;

            Assert.IsTrue(moveRight, "Offset returned false when moving right");
            Assert.AreEqual(expectedPosition, position1, "Pawn failed to move right to open tile");

            Assert.IsFalse(moveUp, "Offset returned true when moving up into a wall");
            Assert.AreEqual(expectedPosition, position2, "Pawn failed to stay on one goddamn place");
        }

        [Test]
        public void MoveSolidPawnIntoSolidPawn() {
            Board board = new Board(BoardCommon.GRID_12X8);

            Point pos1 = new Point(6, 4);
            Point pos2 = new Point(8, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.AddPawn(pawn1);
            board.AddPawn(pawn2);

            Point expectedPosition = new Point(7, 4);

            bool firstMove = pawn2.Offset(Point.Left);
            Point firstPosition = pawn2.Position;

            bool secondMove = pawn2.Offset(Point.Left);
            Point secondPosition = pawn2.Position;
            
            Assert.IsTrue(firstMove, "Pawn was not able to move left one tile");
            Assert.IsFalse(secondMove, "Pawn somehow passed through another solid pawn");

            Assert.AreEqual(expectedPosition, firstPosition, "Pawn could not move to free tile");
            Assert.AreEqual(expectedPosition, secondPosition, "Pawn somehow passed through another solid pawn");
        }

        [Test]
        public void SolidPawnsMoveIntoEachOther() {
            Board board = new Board(BoardCommon.GRID_12X8);

            Point pos1 = new Point(6, 4);
            Point pos2 = new Point(7, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.AddPawn(pawn1);
            board.AddPawn(pawn2);

            Assert.IsFalse(pawn1.Offset(Point.Right));
            Assert.IsFalse(pawn2.Offset(Point.Left));

            Assert.AreEqual(pos1, pawn1.Position);
            Assert.AreEqual(pos2, pawn2.Position);
        }

        [Test]
        public void SolidPawnsMoveIntoSharedCell() {
            Board board = new Board(BoardCommon.GRID_12X8);

            Point pos1 = new Point(6, 4);
            Point pos2 = new Point(8, 4);

            Point pos3 = new Point(7, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.AddPawn(pawn1);
            board.AddPawn(pawn2);

            Assert.IsTrue(pawn1.Offset(Point.Right),"Pawn tried to move from " + pos1 + " to " + pos3 + " but was blocked by something");
            Assert.IsFalse(pawn2.Offset(Point.Left), "Pawn moved from " + pos2 + " to " + pos3 + " when it should've been blocked");

            Assert.AreEqual(pos3, pawn1.Position, "Pawn was not able to move from " + pos1 + " to " + pos3);
            Assert.AreEqual(pos2, pawn2.Position, "Pawn was able to move from " + pos3 + " to " + pos2);
        }

        [Test]
        public void CollisionTest() {
            // Arrange
            Board board = new Board(BoardCommon.GRID_12X8);

            Point pos1 = new Point(board.Width / 2, board.Height / 2); // Center of board
            Point pos2 = pos1 + (3 * Point.Right); // Two tiles to the right of center

            List<Point> footprint = new List<Point>(new Point[] {
                Point.Zero, Point.Up, Point.Down, Point.Left, Point.Right
            });

            Pawn pawn1 = new Pawn("First", board, pos1, footprint, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, footprint, true, true, true);

            board.AddPawn(pawn1);
            board.AddPawn(pawn2);

            bool p1Triggered = false;
            bool p2Triggered = false;

            Pawn.CollisionDelegate p1Collisions = delegate (Pawn source, Pawn other) {
                p1Triggered = true;
                Assert.AreSame(source, pawn1);
                Assert.AreSame(other, pawn2);
            };

            Pawn.CollisionDelegate p2Collisions = delegate (Pawn source, Pawn other) {
                p2Triggered = true;
                Assert.AreSame(source, pawn2);
                Assert.AreSame(other, pawn1);
            };

            pawn1.CollisionEntered += p1Collisions;
            pawn2.CollisionEntered += p2Collisions;

            // Act
            pawn1.Position = pawn1.Position + Point.Right;

            // Assert
            Assert.IsTrue(p1Triggered, "Pawn 1's OnCollisionEnter was not triggered");
            Assert.IsTrue(p2Triggered, "Pawn 2's OnCollisionEnter was not triggered");
        }
    }
}