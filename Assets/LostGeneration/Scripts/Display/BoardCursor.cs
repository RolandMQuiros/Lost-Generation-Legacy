using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
	/// <summary>
	/// A cursor constrained to a <see cref="Board"/>
	/// </summary>
	/// <remarks>
	/// This <see cref="MonoBehaviour"/> searches its <see cref="GameObject"/> for other MonoBehaviours that inherit from
	/// <see cref="IBoardCursorController"/>. IBoardCursorController classes map device inputs to cursor events, allowing
	/// control from gamepads, mice, keyboards, touchscreens, or any other device.
	/// </remarks>
	[RequireComponent(typeof(BoxCollider))]
	public class BoardCursor : MonoBehaviour {
		[Serializable]private class BoardCursorEvent : UnityEvent<Point> { }

		[Tooltip("The cursor pointer")]
		[SerializeField]private Transform _pointer;
		[Tooltip("Reference to the Board")]
		[SerializeField]private BoardRef _boardRef;
		[Tooltip("The cursor's current Point on the Board")]
		[SerializeField]private Point _boardPoint;
		[Tooltip("Called when the player taps and releases")]
		[SerializeField]private BoardCursorEvent _clicked;
		public event Action<Point> Clicked;
		[Tooltip("Called when the player taps down")]
		[SerializeField]private BoardCursorEvent _tappedDown;
		public event Action<Point> TappedDown;
		[Tooltip("Called when the player releases a tap")]
		[SerializeField]private BoardCursorEvent _tappedUp;
		public event Action<Point> TappedUp;
		[Tooltip("Called when the cursor's position moves")]
		[SerializeField]private BoardCursorEvent _moved;
		public event Action<Point> Moved;
		private BoxCollider _clickCollider;
		
		/// <summary>This cursor's <see cref="Point"/> on the <see cref="Board"/></summary>
		/// <returns>The current <see cref="Point"/></returns>
		public Point BoardPoint { get { return _boardPoint; } }
		/// <returns>The <see cref="BoardRef"/> this cursor is attached to</returns>
		public BoardRef BoardRef { get { return _boardRef; } }
		/// <summary>
		/// A <see cref="BoxCollider"/> that can be used to capture raycasts, for inputs that need it
		/// </summary>
		/// <returns>Reference to this BoardCursor's <see cref="BoxCollider"/></returns>
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
			_moved.Invoke(_boardPoint);
			if (Moved != null) { Moved(_boardPoint); }
			if (_pointer != null) {
				_pointer.transform.position = PointVector.ToVector(_boardPoint);
			}
		}

		#region MonoBehaviour
		private void Awake() {
			_clickCollider = GetComponent<BoxCollider>();
			foreach (IBoardCursorController controller in GetComponents<IBoardCursorController>()) {
				controller.Clicked    += p => { if (Clicked != null) Clicked(p); };
				controller.TappedDown += p => { if (TappedDown != null) TappedDown(p); };
				controller.TappedUp   += p => { if (TappedUp != null) TappedUp(p); };

				controller.Clicked    += _clicked.Invoke;
				controller.TappedDown += _tappedDown.Invoke;
				controller.TappedUp   += _tappedUp.Invoke;
				controller.Moved      += OnMoved;
			}
		}

		private void Start() {
			ResizeClickCollider();
		}

		#endregion MonoBehaviour
	}
}