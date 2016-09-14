using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LostGen.Test {

    public class StateOffsetTests {

        [Test]
        public void GetSetFloat() {
            StateOffset state = new StateOffset();

            state.Set("SetValue", 256f);

            Assert.AreEqual(256f, state.Get("SetValue", 999f), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(12345f, state.Get("NotSetValue", 12345f), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetInt() {
            StateOffset state = new StateOffset();

            state.Set("SetValue", 256);

            Assert.AreEqual(256, state.Get("SetValue", 999), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(12345, state.Get("NotSetValue", 12345), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetBool() {
            StateOffset state = new StateOffset();

            state.Set("SetValue", true);

            Assert.IsTrue(state.Get("SetValue", false), "Assigned value for key does not match the retrieved value");
            Assert.IsTrue(state.Get("NotSetValue", true), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void GetSetPoint() {
            StateOffset state = new StateOffset();

            state.Set("SetValue", new Point(256, 512));

            Assert.AreEqual(new Point(256, 512), state.Get("SetValue", Point.Zero), "Assigned value for key does not match the retrieved value");
            Assert.AreEqual(new Point(123, 456), state.Get("NotSetValue", new Point(123, 456)), "Get did not return the default value for an unassigned key");
        }

        [Test]
        public void Subsetting() {
            StateOffset first = new StateOffset();
            first.Set("A", 1);
            first.Set("B", 2f);
            first.Set("C", true);
            first.Set("D", new Point(4, 4));

            StateOffset second = new StateOffset(first);
            second.Set("E", 123123123);

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
            first.Set("A", 1);
            first.Set("B", 2f);
            first.Set("C", true);
            first.Set("D", new Point(4, 4));

            StateOffset second = new StateOffset();
            second.Set("E", 123123123);

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

            first.Set("A", 1);
            second.Set("A", 1);

            first.Set("B", 2f);
            second.Set("B", 2f);

            first.Set("C", new Point(3, 3));

            Console.Write("First: " + first);
            Console.Write("Second: " + second);

            Assert.IsTrue(second.IsSubsetOf(first), "Second was not determined to be subset to First");
            Assert.IsFalse(first.IsSubsetOf(second), "First was evaulated as subset to Second. What the hell.");
        }
    }

}