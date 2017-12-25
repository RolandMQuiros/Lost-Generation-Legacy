using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;

namespace LostGen.Display {
	/// <summary>
	/// Processes how PawnActions are animated by a GameObject when the actions are added to the
	/// attached Combatant's Timeline.  Similar to CombatantView, except it processes PawnActions
	/// instead of events.
	/// </summary>
	[RequireComponent(typeof(CombatantAnimator))]
	public class TimelinePreviewer : MonoBehaviour {
		public Timeline Timeline {
			get { return _timeline; }
			set { SetTimeline(value); }
		}
		
		#region EditorFields
		[SerializeField]private float _moveDuration = 0.1f;
		#endregion EditorFields

		private Timeline _timeline;
		private CombatantAnimator _animator;
		private Dictionary<PawnAction, Coroutine> _activeRuns = new Dictionary<PawnAction, Coroutine>();

		private void SetTimeline(Timeline timeline) {
			if (_timeline != timeline) {
				if (_timeline != null) {
					_timeline.ActionDone -= OnActionDone;
					_timeline.ActionUndone -= OnActionUndone;
					_timeline.ActionsAdded -= OnActionsAdded;
				}
				
				if (timeline != null) {
					_timeline = timeline;
					_timeline.ActionDone += OnActionDone;
					_timeline.ActionUndone += OnActionUndone;
					_timeline.ActionsAdded += OnActionsAdded;
				}
			}
		}

		public void OnActionDone(PawnAction action) {
			if (action.Owner == _timeline.Pawn) {
				MoveAction move = action as MoveAction;
				if (move != null) {
					DoMove(move);
				}
			}
		}
		public void OnActionUndone(PawnAction action) {
			if (action.Owner == _timeline.Pawn) {
				MoveAction move = action as MoveAction;
				if (move != null) {
					UndoMove(move);
				}
			}
		}
		public void OnActionsAdded(IEnumerable<PawnAction> actions) {
			StartCoroutine(RunActions(actions));
		}

		private IEnumerator RunActions(IEnumerable<PawnAction> actions) {
			foreach (PawnAction action in actions) {
				if (action.Owner == _timeline.Pawn) {
					MoveAction move = action as MoveAction;
					if (move != null) {
						yield return _animator.Move(move.Start, move.End, _moveDuration);
						Finish(move);
					}
				}
			}
		}
		private void Finish(PawnAction action) {
			// Removes the action from the list of active Coroutines
			_activeRuns.Remove(action);
		}
		#region PawnActionMethods
		private IEnumerator AddMove(MoveAction move) {
			yield return _animator.Move(move.Start, move.End, _moveDuration);
			Finish(move);
		}
		private void DoMove(MoveAction move) {
			transform.position = PointVector.ToVector(move.End);
		}
		private void UndoMove(MoveAction move) {
			Vector3 start = PointVector.ToVector(move.Start);
			Vector3 end = PointVector.ToVector(move.End);
			
			transform.position = start;
			transform.rotation = Quaternion.AngleAxis(Vector3.Angle(start, end), Vector3.up);
		}
		#endregion PawnActionMethods
		#region MonoBehaviour
		private void Awake() {
			_animator = GetComponent<CombatantAnimator>();
		}
		#endregion MonoBehaviour
	}
}