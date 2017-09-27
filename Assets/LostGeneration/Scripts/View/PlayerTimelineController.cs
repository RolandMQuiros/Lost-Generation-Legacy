using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class PlayerTimelineController : MonoBehaviour {
	public int Step {
		get { return _step; }
		set { SetAllStep(value); }
	}
	public int MaxStep {
		get {
			int maxStep = 0;
			foreach (Timeline timeline in _timelines.Values) {
				if (timeline.Count > maxStep) {
					maxStep = timeline.Count;
				}
			}
			return maxStep;
		}
	}
	public PawnActionEvent ActionDone;
	public PawnActionEvent ActionUndone;
	public PawnActionsEvent ActionsAdded;
	public PawnActionEvent ActionInterrupted;

	private Dictionary<Pawn, Timeline> _timelines = new Dictionary<Pawn, Timeline>();
	private int _step = 0;

	private PlayerSkillController _skillController;

	[SerializeField]private int _debugStep;

	public void AddPawn(Pawn pawn) {
		Timeline timeline = pawn.GetComponent<Timeline>();
		if (timeline != null) {
			_timelines.Add(pawn, timeline);
			SetAllStep(_step);
		}
	}

	public bool RemovePawn(Pawn pawn) {
		return _timelines.Remove(pawn);
	}

	public void SetAllStep(int step) {
		_step = Math.Min(Math.Max(0, step), MaxStep);

		foreach (Pawn pawn in _timelines.Keys) {
			SetPawnStep(pawn, _step);
		}
	}

	///<summary>
	///Sets the step of all Pawns with less than or equal Agility to the pivot, excluding the pivot.
	///</summary>
	public void SetSlowerStep(int step, Pawn pivot) {
		step = Math.Min(Math.Max(0, step), MaxStep);

		int pivotAgility = pivot.RequireComponent<PawnStats>().Base.Agility;

		foreach (Pawn pawn in _timelines.Keys) {
			if (pawn != pivot) {
				int agility = pawn.RequireComponent<PawnStats>().Base.Agility;
				if (agility <= pivotAgility) {
					SetPawnStep(pawn, step);
				}
			}
		}
	}

	public void TruncatePawnToStep(Pawn pawn) {
		Timeline timeline;
		if (_timelines.TryGetValue(pawn, out timeline)) {
			// Undo all actions that have been set after the current step
			List<PawnAction> undone = new List<PawnAction>();
			timeline.TruncateAt(_step, undone);
			for (int i = 0; i < undone.Count; i++)
			{
				pawn.RequireComponent<ActionPoints>().Current -= undone[i].Cost;
				// Unwind the deleted actions
				ActionUndone.Invoke(undone[i]);
			}
		}
	}

	public void SetStepActions(IEnumerable<PawnAction> actions) {
		Queue<PawnAction> added = new Queue<PawnAction>();
		foreach (PawnAction action in actions) {
			Timeline timeline;
			if (_timelines.TryGetValue(action.Owner, out timeline)) {
				// Undo all actions that have been set after the current step
				List<PawnAction> undone = new List<PawnAction>();
				timeline.TruncateAt(_step, undone);

				for (int i = 0; i < undone.Count; i++) {
					// Unwind the deleted actions
					ActionUndone.Invoke(undone[i]);
				}

				// Push the new action
				timeline.PushAction(action);

				// Do the new action
				PawnAction done = timeline.Next();
				done.Do();
				action.Owner.RequireComponent<ActionPoints>().Current -= done.Cost;

				added.Enqueue(done);
				SetAllStep(_step + 1);
			}
		}

		if (added.Count > 0) {
			ActionsAdded.Invoke(added);
		}
	}

	public void ApplyTimelines() {
		foreach (KeyValuePair<Pawn, Timeline> pair in _timelines) {
			// Push all actions on the timeline into the Pawn's action queue
			pair.Key.PushActions(pair.Value.GetPawnActions());

			// Undo all actions on the timeline, so we're at the proper state
			pair.Value.Clear();
		}
	}

	public void SetPawnStep(Pawn pawn, int step) {
		Timeline timeline;
		if (_timelines.TryGetValue(pawn, out timeline)) {
			ActionPoints actionPoints = pawn.RequireComponent<ActionPoints>();

			if (timeline.Count > 0) {
				while (timeline.Step > step) {
					PawnAction undone = timeline.Back();
					if (undone == null) { break; }
					else {
						undone.Undo();
						actionPoints.Current += undone.Cost;
						ActionUndone.Invoke(undone);
					}
				}

				while (timeline.Step < step) {
					PawnAction done = timeline.Next();
					if (done == null) { break; }
					else {
						done.Do();
						actionPoints.Current -= done.Cost;
						ActionDone.Invoke(done);
					}
				}
			}
		}
	}

	#region MonoBehaviour
	private void Awake()
	{
		_skillController = GetComponent<PlayerSkillController>();
	}

	private void Update() {
		_debugStep = _step;
	}
	#endregion MonoBehaviour
}
