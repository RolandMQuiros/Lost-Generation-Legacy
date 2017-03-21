// using System;
// using System.Linq;
// using System.Collections.Generic;
// using NUnit.Framework;
// using LostGen;

// namespace Tests.Skills {
//     [TestFixture]
//     public class Walk {
//         private void ArrangeBoard(int[,,] grid, Point start, Point end, out Board board, out Pawn pawn) {
//             board = BoardCommon.ArrayToBoard(grid);
//             pawn = new Pawn("Walker", board, start);
//             Combatant combatant = pawn.AddComponent<Combatant>();

//             Stats stats = new Stats() { Stamina = 100 };
//             combatant.BaseStats = stats;

//             board.AddPawn(pawn);

//             WalkSkill walk = new WalkSkill(combatant);
//             combatant.AddSkill(walk);

//             walk.SetTarget(end);

//             board.BeginTurn();
//         }

//         [Test]
//         public void WalkNodeNeighborsOnLevel() {
//             Board board = BoardCommon.ArrayToBoard(new int[,,] {
//                 { // Solid floor
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 },
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 1, 1, 1, 1, 1, 0},
//                     {0, 1, 0, 1, 1, 1, 0},
//                     {0, 1, 0, 1, 1, 1, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 }
//             });
            

//             Point origin = new Point(3, 1, 2);
//             WalkNode walkNode = new WalkNode(board, origin, true);

//             HashSet<Point> actualNeighbors = new HashSet<Point>(
//                 walkNode.GetNeighbors().Select(node => (node as BlockNode).Point)
//             );

//             Point[] expectedNeighbors = {
//                 origin + Point.Forward + Point.Right,
//                 origin + Point.Forward,
//                 origin + Point.Right,
//                 origin + Point.Backward + Point.Right,
//                 origin + Point.Backward,
//                 origin + Point.Backward + Point.Left
//             };

//             Console.WriteLine(
//                 "Expected Neighbors: " +
//                 String.Join("; ", expectedNeighbors.Select(pt => pt.ToString()).ToArray())
//             );
//             Console.WriteLine(
//                 "Actual neighbors: " +
//                 String.Join("; ", actualNeighbors.Select(pt => pt.ToString()).ToArray())
//             );
//             Console.WriteLine(BoardCommon.PrintBoard(board, actualNeighbors));
        
//             Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Count);
//             for (int i = 0; i < expectedNeighbors.Length; i++) {
//                 Assert.IsTrue(actualNeighbors.Contains(expectedNeighbors[i]), "Neighbors list did not contain " + expectedNeighbors[i]);
//             }
//         }

//         [Test]
//         public void WalkNodeNeighborsLedges() {
//             Board board = BoardCommon.ArrayToBoard(new int[,,] {
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 },
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 1, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 },
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 1, 1, 1, 1, 1, 0},
//                     {0, 1, 0, 1, 0, 1, 0},
//                     {0, 1, 0, 1, 1, 1, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 },
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 1, 1, 1, 1, 1, 0},
//                     {0, 1, 1, 1, 0, 1, 0},
//                     {0, 1, 0, 1, 1, 1, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 },
//                 {
//                     {0, 0, 0, 0, 0, 0, 0},
//                     {0, 1, 1, 1, 1, 1, 0},
//                     {0, 1, 1, 1, 0, 1, 0},
//                     {0, 1, 1, 1, 1, 1, 0},
//                     {0, 0, 0, 0, 0, 0, 0},
//                 }
//             });

//             Point origin = new Point(3, 2, 2);
//             WalkNode walkNode = new WalkNode(board, origin, true);

//             HashSet<Point> actualNeighbors = new HashSet<Point>(
//                 walkNode.GetNeighbors().Select(node => (node as BlockNode).Point)
//             );

//             Point[] expectedNeighbors = {
//                 origin + Point.Forward + Point.Right,
//                 origin + Point.Forward,
//                 origin + Point.Forward + Point.Left + (2 * Point.Up),
//                 origin + Point.Left + Point.Up,
//                 origin + Point.Backward + Point.Right,
//                 origin + Point.Backward + Point.Down,
//                 origin + Point.Backward + Point.Left
//             };

//             Console.WriteLine(
//                 "Expected Neighbors: " +
//                 String.Join("; ", expectedNeighbors.Select(pt => pt.ToString()).ToArray())
//             );
//             Console.WriteLine(
//                 "Actual neighbors: " +
//                 String.Join("; ", actualNeighbors.Select(pt => pt.ToString()).ToArray())
//             );

//             Console.WriteLine(BoardCommon.PrintBoard(board, actualNeighbors));

//             Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Count, "Number of actual neighbors did not match expected neighbors");
//             for (int i = 0; i < expectedNeighbors.Length; i++) {
//                 Assert.IsTrue(actualNeighbors.Contains(expectedNeighbors[i]), "Neighbors list did not contain " + expectedNeighbors[i]);
//             }
//         }

//         [Test]
//         public void CornerToCorner() {
//             Board board;
//             Pawn pawn;
//             Point start = new Point(1, 1, 1);
//             Point end = new Point(10, 1, 6);

//             int[,,] grid = new int[2, 8, 12];
//             Array.Copy(BoardCommon.GRID_12X1X9, 0, grid, 12*8, 12 * 8);
//             ArrangeBoard(grid, start, end, out board, out pawn);

//             Combatant combatant = pawn.GetComponent<Combatant>();

//             WalkSkill walk = combatant.GetSkill<WalkSkill>();
//             walk.CanWalkDiagonally = false;

//             Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

//             Console.Write(BoardCommon.PrintBoard(board, walk.GetPath(), new List<Point>() {start, end}));

//             walk.Fire();

//             Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
//             board.Turn(messages);

//             Assert.AreEqual(end, pawn.Position);
//         }

//         [Test]
//         public void CenterWall() {
//             int[,,] space = new int[,,] {{
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
//             }};
//             int[,,] grid = new int[space.GetLength(0) + 1, space.GetLength(1), space.GetLength(2)];
//             Array.Copy(space, 0, grid, space.Length, space.GetLength(1) * space.GetLength(2));

//             Board board;
//             Pawn pawn;
//             Point start = new Point(1, 1, 1);
//             Point end = new Point(10, 1, 6);
//             ArrangeBoard(grid, start, end, out board, out pawn);
//             Combatant combatant = pawn.GetComponent<Combatant>();

//             WalkSkill walk = combatant.GetSkill<WalkSkill>();

//             Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);

//             Console.Write(BoardCommon.PrintBoard(board, walk.GetPath(), new List<Point>() {start, end}));

//             walk.Fire();
//             Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
//             board.Turn(messages);

//             Assert.AreEqual(end, pawn.Position);
//         }

//         [Test]
//         public void Maze() {
//             int[,,] space = new int[,,] {{
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
//                 { 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0 },
//                 { 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0 },
//                 { 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0 },
//                 { 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 },
//                 { 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0 },
//                 { 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0 },
//                 { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0 },
//                 { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
//             }};
//             int[,,] grid = new int[space.GetLength(0) + 1, space.GetLength(1), space.GetLength(2)];
//             Array.Copy(space, 0, grid, space.Length, space.GetLength(1) * space.GetLength(2));

//             Board board;
//             Pawn pawn;
//             Point start = new Point(1, 1, 1);
//             Point end = new Point(10, 1, 6);
//             ArrangeBoard(grid, start, end, out board, out pawn);
//             Combatant combatant = pawn.GetComponent<Combatant>();

//             WalkSkill walk = combatant.GetSkill<WalkSkill>();
//             walk.CanWalkDiagonally = false;
//             Console.Write(BoardCommon.PrintBoard(board, walk.GetPath(), new List<Point>() {start, end}));
            
//             Assert.LessOrEqual(walk.ActionPoints, combatant.ActionPoints);
            
//             walk.Fire();
//             Queue<IPawnMessage> messages = new Queue<IPawnMessage>();
//             board.Turn(messages);
            
//             Assert.AreEqual(end, pawn.Position);
//         }
//     }

// }