using System;

namespace LostGen {
    public struct Point : IEquatable<Point> {
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point One = new Point(1, 1);
        public static readonly Point Up = new Point(0, -1);
        public static readonly Point Down = new Point(0, 1);
        public static readonly Point Left = new Point(-1, 0);
        public static readonly Point Right = new Point(1, 0);

        public static readonly Point[] Neighbors = new Point[] {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };

        public static readonly Point[] OctoNeighbors = new Point[] {
            Point.Up,
            Point.Up + Point.Right,
            Point.Right,
            Point.Right + Point.Down,
            Point.Down,
            Point.Down + Point.Left,
            Point.Left,
            Point.Left + Point.Up
        };

        public int X;
        public int Y;

        public Point(int x = 0, int y = 0) {
            X = x;
            Y = y;
        }

        public static double Distance(Point start, Point end) {
            Point offset = end - start;

            return Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y);
        }

        public static int TaxicabDistance(Point start, Point end) {
            Point offset = end - start;

            return Math.Abs(offset.X) + Math.Abs(offset.Y);
        }

        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2) {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point point, float scalar) {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f));
        }

        public static Point operator *(float scalar, Point point) {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f));
        }

        public static Point operator *(Point point, int scalar) {
            return new Point(point.X * scalar, point.Y * scalar);
        }

        public static Point operator *(int scalar, Point point) {
            return new Point(point.X * scalar, point.Y * scalar);
        }


        public override string ToString() {
            return "{x:" + X + ",y:" + Y + "}";
        }

        public bool Equals(Point other) {
            return X == other.X && Y == other.Y;
        }
    }
}
