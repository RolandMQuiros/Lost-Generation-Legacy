using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

namespace Tests {
    public class BoardCommon {
        public static readonly int[,] GRID_12X8 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static string PrintGrid(int[,] grid, IEnumerable<Point> path = null, IEnumerable<Point> pawns = null) {
            string str = string.Empty;
            HashSet<Point> pathPoints = new HashSet<Point>(path);

            for (int j = 0; j < grid.GetLength(0); j++) {
                for (int i = 0; i < grid.GetLength(1); i++) {
                    Point point = new Point(i, j);
                    if (pawns != null && pawns.Contains(point)) {
                        str += "╬";
                    } else if (path != null && pathPoints.Contains(point)) {
                        str += "┼";
                    } else {
                        switch (grid[j, i]) {
                            case 0:
                                str += "█";
                                break;
                            case 1:
                                str += "░";
                                break;
                        }
                    }
                }
                str += "\n";
            }

            return str;
        }
    }
}
