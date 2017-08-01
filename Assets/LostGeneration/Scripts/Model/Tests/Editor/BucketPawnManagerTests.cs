using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests {
    public class BucketPawnManagerTests {
        [Test]
        public void ConstructWithBoardSize() {
            Point boardSize = new Point(32, 32, 32);
            Point bucketSize = new Point(8, 8, 8);
            BucketPawnManager pawnManager = new BucketPawnManager(boardSize, bucketSize);
            Assert.AreEqual(pawnManager.BucketSize, bucketSize);
        }

        [Test]
        public void ConstructWithSubdivisions() {
            Point boardSize = new Point(32, 32, 32);
            BucketPawnManager pawnManager = new BucketPawnManager(boardSize, 4);
            Assert.AreEqual(Point.One * 8, pawnManager.BucketSize);
        }

        [Test]
        public void AddPawn() {
            Point boardSize = new Point(32, 32, 32);
            BucketPawnManager pawnManager = new BucketPawnManager(boardSize, 1);
            Board board = new Board(boardSize);

            Pawn pawn = new Pawn("Test Pawn", board, boardSize / 2);
            
            Assert.IsTrue(pawnManager.Add(pawn), "BucketPawnManager failed to add a test pawn");
            Assert.IsTrue(pawnManager.Contains(pawn), "BucketPawnManager couldn't find a pawn it just added to itself");
            Assert.IsTrue(pawnManager.At(boardSize / 2).Contains(pawn), "BucketPawnManager couldn't find a pawn it just added to itself, at a given location");
        }

        [Test]
        public void AddPawnSplit() {
            Point boardSize = new Point(32, 16, 16);
            Point bucketSize = new Point(16, 16, 16);
            BucketPawnManager pawnManager = new BucketPawnManager(boardSize, bucketSize);
            Assert.AreEqual(bucketSize, pawnManager.BucketSize);
            
            // Create a pawn with a footprint that steps in both buckets
            Board board = new Board(boardSize);
            Point position = boardSize / 2;
            Pawn pawn = new Pawn("Test Pawn", board, position, new Point[] { Point.Left, Point.Zero, Point.Right });
            
            Assert.IsTrue(pawnManager.Add(pawn), "BucketPawnManager failed to add a test pawn");
            Assert.IsTrue(pawnManager.Contains(pawn), "BucketPawnManager couldn't find a pawn it just added to itself");
            Assert.IsTrue(pawnManager.At(position).Contains(pawn), "BucketPawnManager couldn't find a pawn it just added to itself, at a given location");
            Assert.IsTrue(pawnManager.At(position + Point.Left).Contains(pawn));
            Assert.IsTrue(pawnManager.At(position + Point.Right).Contains(pawn));

            Assert.IsTrue(pawnManager.Remove(pawn));
        }
    }
}