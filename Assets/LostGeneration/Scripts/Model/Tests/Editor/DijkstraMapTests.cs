using System;
using System.Linq;
using NUnit.Framework;
using LostGen;

namespace Tests {

	public class DijkstraMapTests {
		private void PrintArray(int[,,] array, Predicate<Point> filter) {
			string display = string.Empty;
			for (int y = 0; y < array.GetLength(0); y++) {
				for (int z = 0; z < array.GetLength(1); z++) {
					for (int x = 0; x < array.GetLength(2); x++) {
						Point point = new Point(x, y, z);
						if (filter(point)) {
							display += '█';
						} else {
							display += Math.Abs(array[y, z, x]).ToString("X");
						}
					}
					display += '\n';
				}
				display += "\n\n";
			}
			Console.WriteLine(display);
		}

		private void PrintMap(DijkstraMap map, Predicate<Point> filter) {
			string display = string.Empty;
			for (int y = 0; y < map.Size.Y; y++) {
				for (int z = 0; z < map.Size.Z; z++) {
					for (int x = 0; x < map.Size.X; x++) {
						Point point = new Point(x, y, z);
						if (filter(point)) {
							display += '█';
						} else {
							display += Math.Abs(map.GetValue(point)).ToString("X");
						}
					}
					display += '\n';
				}
				display += "\n\n";
			}
			Console.WriteLine(display);
		}

		[Test]
		public void ConstructTest() {
			int[,,] board = new int[,,] {
				{		
					{  0,  0,  0,  0,  0,  1,  1,  0 },
					{  0,  1,  0,  0,  0,  1,  1,  1 },
					{  0,  1,  1,  1,  1,  1,  1,  0 },
					{  1,  1,  1,  0,  1,  1,  1,  1 },
					{  1,  1,  0,  1,  1,  1,  1,  1 },
					{  0,  0,  0,  0,  0,  0,  1,  0 }
				}
			};
			Point size = new Point(board.GetLength(2), 1, board.GetLength(1));
			Predicate<Point> filter = (point) => {
				return board[point.Y, point.Z, point.X] == 0;
			};

			DijkstraMap map = new DijkstraMap(size, filter);
			map.SetValue(new Point(1, 0, 3), 0);
			map.Build();

			const int S = int.MaxValue; // S for skip
			int[,,] expected = {
				{	
					{  S,  S,  S,  S,  S,  7,  8,  S },
					{  S,  2,  S,  S,  S,  6,  7,  8 },
					{  S,  1,  2,  3,  4,  5,  6,  S },
					{  1,  0,  1,  S,  5,  6,  7,  8 },
					{  2,  1,  S,  7,  6,  7,  8,  9 },
					{  S,  S,  S,  S,  S,  S,  9,  S }
				}
			};

			Console.WriteLine("Expected:");
			PrintArray(expected, filter);

			Console.WriteLine("Actual:");
			PrintMap(map, filter);

			Console.WriteLine("Difference:");
			int difference = 0;
			for (int z = 0; z < map.Size.Z; z++) {
				for (int x = 0; x < map.Size.X; x++) {
					Point point = new Point(x, 0, z);
					if (filter(point)) {
						Console.Write('█');
					} else if (expected[0, z, x] == map.GetValue(point)) {
						Console.Write(expected[0, z, x]);
					} else {
						Console.Write('░');
						difference++;
					}
				}
				Console.WriteLine();
			}

			Assert.Zero(difference);
		}

        [Test]
        public void Retreat() {
            int[,,] board = new int[,,] {
                {
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 1, 1, 0, 0, 1, 0, 1, 0, 0 },
                    { 0, 1, 1, 0, 0, 1, 1, 1, 0, 0 },
                    { 0, 1, 1, 1, 1, 1, 0, 1, 0, 0 },
                    { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                }
            };
            Point size = new Point(board.GetLength(2), board.GetLength(0), board.GetLength(1));
			Predicate<Point> filter = p => board[p.Y, p.Z, p.X] == 0;
            DijkstraMap map = new DijkstraMap(size, filter);

            // Set the top left corner as the cell we're running from
			Point avoid = new Point(1, 0, 1);
            map.SetValue(avoid, 0);

            // Build the map
            map.Build();
			PrintMap(map, filter);

            // Invert the map by 1.2
            map *= -1.2f;

            // Rescan
            map.Build();
			PrintMap(map, filter);

            // Simulating an escaping Pawn, move a marker downhill until it gets stuck at a point.
            Point current = new Point();
            Point next = new Point(1, 0, 2);
			int loops = 0;
            do {
				Console.WriteLine("Moved from " + current + " to " + next);
                current = next;
                int min = map.GetValue(current);
                foreach (Point offset in Point.Neighbors) {
                    Point neighbor = current + offset;
                    if (map.InBounds(neighbor)) {
                        int mapValue = map.GetValue(neighbor);
                        if (mapValue < min) {
                            min = mapValue;
                            next = neighbor;
                        }
                    }
                }
				loops++;
            } while (current != next && loops < 100);

			if (loops == 100) {
				Assert.Fail("Movement test did not converge at a point");
			}

			// Its eventual hiding space ought to be further from the original, in the very least
			Assert.Less(map.GetValue(current), map.GetValue(avoid));
        }
	}
}