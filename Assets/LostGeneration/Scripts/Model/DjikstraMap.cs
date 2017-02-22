using System;

namespace LostGen {

	public class DjikstraMap {
		private Board _board;
		private byte[,,] _values;
		private Predicate<Point> _filter;

		public DjikstraMap(Board board, Predicate<Point> filter = null) {
			_board = board;
			_values = new byte[board.Size.X, board.Size.Y, board.Size.Z];			
			_filter = filter ?? DefaultFilter;
			
			Clear();
		}

		public void Clear() {
			// Initialize values cell to max
			for (int x = 0; x < _board.Size.X; x++) {
				for (int y = 0; y < _board.Size.Y; y++) {
					for (int z = 0; z < _board.Size.Z; z++) {
						_values[x, y, z] = Byte.MaxValue;
					}
				}
			}
		}

		public void SetValue(Point point, byte value) {
			if (_board.InBounds(point)) {
				_values[point.X, point.Y, point.Z] = value;
			} else {
				throw new ArgumentOutOfRangeException("blockPoint", "Given point is outside the board");
			}
		}

		public byte GetValue(Point point) {
			byte value;
			if (_board.InBounds(point)) {
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
				for (int x = 0; x < _board.Size.X; x++) {
					for (int y = 0; y < _board.Size.Y; y++) {
						for (int z = 0; z < _board.Size.Z; z++) {
							Point point = new Point(x, y, z);
							int value = _values[x, y, z];

							if (!_filter(point)) {
								// Find the neighbor with the smallest value
								byte min = Byte.MaxValue;
								for (int i = 0; i < Point.Neighbors.Length; i++) {
									Point neighbor = point + Point.Neighbors[i];
									if (_board.InBounds(neighbor)) {
										byte neighborVal = _values[neighbor.X, neighbor.Y, neighbor.Z];
										if (neighborVal < min) {
											min = neighborVal;
										}
									}
								}

								// If the current block's value is at least 2 greater than the minimum, set it to 1 more than that minimum
								if (value - min > 2) {
									_values[x, y, z] = (byte)(1 + min);
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
			if (_board.InBounds(point)) {
				BoardBlock block = _board.GetBlock(point);
				skip = block.IsSolid;
				if (!skip && _board.InBounds(point + Point.Down)) {
					BoardBlock below = _board.GetBlock(point + Point.Down);
					skip = !below.IsSolid;
				}
			}

			return skip;
		}

		#region OperatorOverloads

		public static DjikstraMap operator +(DjikstraMap map, byte amount) {
			DjikstraMap sum = new DjikstraMap(map._board, map._filter);
			Array.Copy(map._values, sum._values, map._values.Length);

			for (int x = 0; x < sum._board.Size.X; x++) {
				for (int y = 0; y < sum._board.Size.Y; y++) {
					for (int z = 0; z < sum._board.Size.Z; z++) {
						sum._values[x, y, z] += amount;
					}
				}
			}

			return sum;
		}

		public static DjikstraMap operator +(DjikstraMap map, int amount) {
			return map + (byte)amount;
		}

		public static DjikstraMap operator -(DjikstraMap map, byte amount) {
			return map + (-amount);
		}

		public static DjikstraMap operator -(DjikstraMap map, int amount) {
			return map + (byte)(-amount);
		}

		public static DjikstraMap operator *(DjikstraMap map, float multiplier) {
			DjikstraMap sum = new DjikstraMap(map._board, map._filter);
			Array.Copy(map._values, sum._values, map._values.Length);

			for (int x = 0; x < sum._board.Size.X; x++) {
				for (int y = 0; y < sum._board.Size.Y; y++) {
					for (int z = 0; z < sum._board.Size.Z; z++) {
						byte value = sum._values[x, y, z];
						sum._values[x, y, z] = (byte)(multiplier * value + 0.5f);
					}
				}
			}

			return sum;
		}

		#endregion OperatorOverloads
	}
}