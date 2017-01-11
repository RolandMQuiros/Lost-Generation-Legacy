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

        public static string PrintBoard(Board board, IEnumerable<Point> path = null, IEnumerable<Point> pawns = null) {
            string str = string.Empty;
            HashSet<Point> pathPoints = new HashSet<Point>(path);
            
            for (int y = 0; y < board.Size.Y; y++) {
                str += "y = " + y + "\n";
                for (int z = 0; z < board.Size.Z; z++) {
                    for (int x = 0; x < board.Size.X; x++) {
                        Point point = new Point(x, y, z);
                        if (pawns != null && pawns.Contains(point)) {
                            str += "╬";
                        } else if (path != null && pathPoints.Contains(point)) {
                            str += "┼";
                        } else {
                            BoardBlock block = board.GetBlock(point);
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

        public static Board ArrayToBoard(int[,,] array) {
            Board board = new Board(array.GetLength(2), array.GetLength(0), array.GetLength(1));
            for (int z = 0; z < array.GetLength(1); z++) {
                for (int y = 0; y < array.GetLength(0); y++) {
                    for (int x = 0; x < array.GetLength(2); x++) {
                        BoardBlock block = new BoardBlock();
                        block.IsSolid = array[y, z, x] == 0;
                        board.SetBlock(block, new Point(x, y, z));
                    }
                }
            }

            return board;
        }
    }
}
