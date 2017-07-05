using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class PlayerTimelineController : MonoBehaviour {
	public int Step
	{
		get { return _step; }
		set { SetStep(value); }
	}
	public PawnActionEvent ActionDone;
	public PawnActionEvent ActionUndone;
	public PawnActionEvent ActionAdded;

	private Dictionary<Pawn, Timeline> _timelines = new Dictionary<Pawn, Timeline>();
	private int _step = 0;

	private SkillController _skillController;

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
		step = Math.Min(Math.Max(0, step), maxStep);

		bool deactivateSkill = false;

		if (_step != step)
		{
			_step = step;
			foreach (Timeline timeline in _timelines.Values)
			{
				if (timeline.Count > 0)
				{
					while (timeline.Step > _step)
					{
						PawnAction undone = timeline.Back();
						if (undone == null) { break; }
						else
						{
							undone.Undo();
							ActionUndone.Invoke(undone);
							deactivateSkill = true;
						}
					}

					while (timeline.Step < _step)
					{
						PawnAction done = timeline.Next();
						if (done == null) { break; }
						else
						{
							done.Do();
							ActionDone.Invoke(done);
							deactivateSkill = true;
						}
					}
				}
			}
		}

		if (deactivateSkill)
		{
			_skillController.DeactivateSkill();
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
				ActionUndone.Invoke(undone[i]);
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
				ActionUndone.Invoke(undone[i]);
			}

			// Push the new action
			timeline.PushAction(action);

			// Do the new action
			PawnAction done = timeline.Next();
			done.Do();
			ActionAdded.Invoke(done);
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

	#region MonoBehaviour
	private void Awake()
	{
		_skillController = GetComponent<SkillController>();
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
	#endregion MonoBehaviour
}
