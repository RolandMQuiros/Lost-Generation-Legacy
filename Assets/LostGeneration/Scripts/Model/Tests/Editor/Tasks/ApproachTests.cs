using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using LostGen;

namespace Tests.Tasks {

    public class ApproachTests {
        [Test]
        public void PathfindToPoint() {
            string[] map = new string[] {
                    "###################\n" +
                    "###################\n" +
                    "###################\n" +
                    "###################\n" +
                    "###################\n" +
                    "###################\n" +
                    "###################\n" +
                    "###################\n",

                    "###################\n" +
                    "#...###..#####....#\n" +
                    "#...#.#..#..##.##.#\n" +
                    "#.###...##........#\n" +
                    "#.....#.######.##.#\n" +
                    "#.#####....###.##.#\n" +
                    "#.#............##.#\n" +
                    "###################"
            };
            Board board = BoardCommon.StringsToBoard(map);

            Point start = new Point(3, 1, 1);
            Point end = new Point(17, 1, 6);
            
            Pawn pawn = new Pawn("Approacher", board, start);
            pawn.AddComponent(new ActionPoints(5));
            pawn.AddComponent(new LongWalkSkill());
            board.Pawns.Add(pawn);

            DijkstraMap approachMap = new DijkstraMap(board.Blocks.Size, p => board.IsSolid(p));
            approachMap.SetValue(end, 0);
            approachMap.Build();

            DijkstraMapTests.PrintMap(approachMap, p => board.IsSolid(p));

            ApproachTask task = new ApproachTask(pawn, approachMap);
            IEnumerator doing = task.Do();

            Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
            while (doing.MoveNext()) {
                if (doing.Current == null) {
                    Assert.Fail("Approach plan failed");
                }
                board.BeginTurn();
                board.Turn(messages);
            }
            board.BeginTurn();
            board.Turn(messages);

            Assert.NotZero(messages.Count);
            Assert.AreEqual(end, pawn.Position);
        }
    }

}