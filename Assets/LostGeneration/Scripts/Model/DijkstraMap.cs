using System;

namespace LostGen {

	/// <summary>
	/// 	A general purpose heatmap algorithm used to rate positions on the <see cref="Board"/> by some measure of
	/// 	desirability.
	/// </summary>
	/// <remarks>
	/// 	Based on the work of <see href="https://sites.google.com/site/broguegame/home">Brian Walker, of Brogue</see>
	/// 	fame.
	/// </remarks>
	/// <seealso href="https://www.roguebasin.com/index.php?title=The_Incredible_Power_of_Djikstra_Maps">
	/// 	<i>The Incredible Power of Djikstra Maps</i> by Brian Walker
	/// </seealso>
	/// <seealso href="https://www.roguebasin.com/index.php?title=Djikstra_Maps_Visualized">
	/// 	Djikstra Maps Visualized, by Derrick Creamer
	/// </seealso>
	public class DijkstraMap {
		public Point Size {
			get { return new Point(_values.GetLength(0), _values.GetLength(1), _values.GetLength(2)); }
		}
		private int[,,] _values;
		private Predicate<Point> _filter;

		/// <summary>
		/// 	Create a new DjikstraMap for the given <see cref="Board"/>.
		/// </summary>
		/// <param name="size">The size of this DjikstraMap</param>
		/// <param name="filter">
		/// 	A filter function. Returns true when given a <see cref="Point"/> on the Board that
		/// 	should be ignored by the DjikstraMap evaluation.
		/// </param>
		public DijkstraMap(Point size, Predicate<Point> filter = null) {
			_values = new int[size.X, size.Y, size.Z];			
			_filter = filter ?? DefaultFilter;
			Clear();
		}

		/// <summary>
		/// Sets all cells in the DjikstraMap to the highest integer value
		/// </summary>
		public void Clear() {
			// Initialize values cell to max
			for (int x = 0; x < _values.GetLength(0); x++) {
				for (int y = 0; y < _values.GetLength(1); y++) {
					for (int z = 0; z < _values.GetLength(2); z++) {
						_values[x, y, z] = int.MaxValue;
					}
				}
			}
		}

		/// <summary>
		/// Sets a cell value to some explicit value
		/// </summary>
		/// <param name="point">Cell to set</param>
		/// <param name="value">Value to set cell to</param>
		public void SetValue(Point point, int value) {
			if (InBounds(point)) {
				if (_filter(point)) {
					_values[point.X, point.Y, point.Z] = int.MaxValue;
				} else {
					_values[point.X, point.Y, point.Z] = value;
				}
			} else {
				throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds of the map");
			}
		}

		/// <summary>
		/// Returns the value of the given cell
		/// </summary>
		/// <param name="point">Target cell</param>
		/// <returns>Value of the cell at the given <see cref="Point"/></returns>
		public int GetValue(Point point) {
			int value;
			if (InBounds(point)) {
				if (!_filter(point)) {
					value = _values[point.X, point.Y, point.Z];
				} else {
					value = int.MaxValue;
				}
			} else {
				throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds of the map");
			}

			return value;
		}

		/// <summary>Creates the full DjikstraMap.</summary>
		/// <remarks>
		///		
		/// </remarks>
		/// <param name="maxPasses">Maximum number of times the algorithm will re-evaluate the entire map</param>
		public void Build(int maxPasses = -1) {
			bool changed = true;
			int passes = 0;
			
			while (changed || (maxPasses > 0 && passes < maxPasses)) {
				changed = false;
				for (int x = 0; x < _values.GetLength(0); x++) {
					for (int y = 0; y < _values.GetLength(1); y++) {
						for (int z = 0; z < _values.GetLength(2); z++) {
							Point point = new Point(x, y, z);
							int value = _values[x, y, z];

							if (!_filter(point)) {
								// Find the neighbor with the smallest value
								int min = int.MaxValue;
								for (int i = 0; i < Point.Neighbors.Length; i++) {
									Point neighbor = point + Point.Neighbors[i];
									if (InBounds(neighbor)) {
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
		
		public bool InBounds(Point point) {
			return point.X >= 0 && point.X < _values.GetLength(0) &&
				   point.Y >= 0 && point.Y < _values.GetLength(1) &&
				   point.Z >= 0 && point.Z < _values.GetLength(2);
		}

		private bool DefaultFilter(Point point) {
			return true;
		}

		#region OperatorOverloads

		public static DijkstraMap operator +(DijkstraMap map, int amount) {
			DijkstraMap sum = new DijkstraMap(map.Size, map._filter);
			Array.Copy(map._values, sum._values, map._values.Length);

			for (int x = 0; x < sum._values.GetLength(0); x++) {
				for (int y = 0; y < sum._values.GetLength(1); y++) {
					for (int z = 0; z < sum._values.GetLength(2); z++) {
						Point point = new Point(x, y, z);
						if (map._filter(point)) {
							sum._values[x, y, z] = int.MaxValue;
						} else {
							sum._values[x, y, z] += amount;
						}
					}
				}
			}

			return sum;
		}

		public static DijkstraMap operator *(DijkstraMap map, float multiplier) {
			DijkstraMap product = new DijkstraMap(map.Size, map._filter);
			Array.Copy(map._values, product._values, map._values.Length);

			for (int x = 0; x < product._values.GetLength(0); x++) {
				for (int y = 0; y < product._values.GetLength(1); y++) {
					for (int z = 0; z < product._values.GetLength(2); z++) {
						Point point = new Point(x, y, z);
						if (map._filter(point)) {
							product._values[x, y, z] = int.MaxValue;
						} else {
							int value = product._values[x, y, z];
							product._values[x, y, z] = (int)(multiplier * value + 0.5f);
						}
					}
				}
			}

			return product;
		}

		#endregion OperatorOverloads
	}
}