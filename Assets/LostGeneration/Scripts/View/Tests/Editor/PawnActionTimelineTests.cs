using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

using LostGen;

namespace Tests
{

	public class PawnActionTimelineTests
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
			PawnActionTimeline timeline = new PawnActionTimeline();
			timeline.PushAction(new TestMoveAction(null, Point.Zero, Point.One));

			List<PawnAction> undone = new List<PawnAction>();
			timeline.TruncateAt(0, undone);

			Assert.AreEqual(1, undone.Count);
			
			TestMoveAction undoneMove = (TestMoveAction)undone[0];
			Point undoneTo = undoneMove.To;
			// Assert.AreEqual(Point.One, undoneTo);
		}
	}

}