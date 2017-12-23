
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using LostGen.Model;

namespace Tests {
	public class WalkNodeTests {
		[Test]
		public void HashCodes() {
			Board board = BoardCommon.ArrayToBoard(new int[,,] {
				{
					{0, 0, 0},
					{0, 0, 0},
					{0, 0, 0}
				},
				{
					{1, 1, 1},
					{1, 1, 1},
					{1, 1, 1}
				}
			});
			
			WalkNode center = new WalkNode(board, Point.One, true, true);
			HashSet<WalkNode> neighbors = new HashSet<WalkNode>(
				center.GetNeighbors().Cast<WalkNode>()
			);

			Assert.AreEqual(8, neighbors.Count);

			WalkNode neighbor = neighbors.First();
			foreach (WalkNode neighborsNeighbor in neighbor.GetNeighbors()) {
				neighbors.Add(neighborsNeighbor);
			} 

			Assert.AreEqual(9, neighbors.Count);
		}


		[Test]
		public void OverlappingNeighbors() {
			Board board = BoardCommon.ArrayToBoard(new int[,,] {
				{
					{0, 0, 0},
					{0, 0, 0},
					{0, 0, 0}
				},
				{
					{1, 1, 1},
					{1, 1, 1},
					{1, 1, 1}
				}
			});

			WalkNode center = new WalkNode(board, new Point(1, 1, 1), true, false);
			HashSet<Point> points = new HashSet<Point>(
				GraphMethods.FloodFill<BlockNode>(center).Select(n => n.Point)
			);

			Assert.AreEqual(8, points.Count);
			
			foreach (Point neighbor in Point.NeighborsXZ) {
				Assert.IsTrue(points.Contains(center.Point + neighbor));
			}
		}
	}
}
