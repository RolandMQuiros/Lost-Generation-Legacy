using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests {
    public class LineTests {

        [Test]
        public void HorizontalTest() {
            Point start = Point.Zero;
            Point end = Point.Right * 10;

            Point[] line = Point.Line(start, end).ToArray();

            for (int i = 0; i < line.Length; i++) {
                Console.Write(line[i]);
            }

            for (int i = 0; i < line.Length; i++) {
                Assert.AreEqual(Point.Right * i, line[i]);
            }
        }

        [Test]
        public void VerticalTest() {
            Point start = new Point(16, 23);
            Point end = start + (Point.Up * 10);

            Point[] line = Point.Line(start, end).ToArray();

            int i = 0;
            foreach (Point point in line.OrderBy(p => Point.TaxicabDistance(start, p))) {
                Point expected = start + Point.Up * i++;
                Console.Write("Expected: " + expected + "; Actual: " + point);
                Assert.AreEqual(expected, point);
            }
        }

        [Test]
        public void Diagonal() {
            Point start = new Point(16, 23);
            Point end = start + ((Point.Up + Point.Left) * 10);

            Point[] line = Point.Line(start, end).ToArray();
            Point[] expected = new Point[line.Length];

            Point expPoint = start;
            int i = 0;
            while (!expPoint.Equals(end)) {
                expected[i++] = expPoint;
                expPoint += Point.Up + Point.Left;
            }
            expected[i] = end;

            i = 0;
            foreach (Point linePoint in line.OrderBy(p => Point.TaxicabDistance(start, p))) {
                Console.Write("Expected: " + expected[i] + "; Actual: " + linePoint);
                Assert.AreEqual(expected[i++], linePoint);
            }
        }

        [Test]
        public void Diagonal3DPositive() {
            Point start = Point.Zero;
            Point end = Point.One * 10;

            int count = 0;
            foreach (Point point in Point.Line(start, end)) {
                Assert.AreEqual(Point.One * count++, point);
            }
        }

        [Test]
        public void Diagonal3DPositiveShallow() {
            Point start = Point.Zero;
            Point end = new Point(10, 5, 10);

            float distance = Point.FDistance(Point.Zero, end);
            float stepX = end.X / distance;
            float stepY = end.Y / distance;
            float stepZ = end.Z / distance;

            List<Point> line = new List<Point>(Point.Line(start, end));
            line.ForEach(point => Console.Write(point));

            int count = 0;
            foreach (Point point in line) {
                Point expected = new Point(
                    (int)(stepX * count + 0.5f),
                    (int)(stepY * count + 0.5f),
                    (int)(stepZ * count + 0.5f)
                );
                Assert.AreEqual(expected, point);
                count++;
            }
        }
    }
}