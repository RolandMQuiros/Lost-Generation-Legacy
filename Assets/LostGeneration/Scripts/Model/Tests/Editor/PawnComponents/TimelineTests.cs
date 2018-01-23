using System;
using UnityEngine;
using UnityEditor;
//using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

using LostGen.Model;

namespace Tests.PawnComponents
{

	public class TimelineTests
	{

		private class TestMoveAction : PawnAction
		{
			public Point From;
			public Point To;
			public TestMoveAction(Pawn owner, Point from, Point to, int cost = 0)
				: base(owner)
			{
				From = from;
				To = to;
				Cost = cost;
			}
		}

		[Test]
		public void Truncate()
		{
			Timeline timeline = new Timeline();
			PawnAction action = new TestMoveAction(null, Point.Zero, Point.One);
			
			timeline.PushAction(action);
			Assert.AreEqual(action, timeline.Next());
			
			List<PawnAction> undone = new List<PawnAction>();
			timeline.ActionUndone += a => { undone.Add(a); };
			timeline.Clear();

			Assert.AreEqual(1, undone.Count);
			
			TestMoveAction undoneMove = (TestMoveAction)undone[0];
			Point undoneTo = undoneMove.To;
			Assert.AreEqual(Point.One, undoneTo);
		}

		[Test]
		public void TruncateEmpty()
		{
			Timeline timeline = new Timeline();	
			timeline.TruncateAt(0);
		}

		[Test]
		public void BigTruncate()
		{
			Timeline timeline = new Timeline();
			for (int i = 0; i < 10; i++)
			{
				PawnAction action = new TestMoveAction(null, Point.Zero, Point.One);
				timeline.PushAction(action);
				Assert.AreEqual(action, timeline.Next());
			}
			
			List<PawnAction> undone = new List<PawnAction>();
			timeline.ActionUndone += p => { undone.Add(p); };
			timeline.Clear();

			Assert.AreEqual(10, undone.Count);
		}

		[Test]
		public void TruncateHalf()
		{
			Timeline timeline = new Timeline();
			for (int i = 0; i < 10; i++)
			{
				PawnAction action = new TestMoveAction(null, Point.Zero, Point.One);
				timeline.PushAction(action);
				Assert.AreEqual(action, timeline.Next());
			}
			List<PawnAction> undone = new List<PawnAction>();
			timeline.ActionUndone += a => { undone.Add(a); };
			timeline.TruncateAt(5);

			Assert.AreEqual(5, undone.Count);
		}

		[Test]
		public void Iterate() {
			Timeline timeline = new Timeline();
			Point from = Point.Zero;
			Point to = new Point(1, 0, 0);
			for (int i = 0; i < 10; i++) {
				PawnAction action = new TestMoveAction(null, from, to, i);
				timeline.PushAction(action);
				from = to;
				to.X++;
			}

			Assert.AreEqual(10, timeline.Count);

			int expectedCost = 0;
			foreach (PawnAction action in timeline.GetPawnActions()) {
				Assert.NotNull(action);
				Assert.AreEqual(expectedCost++, action.Cost);
			}

			// Make sure it doesn't break from multiple iterations
			expectedCost = 0;
			foreach (PawnAction action in timeline.GetPawnActions()) {
				Assert.NotNull(action);
				Assert.AreEqual(expectedCost++, action.Cost);
			}
		}

		[Test]
		public void TruncateUndone() {
			// Truncates and checks the undone actions
			Timeline timeline = new Timeline();
			Point from = Point.Zero;
			Point to = new Point(1, 0, 0);

			List<TestMoveAction> expected = new List<TestMoveAction>();
			for (int i = 0; i < 10; i++) {
				TestMoveAction action = new TestMoveAction(null, from, to, i);
				timeline.PushAction(action);
				from = to;
				to.X++;
				if (i > 4) {
					expected.Add(action);
				}
			}
			timeline.ToLast();
			expected.Reverse();
			
			List<TestMoveAction> actual = new List<TestMoveAction>();
			timeline.ActionUndone += a => { actual.Add((TestMoveAction)a); };

			timeline.TruncateAt(4);

			for (int i = 0; i < expected.Count; i++) {
				Assert.AreEqual(expected[i].From, actual[i].From);
				Assert.AreEqual(expected[i].To, actual[i].To);
			}
		}
	}

}