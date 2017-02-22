using System;
using NUnit.Framework;
using LostGen;

namespace Tests {

	public class DjikstraMapTests {

		[Test]
		public void ConstructTest() {
			Board board = BoardCommon.ArrayToBoard( new int[,,] {
				{
					{ 0, 0, 0, 0 },
					{ 0, 0, 0, 0 },
					{ 0, 0, 0, 0 },
					{ 0, 0, 0, 0 }
				},
				{	
					{ 0, 0, 0, 1 },
					{ 1, 1, 0, 1 },
					{ 0, 1, 1, 1 },
					{ 0, 1, 0, 0 }
				}
			});

			DjikstraMap map = new DjikstraMap(board);
			map.SetValue(new Point(1, 1, 2), 0);
			map.Build(1);

			string display = string.Empty;
			for (int y = 0; y < board.Size.Y; y++) {
				for (int z = 0; z < board.Size.Z; z++) {
					for (int x = 0; x < board.Size.X; x++) {
						Point point = new Point(x, y, z);
						if (board.GetBlock(point).IsSolid) {
							display += '#';
						} else {
							display += map.GetValue(point);
						}
					}
					display += '\n';
				}
				display += "\n\n";
			}

			Console.WriteLine(display);
		}
	}
}