using System;
using System.Collections.Generic;

namespace LostGen
{
    [Serializable]
    public struct Point : IEquatable<Point>
    {
        public static readonly Point Zero = new Point(0, 0, 0);
        public static readonly Point One = new Point(1, 1, 1);
        public static readonly Point Up = new Point(0, 1, 0);
        public static readonly Point Down = new Point(0, -1, 0);
        public static readonly Point Left = new Point(-1, 0, 0);
        public static readonly Point Right = new Point(1, 0, 0);
        public static readonly Point Forward = new Point(0, 0, 1);
        public static readonly Point Backward = new Point(0, 0, -1);
        public static readonly Point Max = new Point(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
        public static readonly Point Min = new Point(Int32.MinValue, Int32.MinValue, Int32.MinValue);
        public static readonly Point[] Neighbors = new Point[] {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left,
            Point.Forward,
            Point.Backward
        };

        public static readonly Point[] NeighborsFull = new Point[] {
            Point.Up + Point.Forward,
            Point.Up + Point.Forward + Point.Left,
            Point.Up + Point.Forward + Point.Right,
            Point.Up + Point.Left,
            Point.Up,
            Point.Up + Point.Right,
            Point.Up + Point.Backward + Point.Left,
            Point.Up + Point.Backward,
            Point.Up + Point.Backward + Point.Right,

            Point.Forward,
            Point.Forward + Point.Left,
            Point.Forward + Point.Right,
            Point.Left,
            Point.Right,
            Point.Backward + Point.Left,
            Point.Backward,
            Point.Backward + Point.Right,

            Point.Down + Point.Forward,
            Point.Down + Point.Forward + Point.Left,
            Point.Down + Point.Forward + Point.Right,
            Point.Down + Point.Left,
            Point.Down,
            Point.Down + Point.Right,
            Point.Down + Point.Backward + Point.Left,
            Point.Down + Point.Backward,
            Point.Down + Point.Backward + Point.Right,
        };

        public static readonly Point[] NeighborsFullXZ = new Point[] {
            Point.Forward + Point.Left,
            Point.Forward,
            Point.Forward + Point.Right,

            Point.Left,
            Point.Right,

            Point.Backward + Point.Left,
            Point.Backward,
            Point.Backward + Point.Right
        };

        public static readonly Point[] NeighborsXZ = new Point[] {
            Point.Forward,
            Point.Right,
            Point.Backward,
            Point.Left
        };

        public static readonly Point[] NeighborsFullXY = new Point[] {
            Point.Up,
            Point.Up + Point.Right,
            Point.Right,
            Point.Right + Point.Down,
            Point.Down,
            Point.Down + Point.Left,
            Point.Left +
            Point.Left + Point.Up
        };

        public static readonly Point[] NeighborsXY = new Point[] {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };

        public static readonly Point[] NeighborsFullZY = new Point[] {
            Point.Up,
            Point.Up + Point.Forward,
            Point.Forward,
            Point.Forward + Point.Down,
            Point.Down,
            Point.Down + Point.Backward,
            Point.Backward,
            Point.Backward + Point.Up
        };

        public static readonly Point[] NeighborsZY = new Point[] {
            Point.Up,
            Point.Forward,
            Point.Down,
            Point.Backward
        };

        public int X;
        public int Y;
        public int Z;

        public Point XY { get { return new Point(X, Y, 0); } }
        public Point XZ { get { return new Point(X, 0, Z); } }
        public Point YZ { get { return new Point(0, Y, Z); } }
        
        public Point(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Point other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + ",z: " + Z + "}";
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Point Abs(Point point)
        {
            return new Point
            (
                point.X < 0 ? -point.X : point.X,
                point.Y < 0 ? -point.Y : point.Y,
                point.Z < 0 ? -point.Z : point.Z
            );
        }

        public static double Distance(Point start, Point end)
        {
            Point offset = end - start;

            return Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z);
        }

        public static float FDistance(Point start, Point end)
        {
            return (float)Distance(start, end);
        }

        public static int TaxicabDistance(Point start, Point end)
        {
            Point offset = Point.Abs(end - start);
            return offset.X + offset.Y + offset.Z;
        }

        public static int ChebyshevDistance(Point start, Point end) {
            Point offset = Point.Abs(end - start);
            return Math.Max(Math.Max(offset.X, offset.Y), offset.Z);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static Point operator *(Point point, float scalar)
        {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f), (int)(point.Z * scalar + 0.5f));
        }

        public static Point operator *(float scalar, Point point)
        {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f), (int)(point.Z * scalar + 0.5f));
        }

        public static Point operator *(Point point, int scalar)
        {
            return new Point(point.X * scalar, point.Y * scalar, point.Z * scalar);
        }

        public static Point operator *(int scalar, Point point)
        {
            return new Point(point.X * scalar, point.Y * scalar, point.Z * scalar);
        }

        public static Point operator /(int scalar, Point point)
        {
            return new Point(point.X / scalar, point.Y / scalar, point.Z / scalar);
        }

        public static Point operator /(Point point, int scalar)
        {
            return new Point(point.X / scalar, point.Y / scalar, point.Z / scalar);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }

        public static IEnumerable<Point> Line(Point start, Point end) {
            Point delta = Point.Abs(end - start);
            Point step = new Point(
                delta.X > 0 ? 1 : -1,
                delta.Y > 0 ? 1 : -1,
                delta.Z > 0 ? 1 : -1
            );
            int maxDelta = Math.Max(delta.X, Math.Max(delta.Y, delta.Z));
            int halfMaxDelta = maxDelta >> 1;
            Point error = new Point(halfMaxDelta, halfMaxDelta, halfMaxDelta);

            Point point = start;
            for (int i = maxDelta; i > 0; i--) {
                yield return point;
                error -= delta;
                if (error.X < 0) {
                    error.X += maxDelta;
                    point.X += step.X;
                }
                if (error.Y < 0) {
                    error.Y += maxDelta;
                    point.Y += step.Y;
                }
                if (error.Z < 0) {
                    error.Z += maxDelta;
                    point.Z += step.Z;
                }
            }
        }

        public static CardinalDirection DirectionBetweenPoints(Point p1, Point p2)
        {
            Point difference = p2 - p1;

            CardinalDirection direction = CardinalDirection.South;
            int scalarY = Math.Abs(difference.Y);

            if (difference.Y >= 0)
            {
                if (difference.X > scalarY)
                {
                    direction = CardinalDirection.East;
                }
                else if (difference.X <= scalarY && difference.X >= -scalarY)
                {
                    direction = CardinalDirection.South;
                }
                else if (difference.X < -scalarY)
                {
                    direction = CardinalDirection.West;
                }
            }
            else
            {
                if (difference.X > scalarY)
                {
                    direction = CardinalDirection.East;
                }
                else if (difference.X <= scalarY && difference.X >= -scalarY)
                {
                    direction = CardinalDirection.North;
                }
                else if (difference.X < -scalarY)
                {
                    direction = CardinalDirection.West;
                }
            }

            return direction;
        }

        public static Point UpperBound(IEnumerable<Point> points)
        {
            Point max = Point.Min;
            foreach (Point point in points)
            {
                if (point.X > max.X) { max.X = point.X; }
                if (point.Y > max.Y) { max.Y = point.Y; }
                if (point.Z > max.Z) { max.Z = point.Z; }
            }
            return max;
        }

        public static Point LowerBound(IEnumerable<Point> points)
        {
            Point min = Point.Max;
            foreach (Point point in points)
            {
                if (point.X < min.X) { min.X = point.X; }
                if (point.Y < min.Y) { min.Y = point.Y; }
                if (point.Z < min.Z) { min.Z = point.Z; }
            }
            return min;
        }
    }
}
