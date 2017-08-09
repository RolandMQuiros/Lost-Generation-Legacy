using System;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
	public class StandardBlockManager : IBlockManager {
		private BoardBlock[,,] _blocks;
		public Point Size {
			get {
				return new Point(_blocks.GetLength(0),
								 _blocks.GetLength(1),
								 _blocks.GetLength(2));
			}
		}
        public event Action<Dictionary<BoardBlock, BoardBlock>> Changed;

		public StandardBlockManager(Point size) {
			_blocks = new BoardBlock[size.X, size.Y, size.Z];
			for (int x = 0; x < size.X; x++) {
				for (int y = 0; y < size.Y; y++) {
					for (int z = 0; z < size.Z; z++) {
						_blocks[x, y, z].Point = new Point(x, y, z);
					}
				}
			}
		}

        public BoardBlock At(Point at) {
			if (!InBounds(at)) {
				throw new ArgumentOutOfRangeException("Given Point " + at + " is outside the bounds of the Board " + Size, "point");
			}

			return _blocks[at.X, at.Y, at.Z];
		}

		public IEnumerable<BoardBlock> At(IEnumerable<Point> at) {
			foreach (Point point in at) {
				yield return At(point);
			}
		}

        public void Set(BoardBlock block) {
			if (!InBounds(block.Point)) {
				throw new ArgumentOutOfRangeException("Given Point " + block.Point + " is outside the bounds of the Board " + Size, "point");
			}

			BoardBlock current = _blocks[block.Point.X, block.Point.Y, block.Point.Z];
			if (current != block) {
				_blocks[block.Point.X, block.Point.Y, block.Point.Z] = block;
				if (Changed != null) {
					Changed(new Dictionary<BoardBlock, BoardBlock>() {{ current, block }});
				}
			}
		}
        public void Set(IEnumerable<BoardBlock> blocks) {
			Dictionary<BoardBlock, BoardBlock> changes = null;
			foreach (BoardBlock block in blocks) {
				BoardBlock old = _blocks[block.Point.X, block.Point.Y, block.Point.Z];
				if (old != block) {
					_blocks[block.Point.X, block.Point.Y, block.Point.Z] = block;
					
					if (changes == null) {
						changes = new Dictionary<BoardBlock, BoardBlock>();
					} else if (changes.ContainsKey(old)) {
						throw new ArgumentException("The same block " + old + " was set multiple times", "blocks");
					} else {
						changes.Add(old, block);
					}
				}
			}

			if (changes != null && changes.Count > 0 && Changed != null) {
				Changed(changes);
			}
		}
        public bool InBounds(Point point) {
			return point.X >= 0 && point.X < _blocks.GetLength(0) &&
				   point.Y >= 0 && point.Y < _blocks.GetLength(1) &&
				   point.Z >= 0 && point.Z < _blocks.GetLength(2);
		}
	}
}