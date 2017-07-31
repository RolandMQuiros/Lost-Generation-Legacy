using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.PawnComponents {
    [TestFixture]
    public class GravityTests {
        private static readonly int[,,] _chuteGrid = new int[,,] {
            {
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 }
            },
            {
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 1, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 }
            },
            {
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 1, 0, 0 },
                {0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0 }
            },
            {
                {0, 0, 0, 0, 0 },
                {0, 1, 1, 1, 0 },
                {0, 1, 1, 1, 0 },
                {0, 1, 1, 1, 0 },
                {0, 0, 0, 0, 0 }
            },
        };

        [Test]
        public void MoveOffCliff() {
            Board board = BoardCommon.ArrayToBoard(_chuteGrid);
            Pawn faller = new Pawn("Faller", board, new Point(1, 3, 2), null, true, true, true);
            faller.AddComponent<Gravity>();
            board.AddPawn(faller);
            
            faller.PushAction(new MoveAction(faller, faller.Position, faller.Position + Point.Right, 0, true));

            Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
            board.Step(messages);

            Console.Write(string.Join("\n", messages.Select(m => m.Text).ToArray()));
            Console.WriteLine(BoardCommon.PrintBoard(board, new Point[] { faller.Position }));

            Assert.AreEqual(new Point(2, 1, 2), faller.Position);
        }

        [Test]
        public void SquashOtherPawn() {
            Board board = BoardCommon.ArrayToBoard(_chuteGrid);
            Pawn faller = new Pawn("Faller", board, new Point(1, 3, 2), null, true, true, true);
            faller.AddComponent<Gravity>();

            Pawn squashed = new Pawn("Squashed", board, new Point(2, 1, 2), null, true, true, true);
            squashed.AddComponent<Gravity>();

            board.AddPawn(faller);
            board.AddPawn(squashed);
            
            faller.PushAction(new MoveAction(faller, faller.Position, faller.Position + Point.Right, 0, true));

            Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
            board.Step(messages);

            Console.Write(string.Join("\n", messages.Select(m => m.Text).ToArray()));
            Console.WriteLine(BoardCommon.PrintBoard(board, new Point[] { faller.Position }, new Point[] { squashed.Position }));

            Assert.AreEqual(new Point(2, 2, 2), faller.Position);
        }
    }
}