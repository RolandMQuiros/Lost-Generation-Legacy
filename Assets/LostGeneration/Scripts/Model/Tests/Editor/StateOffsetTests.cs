using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests {
    public class StateOffsetTests {
        [Test]
        public void GetSetFloat() {
            StateOffset state = new StateOffset();

            state.Add("SetValue", 256f);

            Assert.AreEqual(256f, state.Get("SetValue", 999f), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(12345f, state.Get("NotSetValue", 12345f), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetInt() {
            StateOffset state = new StateOffset();

            state.Add("SetValue", 256);

            Assert.AreEqual(256, state.Get("SetValue", 999), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(12345, state.Get("NotSetValue", 12345), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetBool() {
            StateOffset state = new StateOffset();

            state.Add("SetValue", true);

            Assert.IsTrue(state.Get("SetValue", false), "Assigned value for key does not match the retrieved value");
            Assert.IsTrue(state.Get("NotSetValue", true), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetPoint() {
            StateOffset state = new StateOffset();

            state.Add("SetValue", new Point(256, 512));

            Assert.AreEqual(new Point(256, 512), state.Get("SetValue", Point.Zero), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(new Point(123, 456), state.Get("NotSetValue", new Point(123, 456)), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void Subsetting() {
            StateOffset first = new StateOffset();
            first.Add("A", 1);
            first.Add("B", 2f);
            first.Add("C", true);
            first.Add("D", new Point(4, 4));

            StateOffset second = new StateOffset(first);
            second.Add("E", 123123123);

            Console.Write("First: " + first);
            Console.Write("Second: " + second);

            Assert.AreEqual(1, second.Get("A", 0));
            Assert.AreEqual(2f, second.Get("B", 0f));
            Assert.AreEqual(true, second.Get("C", false));
            Assert.AreEqual(new Point(4, 4), second.Get("D", Point.Zero));
            Assert.AreEqual(123123123, second.Get("E", 0));
        }

        [Test]
        public void Adding() {
            StateOffset first = new StateOffset();
            first.Add("A", 1);
            first.Add("B", 2f);
            first.Add("C", true);
            first.Add("D", new Point(4, 4));

            StateOffset second = new StateOffset();
            second.Add("E", 123123123);

            Console.Write("First: " + first);
            Console.Write("Second: " + second);

            second += first;

            Console.Write("First + Second: " + second);

            Assert.AreEqual(1, second.Get("A", 0));
            Assert.AreEqual(2f, second.Get("B", 0f));
            Assert.AreEqual(true, second.Get("C", false));
            Assert.AreEqual(new Point(4, 4), second.Get("D", Point.Zero));
            Assert.AreEqual(123123123, second.Get("E", 0));
        }

        [Test]
        public void IsSubset() {
            StateOffset first = new StateOffset();
            StateOffset second = new StateOffset();

            first.Add("A", 1);
            second.Add("A", 1);

            first.Add("B", 2f);
            second.Add("B", 2f);

            first.Add("C", new Point(3, 3));

            Console.Write("First: " + first);
            Console.Write("Second: " + second);

            Assert.IsTrue(second.IsSubsetOf(first), "Second was not determined to be subset to First");
            Assert.IsFalse(first.IsSubsetOf(second), "First was evaulated as subset to Second. What the hell.");
        }

        [Test]
        public void Intersect() {
            StateOffset first = new StateOffset() {
                {"A" , 1},
                {"B", 2f},
                {"C", false},
                {"D", Point.One}
            };

            StateOffset second = new StateOffset() {
                {"A" , 1},
                {"B", 2f},
                {"C", false},
                {"D", Point.Zero}
            };

            StateOffset intersection = StateOffset.Intersect(first, second);

            Assert.AreEqual(1, intersection.Get("A", 0));
            Assert.AreEqual(2f, intersection.Get("B", 0f));
            Assert.AreEqual(false, intersection.Get("C", true));
            Assert.AreEqual(Point.Max, intersection.Get("D", Point.Max));

        }
    }

}