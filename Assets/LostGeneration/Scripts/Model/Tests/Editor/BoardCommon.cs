using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

namespace Tests {
    public class BoardCommon {
        public static readonly int[,,] GRID_12X1X9 = new int[,,] {
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };

        public static string BoardString(Board board, IEnumerable<Point> path = null, IEnumerable<Point> pawns = null) {
            string str = string.Empty;
            HashSet<Point> pathPoints;
            if (path == null) { pathPoints = new HashSet<Point>(); }
            else { pathPoints = new HashSet<Point>(path); }
            
            for (int y = 0; y < board.Blocks.Size.Y; y++) {
                str += "y = " + y + "\n";
                for (int z = 0; z < board.Blocks.Size.Z; z++) {
                    for (int x = 0; x < board.Blocks.Size.X; x++) {
                        Point point = new Point(x, y, z);
                        if (pawns != null && pawns.Contains(point)) {
                            str += "╬";
                        } else if (path != null && pathPoints.Contains(point)) {
                            str += "┼";
                        } else {
                            BoardBlock block = board.Blocks.At(point);
                            if (block.IsSolid) { str += "█"; }
                            else { str += "░"; }
                        }
                    }
                    str += "\n";
                }
                str += "\n";
            }

            return str;
        }

        public static Board StandardBoard(Point size) {
            StandardBlockManager blocks = new StandardBlockManager(size);
            BucketPawnManager pawns = new BucketPawnManager(size, 1);
            Board board = new Board(blocks, pawns);
            return board;
        }

        public static Board ArrayToBoard(int[,,] array) {
            Board board = StandardBoard(new Point(array.GetLength(2), array.GetLength(0), array.GetLength(1)));
            for (int z = 0; z < array.GetLength(1); z++) {
                for (int y = 0; y < array.GetLength(0); y++) {
                    for (int x = 0; x < array.GetLength(2); x++) {
                        BoardBlock block = new BoardBlock() {
                            Point = new Point(x, y, z),
                            IsSolid = array[y, z, x] == 0,
                            IsOpaque = array[y, z, x] == 0
                        };
                        board.Blocks.Set(block);
                    }
                }
            }

            return board;
        }

        public static Board StringsToBoard(string[] map) {
            List<BoardBlock> blocksToAdd = new List<BoardBlock>();
            Point point = new Point();
            Point size = new Point(0, map.Length, 0);
            for (int y = 0; y < map.Length; y++) {
                point.X = 0;
                point.Y = y;
                point.Z = 0;

                for (int i = 0; i < map[y].Length; i++) {
                    switch (map[y][i]) {
                        case '#':
                            blocksToAdd.Add(new BoardBlock() {
                                Point = point,
                                IsSolid = true,
                                IsOpaque = true
                            });
                            point.X++;
                            break;
                        case '\n':
                            point.X = 0;
                            point.Z++;
                            break;
                        default:
                            point.X++;
                            break;
                    }
                    if (point.X > size.X) { size.X = point.X; }
                    if (point.Z > size.Z) { size.Z = point.Z; }
                }
            }
            Board board = StandardBoard(size);
            for (int i = 0; i < blocksToAdd.Count; i++) {
                board.Blocks.Set(blocksToAdd[i]);
            }

            return board;
        }
    }
}
