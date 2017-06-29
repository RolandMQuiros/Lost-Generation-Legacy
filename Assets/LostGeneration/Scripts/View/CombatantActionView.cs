using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(CombatantAnimator))]
public class CombatantActionView : MonoBehaviour {
	public Pawn Pawn;
	private CombatantAnimator _animator;
	private Dictionary<PawnAction, Coroutine> _activeRuns = new Dictionary<PawnAction, Coroutine>();

	public void OnActionDone(PawnAction action)
	{
		if (action.Owner == Pawn)
		{
			MoveAction move = action as MoveAction;
			if (move != null)
			{
				Coroutine moveCo = StartCoroutine(DoMove(move));
				_activeRuns.Add(move, moveCo);
			}
		}
	}

	public void OnActionUndone(PawnAction action)
	{
		if (action.Owner == Pawn)
		{
			MoveAction move = action as MoveAction;
			if (move != null)
			{
				UndoMove(move);
			}
		}
	}

	private void Finish(PawnAction action) {
		// Removes the action from the list of active Coroutines
		_activeRuns.Remove(action);
	}

	#region PawnActionMethods
	private IEnumerator DoMove(MoveAction move)
	{
		yield return _animator.Move(move.Start, move.End);
		Finish(move);
	}

	private void UndoMove(MoveAction move)
	{
		Vector3 start = PointVector.ToVector(move.Start);
		Vector3 end = PointVector.ToVector(move.End);
		
		transform.position = start;
		transform.rotation = Quaternion.FromToRotation(start, start - end);
	}

	#endregion PawnActionMethods

	#region MonoBehaviour
	private void Awake()
	{
		_animator = GetComponent<CombatantAnimator>();
	}
	#endregion MonoBehaviour
}
