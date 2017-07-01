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

	private Dictionary<Pawn, Timeline> _timelines = new Dictionary<Pawn, Timeline>();
	private int _step = 0;

	[SerializeField]private int _debugStep;

	public void AddPawn(Pawn pawn)
	{
		Timeline timeline = pawn.GetComponent<Timeline>();
		if (timeline != null)
		{
			_timelines.Add(pawn, timeline);
			SetStep(_step);
		}
	}

	public bool RemovePawn(Pawn pawn)
	{
		return _timelines.Remove(pawn);
	}

	public void SetStep(int step)
	{
		int maxStep = 0;
		foreach (Timeline timeline in _timelines.Values)
		{
			if (timeline.Count > maxStep)
			{
				maxStep = timeline.Count;
			}
		}
		_step = Math.Min(Math.Max(0, step), maxStep);

		foreach (Timeline timeline in _timelines.Values)
		{
			while (timeline.Step > Math.Max(0, _step))
			{
				PawnAction undone = timeline.CurrentAction;
				undone.Undo();
				if (ActionUndone != null) { ActionUndone(undone); }
				timeline.Back(); 
			}

			while (timeline.Step < Math.Min(timeline.Count, _step))
			{
				PawnAction done = timeline.CurrentAction;
				done.Do();
				if (ActionDone != null) { ActionDone(done); }
				timeline.Next();
			}
		}
	}

	public void TruncateToStep(Pawn pawn)
	{
		Timeline timeline;
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
		Timeline timeline;
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
			timeline.Next();
			PawnAction done = timeline.CurrentAction;
			if (ActionDone != null) { ActionDone(done); }
			SetStep(_step + 1);
		}
	}

	public void ApplyTimelines()
	{
		foreach (KeyValuePair<Pawn, Timeline> pair in _timelines)
		{
			// Push all actions on the timeline into the Pawn's action queue
			pair.Key.PushActions(pair.Value.GetPawnActions());

			// Undo all actions on the timeline, so we're at the proper state
			pair.Value.Clear();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			SetStep(_step - 1);
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			SetStep(_step + 1);
		}
		_debugStep = _step;
	}
}
