using UnityEngine;
using UnityEditor;
//using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

using LostGen;

namespace Tests.PawnComponents
{

	public class TimelineTests
	{

		private class TestMoveAction : PawnAction
		{
			public Point From;
			public Point To;
			public TestMoveAction(Pawn owner, Point from, Point to)
				: base(owner)
			{
				From = from;
				To = to;
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
			timeline.TruncateAt(0, undone);

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
			timeline.TruncateAt(0, undone);

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
			timeline.TruncateAt(5, undone);

			Assert.AreEqual(5, undone.Count);
		}
	}

}