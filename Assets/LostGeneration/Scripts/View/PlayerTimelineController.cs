using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class PlayerTimelineController : MonoBehaviour {
	public int Step
	{
		get { return _step; }
		set { SetStep(value); }
	}
	public event Action<PawnAction> ActionDone;
	public event Action<PawnAction> ActionUndone;

	private Dictionary<Pawn, PawnActionTimeline> _timelines = new Dictionary<Pawn, PawnActionTimeline>();
	private int _step = 0;

	public void AddPawn(Pawn pawn)
	{
		PawnActionTimeline timeline = new PawnActionTimeline();
		_timelines.Add(pawn, timeline);
		SetStep(_step);
	}

	public bool RemovePawn(Pawn pawn)
	{
		return _timelines.Remove(pawn);
	}

	public void SetStep(int step)
	{
		_step = step;
		foreach (PawnActionTimeline timeline in _timelines.Values)
		{
			while (timeline.Step > _step)
			{
				PawnAction undone = timeline.Back();
				if (undone != null && ActionUndone != null) { ActionUndone(undone); } 
			}

			while (timeline.Step < _step)
			{
				PawnAction done = timeline.Next();
				if (done != null && ActionDone != null) { ActionDone(done); }
			}
		}
	}

	public void TruncateToStep(Pawn pawn)
	{
		PawnActionTimeline timeline;
		if (_timelines.TryGetValue(pawn, out timeline))
		{
			// Undo all actions that have been set after the current step
			List<PawnAction> undone = new List<PawnAction>();
			timeline.TruncateAt(_step, undone);
			for (int i = 0; i < undone.Count; i++)
			{
				// Unwind the deleted actions
				if (ActionUndone != null) { ActionUndone(undone[i]); }
			}
		}
	}

	public void SetAction(PawnAction action)
	{
		PawnActionTimeline timeline;
		if (_timelines.TryGetValue(action.Owner, out timeline))
		{
			// Undo all actions that have been set after the current step
			List<PawnAction> undone = new List<PawnAction>();
			timeline.TruncateAt(_step, undone);
			for (int i = 0; i < undone.Count; i++)
			{
				// Unwind the deleted actions
				if (ActionUndone != null) { ActionUndone(undone[i]); }
			}

			// Push the new action
			timeline.PushAction(action);

			// Do the new action
			PawnAction done = timeline.Next();
			if (ActionDone != null) { ActionDone(done); }
			SetStep(_step + 1);
		}
	}

	public void ApplyTimelines()
	{
		foreach (KeyValuePair<Pawn, PawnActionTimeline> pair in _timelines)
		{
			// Push all actions on the timeline into the Pawn's action queue
			pair.Key.PushActions(pair.Value.GetPawnActions());

			// Undo all actions on the timeline, so we're at the proper state
			pair.Value.Clear();
		}
	}
}
