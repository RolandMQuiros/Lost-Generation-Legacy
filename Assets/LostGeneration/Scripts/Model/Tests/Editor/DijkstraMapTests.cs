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
							display += array[y, z, x].ToString("X");
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
							display += map.GetValue(point).ToString("X");
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
			map.SetValue(new Point(1, 0, 2), 0);
			map.Build();

			const int S = int.MaxValue; // S for skip
			int[,,] expected = {
				{	
					{  S,  S,  S,  S,  S,  7,  8,  S },
					{  S,  2,  S,  S,  S,  6,  7,  9 },
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
	}
}