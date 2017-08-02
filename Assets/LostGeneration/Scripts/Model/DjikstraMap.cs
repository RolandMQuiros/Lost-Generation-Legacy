using System;

namespace LostGen {

	public class DjikstraMap {
		public Board Board { get; private set; }
		private int[,,] _values;
		private Predicate<Point> _filter;

		public DjikstraMap(Board board, Predicate<Point> filter = null) {
			Board = board;
			_values = new int[board.Blocks.Size.X, board.Blocks.Size.Y, board.Blocks.Size.Z];			
			_filter = filter ?? DefaultFilter;
			
			Clear();
		}

		public void Clear() {
			// Initialize values cell to max
			for (int x = 0; x < Board.Blocks.Size.X; x++) {
				for (int y = 0; y < Board.Blocks.Size.Y; y++) {
					for (int z = 0; z < Board.Blocks.Size.Z; z++) {
						_values[x, y, z] = Int32.MaxValue;
					}
				}
			}
		}

		public void SetValue(Point point, int value) {
			if (Board.Blocks.InBounds(point)) {
				_values[point.X, point.Y, point.Z] = value;
			} else {
				throw new ArgumentOutOfRangeException("blockPoint", "Given point is outside the board");
			}
		}

		public int GetValue(Point point) {
			int value;
			if (Board.Blocks.InBounds(point)) {
				value = _values[point.X, point.Y, point.Z];
			} else {
				throw new ArgumentOutOfRangeException("blockPoint", "Given point is outside the board");
			}

			return value;
		}

		public void Build(int maxPasses = -1) {
			bool changed = true;
			int passes = 0;
			
			while (changed || (maxPasses > 0 && passes < maxPasses)) {
				changed = false;
				for (int x = 0; x < Board.Blocks.Size.X; x++) {
					for (int y = 0; y < Board.Blocks.Size.Y; y++) {
						for (int z = 0; z < Board.Blocks.Size.Z; z++) {
							Point point = new Point(x, y, z);
							int value = _values[x, y, z];

							if (!_filter(point)) {
								// Find the neighbor with the smallest value
								int min = Int32.MaxValue;
								for (int i = 0; i < Point.Neighbors.Length; i++) {
									Point neighbor = point + Point.Neighbors[i];
									if (Board.Blocks.InBounds(neighbor)) {
										int neighborVal = _values[neighbor.X, neighbor.Y, neighbor.Z];
										if (neighborVal < min) {
											min = neighborVal;
										}
									}
								}

								// If the current block's value is at least 2 greater than the minimum, set it to 1 more than that minimum
								if (value - min > 2) {
									_values[x, y, z] = min + 1;
									changed = true;
								}
							}
						}
					}
				}
				passes++;
			}
		}
		
		private bool DefaultFilter(Point point) {
			bool skip = false;

			// Skip only if the current block is solid, or the block below it is not solid.
			// This limits the scan to blocks that have ground to stand on.
			if (Board.Blocks.InBounds(point)) {
				BoardBlock block = Board.Blocks.At(point);
				skip = block.IsSolid;
				if (!skip && Board.Blocks.InBounds(point + Point.Down)) {
					BoardBlock below = Board.Blocks.At(point + Point.Down);
					skip = !below.IsSolid;
				}
			}

			return skip;
		}

		#region OperatorOverloads

		public static DjikstraMap operator +(DjikstraMap map, int amount) {
			DjikstraMap sum = new DjikstraMap(map.Board, map._filter);
			Array.Copy(map._values, sum._values, map._values.Length);

			for (int x = 0; x < sum.Board.Blocks.Size.X; x++) {
				for (int y = 0; y < sum.Board.Blocks.Size.Y; y++) {
					for (int z = 0; z < sum.Board.Blocks.Size.Z; z++) {
						sum._values[x, y, z] += amount;
					}
				}
			}

			return sum;
		}

		public static DjikstraMap operator *(DjikstraMap map, float multiplier) {
			DjikstraMap sum = new DjikstraMap(map.Board, map._filter);
			Array.Copy(map._values, sum._values, map._values.Length);

			for (int x = 0; x < sum.Board.Blocks.Size.X; x++) {
				for (int y = 0; y < sum.Board.Blocks.Size.Y; y++) {
					for (int z = 0; z < sum.Board.Blocks.Size.Z; z++) {
						int value = sum._values[x, y, z];
						sum._values[x, y, z] = (int)(multiplier * value + 0.5f);
					}
				}
			}

			return sum;
		}

		#endregion OperatorOverloads
	}
}