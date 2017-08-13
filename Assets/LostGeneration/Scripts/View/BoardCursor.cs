using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class BoardCursor : MonoBehaviour {

	[SerializeField]private Transform _pointer;
	[Serializable]private class BoardCursorEvent : UnityEvent<Point> { }
	[SerializeField]private Point _boardPoint;
	[SerializeField]private BoardCursorEvent Clicked;
    [SerializeField]private BoardCursorEvent TappedDown;
    [SerializeField]private BoardCursorEvent TappedUp;
    [SerializeField]private BoardCursorEvent Moved;

	public Point BoardPoint { get { return _boardPoint; } }

	private void OnMoved(Point point) {
		_boardPoint = point;
		Moved.Invoke(_boardPoint);
		if (_pointer != null) {
			_pointer.transform.position = PointVector.ToVector(_boardPoint);
		}
	}

	#region MonoBehaviour
	private void Awake() {
		foreach (IBoardCursorController controller in GetComponents<IBoardCursorController>()) {
			controller.Clicked    += Clicked.Invoke;
			controller.TappedDown += TappedDown.Invoke;
			controller.TappedUp   += TappedUp.Invoke;
			controller.Moved      += OnMoved;
		}
	}
	#endregion MonoBehaviour
}
