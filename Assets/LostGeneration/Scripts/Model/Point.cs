namespace LostGen {
    public struct Point {
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point One = new Point(1, 1);
        public static readonly Point Up = new Point(0, -1);
        public static readonly Point Down = new Point(0, 1);
        public static readonly Point Left = new Point(-1, 0);
        public static readonly Point Right = new Point(1, 0);

        public int X;
        public int Y;

        public Point(int x = 0, int y = 0) {
            X = x;
            Y = y;
        }

        public static Point operator +(Point p1, Point p2) {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2) {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point point, float scalar) {
            return new Point((int)((float)point.X * scalar + 0.5f), (int)((float)point.Y * scalar + 0.5f));
        }

        public override string ToString() {
            return "{x:" + X + ",y:" + Y + "}";
        }
    }
}
