using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace LostGen.Test {
    public class LineTests {

        [Test]
        public void HorizontalTest() {
            Point start = Point.Zero;
            Point end = Point.Right * 10;

            Point[] line = Point.Line(start, end);

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

            Point[] line = Point.Line(start, end);

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

            Point[] line = Point.Line(start, end);
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
    }
}