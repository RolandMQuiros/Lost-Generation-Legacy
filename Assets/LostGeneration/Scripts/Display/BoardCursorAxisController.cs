using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardCursorAxisController : MonoBehaviour,
										 IBoardCursorController {
	[SerializeField]private BoardRef _boardRef;
	[SerializeField]private Point _boardPoint;
	[SerializeField]private string _verticalAxis;
	[SerializeField]private string _horizontalAxis;
	[SerializeField]private float _maxSpeed;
	[SerializeField]private Vector2 _screenPoint;
	[SerializeField]private int _startingSlice = int.MaxValue;

	public Point BoardPoint { get { return _boardPoint; } }
	public event Action<Point> Clicked;
	public event Action<Point> TappedDown;
	public event Action<Point> TappedUp;
	public event Action<Point> Moved;

	#region MonoBehaviour
	private void OnValidate() {
		if (_boardRef == null) {
			throw new MissingReferenceException("BoardRef is null");
		}

		if (_boardRef.Board != null) {
			_startingSlice = _boardRef.Board.Blocks.Size.Y;
		} else {
			_startingSlice = int.MaxValue;
		}
	}

	private void LateUpdate() {
		float vertical = Input.GetAxis(_verticalAxis);
		float horizontal = Input.GetAxis(_horizontalAxis);

		if (vertical != 0f || horizontal != 0f) {
			// Move the point on screen
			_screenPoint += new Vector2(
				Mathf.Clamp(_screenPoint.x + Input.GetAxis(_verticalAxis)   * _maxSpeed * Time.deltaTime, 0f, Screen.width),
				Mathf.Clamp(_screenPoint.y + Input.GetAxis(_horizontalAxis) * _maxSpeed * Time.deltaTime, 0f, Screen.height)
			);

			// Cast from top of the Board's volume to the bottom
			Point start = PointVector.ToPoint(new Vector3(_screenPoint.x, _boardRef.Board.Blocks.Size.Y - 1, _screenPoint.y));
			start.Y = Math.Min(start.Y, _startingSlice);
			Point end = new Point(start.X, 0, start.Y);
			Point previous = start;

			foreach (Point point in Point.Line(start, end)) {
				BoardBlock block = _boardRef.Board.Blocks.At(point);
				if (block.IsSolid) { break; }
				previous = point;
			}

			if (previous != _boardPoint) {
				Moved(_boardPoint);
			}
		}
	}
	#endregion MonoBehaviour
}

