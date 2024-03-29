﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen.Model;

namespace Tests.Integration {
    [TestFixture]
    public class PawnTests {
        [Test]
        public void CreateWithoutBoard() {
            Assert.Throws(
                typeof(ArgumentNullException),
                () => { new Pawn("NoBoard", null, Point.Zero); }
            );
        }

        [Test]
        public void MoveIntoWall() {
            int[,,] grid = new int[2, 8, 12];
            Array.Copy(BoardCommon.GRID_12X1X9, 0, grid, 12*8, 12 * 8);
            
            Board board = BoardCommon.ArrayToBoard(grid);
            Pawn pawn = new Pawn("Mover", board, Point.One, null, true, true);

            board.Pawns.Add(pawn);

            Point expectedPosition = new Point(2, 1, 1);

            bool moveRight = pawn.Offset(Point.Right);
            Point position1 = pawn.Position;

            bool moveBackward = pawn.Offset(Point.Backward);
            Point position2 = pawn.Position;

            Assert.IsTrue(moveRight, "Offset returned false when moving right");
            Assert.AreEqual(expectedPosition, position1, "Pawn failed to move right to open tile");

            Assert.IsFalse(moveBackward, "Offset returned true when moving backward into a wall");
            Assert.AreEqual(expectedPosition, position2, "Pawn failed to stay on one goddamn place");
        }

        [Test]
        public void MoveSolidPawnIntoSolidPawn() {
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);

            Point pos1 = new Point(6, 0, 4);
            Point pos2 = new Point(8, 0, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.Pawns.Add(pawn1);
            board.Pawns.Add(pawn2);

            Point expectedPosition = new Point(7, 0, 4);

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
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);

            Point pos1 = new Point(6, 0, 4);
            Point pos2 = new Point(7, 0, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.Pawns.Add(pawn1);
            board.Pawns.Add(pawn2);

            Assert.IsFalse(pawn1.Offset(Point.Right));
            Assert.IsFalse(pawn2.Offset(Point.Left));

            Assert.AreEqual(pos1, pawn1.Position);
            Assert.AreEqual(pos2, pawn2.Position);
        }

        [Test]
        public void SolidPawnsMoveIntoSharedCell() {
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);

            Point pos1 = new Point(6, 0, 4);
            Point pos2 = new Point(8, 0, 4);

            Point pos3 = new Point(7, 0, 4);

            Pawn pawn1 = new Pawn("First", board, pos1, null, true, true, true);
            Pawn pawn2 = new Pawn("Second", board, pos2, null, true, true, true);

            board.Pawns.Add(pawn1);
            board.Pawns.Add(pawn2);

            Assert.IsTrue(pawn1.Offset(Point.Right),"Pawn tried to move from " + pos1 + " to " + pos3 + " but was blocked by something");
            Assert.IsFalse(pawn2.Offset(Point.Left), "Pawn moved from " + pos2 + " to " + pos3 + " when it should've been blocked");

            Assert.AreEqual(pos3, pawn1.Position, "Pawn was not able to move from " + pos1 + " to " + pos3);
            Assert.AreEqual(pos2, pawn2.Position, "Pawn was able to move from " + pos3 + " to " + pos2);
        }

        [Test]
        public void PawnFootprintOutsideBoard() {
            Board board = BoardCommon.ArrayToBoard(BoardCommon.GRID_12X1X9);

            Assert.Throws(
                typeof(ArgumentOutOfRangeException),
                () => {
                    new Pawn(
                        "Footprint",
                        board,
                        new Point(6, 0, 4),
                        new Point[] { Point.Zero, Point.Up }
                    );
                }
            );
        }


        private class TestComponent : PawnComponent {
            public event Action<Pawn, Pawn> CollisionEntered;
            public event Action<Pawn, Pawn> CollisionStayed;
            public event Action<Pawn, Pawn> CollisionExited;
            
            public override void OnCollisionEnter(Pawn other) {
                if (CollisionEntered != null) { CollisionEntered(Pawn, other); }
            }
            public override void OnCollisionStay(Pawn other) {
                if (CollisionStayed != null) { CollisionStayed(Pawn, other); }
            }
            public override void OnCollisionExit(Pawn other) {
                if (CollisionExited != null) { CollisionExited(Pawn, other); }
            }
        }

        [Test]
        public void CollisionTest() {
            // Arrange
            int[,,] grid = new int[2, 8, 12];
            Array.Copy(BoardCommon.GRID_12X1X9, 0, grid, 12*8, 12 * 8);
            Board board = BoardCommon.ArrayToBoard(grid);

            Point pos1 = new Point(6, 1, 4); // Center of board
            Point pos2 = pos1 + (3 * Point.Right); // Two tiles to the right of center

            // TODO: Write a test where the footprint falls outside the board
            List<Point> footprint = new List<Point>(new Point[] {
                Point.Zero, Point.Forward, Point.Backward, Point.Left, Point.Right
            });

            Pawn pawn1 = new Pawn("First", board, pos1, footprint, true, true, true);
            TestComponent test1 = pawn1.AddComponent<TestComponent>();

            Pawn pawn2 = new Pawn("Second", board, pos2, footprint, true, true, true);
            TestComponent test2 = pawn2.AddComponent<TestComponent>();

            board.Pawns.Add(pawn1);
            board.Pawns.Add(pawn2);

            bool p1Triggered = false;
            bool p2Triggered = false;

            test1.CollisionEntered += delegate (Pawn source, Pawn other) {
                p1Triggered = true;
                Assert.AreSame(source, pawn1);
                Assert.AreSame(other, pawn2);
            };

            test2.CollisionEntered += delegate (Pawn source, Pawn other) {
                p2Triggered = true;
                Assert.AreSame(source, pawn2);
                Assert.AreSame(other, pawn1);
            };

            // Act
            pawn1.Position = pawn1.Position + Point.Right;

            // Assert
            Assert.IsTrue(p1Triggered, "Pawn 1's OnCollisionEnter was not triggered");
            Assert.IsTrue(p2Triggered, "Pawn 2's OnCollisionEnter was not triggered");
        }
    }
}