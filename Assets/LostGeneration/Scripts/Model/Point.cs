using System;

namespace LostGen {
    [Serializable]
    public struct Point : IEquatable<Point> {
        public static readonly Point Zero = new Point(0, 0, 0);
        public static readonly Point One = new Point(1, 1, 1);
        public static readonly Point Up = new Point(0, 1, 0);
        public static readonly Point Down = new Point(0, -1, 0);
        public static readonly Point Left = new Point(-1, 0, 0);
        public static readonly Point Right = new Point(1, 0, 0);
        public static readonly Point Forward = new Point(0, 0, 1);
        public static readonly Point Backward = new Point(0, 0, -1);

        public static readonly Point[] Neighbors = new Point[] {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left,
            Point.Forward,
            Point.Backward
        };

        public static readonly Point[] FullNeighbors = new Point[] {
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

        public static readonly Point[] TransverseOctoNeighbors = new Point[] {
            Point.Forward + Point.Left,
            Point.Forward,
            Point.Forward + Point.Right,

            Point.Left,
            Point.Right,
            
            Point.Backward + Point.Left,
            Point.Backward,
            Point.Backward + Point.Right
        };

        public static readonly Point[] TransverseNeighbors = new Point[] {
            Point.Forward,
            Point.Right,
            Point.Backward,
            Point.Left
        };

        public int X;
        public int Y;
        public int Z;

        public Point(int x = 0, int y = 0, int z = 0) {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Point other) {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public override string ToString() {
            return "{x:" + X + ",y:" + Y + ",z: " + Z + "}";
        }

        public bool Equals(Point other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj) {
            return Equals((Point)obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static double Distance(Point start, Point end) {
            Point offset = end - start;

            return Math.Sqrt(offset.X * offset.X + offset.Y * offset.Y + offset.Z * offset.Z);
        }

        public static float FDistance(Point start, Point end) {
            return (float)Distance(start, end);
        }

        public static int TaxicabDistance(Point start, Point end) {
            Point offset = end - start;

            return Math.Abs(offset.X) + Math.Abs(offset.Y) + Math.Abs(offset.Z);
        }

        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point operator -(Point p1, Point p2) {
            return new Point(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static Point operator *(Point point, float scalar) {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f), (int)(point.Z * scalar + 0.5f));
        }

        public static Point operator *(float scalar, Point point) {
            return new Point((int)(point.X * scalar + 0.5f), (int)(point.Y * scalar + 0.5f), (int)(point.Z * scalar + 0.5f));
        }

        public static Point operator *(Point point, int scalar) {
            return new Point(point.X * scalar, point.Y * scalar, point.Z * scalar);
        }

        public static Point operator *(int scalar, Point point) {
            return new Point(point.X * scalar, point.Y * scalar, point.Z * scalar);
        }

        public static Point operator /(int scalar, Point point) {
            return new Point(point.X / scalar, point.Y / scalar, point.Z / scalar);
        }

        public static Point operator /(Point point, int scalar) {
            return new Point(point.X / scalar, point.Y / scalar, point.Z / scalar);
        }

        public static bool operator ==(Point p1, Point p2) {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2) {
            return !p1.Equals(p2);
        }

        public static Point[] Line(Point start, Point end) {
            bool steep = Math.Abs(end.Y - start.Y) > Math.Abs(end.X - start.Y);

            int swap;
            if (steep) {
                swap = start.X;
                start.X = start.Y;
                start.Y = swap;

                swap = end.X;
                end.X = end.Y;
                end.Y = swap;
            }

            if (start.X > end.X) {
                swap = start.X;
                start.X = end.X;
                end.X = swap;

                swap = start.Y;
                start.Y = end.Y;
                end.Y = swap;
            }

            int dx = end.X - start.X;
            int dy = Math.Abs(end.Y - start.Y);

            int err = dx / 2;
            int ystep = (start.Y < end.Y) ? 1 : -1;
            int y = start.Y;

            Point[] line = new Point[Math.Abs(end.X - start.X) + 1];

            for (int x = start.X; x <= end.X; x++) {
                Point point;
                if (steep) {
                    point = new Point(y, x);
                } else {
                    point = new Point(x, y);
                }
                err -= dy;
                if (err < 0) {
                    y += ystep;
                    err += dx;
                }

                line[x - start.X] = point;
            }

            return line;
        }

        public static CardinalDirection DirectionBetweenPoints(Point p1, Point p2) {
            Point difference = p2 - p1;

            CardinalDirection direction = CardinalDirection.South;
            int scalarY = Math.Abs(difference.Y);

            if (difference.Y >= 0) {
                if (difference.X > scalarY) {
                    direction = CardinalDirection.East;
                } else if (difference.X <= scalarY && difference.X >= -scalarY) {
                    direction = CardinalDirection.South;
                } else if (difference.X < -scalarY) {
                    direction = CardinalDirection.West;
                }
            } else {
                if (difference.X > scalarY) {
                    direction = CardinalDirection.East;
                } else if (difference.X <= scalarY && difference.X >= -scalarY) {
                    direction = CardinalDirection.North;
                } else if (difference.X < -scalarY) {
                    direction = CardinalDirection.West;
                }
            }

            return direction;
        }
    }
}
