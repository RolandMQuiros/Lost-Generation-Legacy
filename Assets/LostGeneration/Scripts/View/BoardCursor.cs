using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(BoxCollider))]
public class BoardCursor : MonoBehaviour {

	[SerializeField]private Transform _pointer;
	[SerializeField]private BoardRef _boardRef;
	[Serializable]private class BoardCursorEvent : UnityEvent<Point> { }
	[SerializeField]private Point _boardPoint;
	[SerializeField]private BoardCursorEvent Clicked;
    [SerializeField]private BoardCursorEvent TappedDown;
    [SerializeField]private BoardCursorEvent TappedUp;
    [SerializeField]private BoardCursorEvent Moved;

	private BoxCollider _clickCollider;
	public Point BoardPoint { get { return _boardPoint; } }
	public BoardRef BoardRef { get { return _boardRef; } }
	public BoxCollider ClickCollider { get { return _clickCollider; } }

	private void ResizeClickCollider() {
		Bounds clickBounds = _clickCollider.bounds;
        Vector3 sizeVector = PointVector.ToVector(_boardRef.Board.Blocks.Size) - PointVector.ToVector(Point.One) * 0.5f;
        clickBounds.SetMinMax(_boardRef.transform.position - Vector3.one * 0.5f, sizeVector);

        _clickCollider.center = clickBounds.center;
        _clickCollider.size = clickBounds.size;
	}

	private void OnMoved(Point point) {
		_boardPoint = point;
		Moved.Invoke(_boardPoint);
		if (_pointer != null) {
			_pointer.transform.position = PointVector.ToVector(_boardPoint);
		}
	}

	#region MonoBehaviour
	private void Awake() {
		_clickCollider = GetComponent<BoxCollider>();
		foreach (IBoardCursorController controller in GetComponents<IBoardCursorController>()) {
			controller.Clicked    += Clicked.Invoke;
			controller.TappedDown += TappedDown.Invoke;
			controller.TappedUp   += TappedUp.Invoke;
			controller.Moved      += OnMoved;
		}
	}

	private void Start() {
		ResizeClickCollider();
	}

	#endregion MonoBehaviour
}
