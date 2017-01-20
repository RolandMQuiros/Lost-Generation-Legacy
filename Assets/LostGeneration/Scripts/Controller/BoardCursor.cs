using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen;


public class BoardCursor : MonoBehaviour,
                           IPointerClickHandler,
                           IPointerDownHandler,
                           IPointerUpHandler {
	public Camera Camera;
	public BoardData BoardData;
	public enum CursorAxis {
		XZ, XY
	}
	public CursorAxis Axis = CursorAxis.XZ;
	public Vector3 ScreenPoint;
	public Vector3 WorldPoint;
    public Point BoardPoint;

    [Serializable]
    public class CursorEvent : UnityEvent<Point> { }

    public CursorEvent Clicked;
    public CursorEvent TappedDown;
    public CursorEvent TappedUp;

	private Collider _clickCollider;
	private bool _isWindowFocused;


	#region MonoBehaviour
    private void Awake() {
		_clickCollider = GetComponent<Collider>();
        Camera = Camera ?? Camera.main;
    }

    private void Update() {
        // float shiftPlane = Input.GetAxis("Shift Plane");
        // if (shiftPlane > 0) {
        //     PlanePosition++;
        // } else if (shiftPlane < 0) {
        //     PlanePosition--;
        // }
        
        // if (shiftPlane != 0) {
        //     switch (Axis) {
        //         case CursorAxis.XY:
        //             transform.position.Set(transform.position.x, transform.position.y, (float)PlanePosition);
        //             break;
        //         case CursorAxis.XZ:
        //             transform.position.Set(transform.position.x, (float)PlanePosition, transform.position.z);
        //             break;
        //         case CursorAxis.ZY:
        //             transform.position.Set((float)PlanePosition, transform.position.y, transform.position.z);
        //             break;
        //     }
        // }

        bool flipPlane = Input.GetButtonDown("Flip Plane");
        if (flipPlane) {
            Axis = (CursorAxis)(((int)Axis + 1) % 2);
            switch (Axis) {
                case CursorAxis.XY:
                    transform.rotation = Quaternion.AngleAxis(90f, Vector3.right);
                    break;
                case CursorAxis.XZ:
                    transform.rotation = Quaternion.identity;
                    break;
            }   
        }
    }

    private void LateUpdate() {
        if (!IsMouseOutsideWindow() && _isWindowFocused) {
            ScreenPoint = Input.mousePosition;

            // Get the point on the Collider beneath the cursor
            Ray screenCast = Camera.ScreenPointToRay(ScreenPoint);
			RaycastHit hitInfo;
			_clickCollider.Raycast(screenCast, out hitInfo, 100f);
            WorldPoint = hitInfo.point;
            
            // Convert to integer Point
			Vector3 snapped = PointVector.Round(WorldPoint);
            Point boardPoint = PointVector.ToPoint(snapped);

            BoardPoint = boardPoint;
        }
    }

    private void OnApplicationFocus(bool hasFocus) {
        _isWindowFocused = hasFocus;
    }

    private bool IsMouseOutsideWindow() {
        return Input.mousePosition.x < 0 || Input.mousePosition.x >= Screen.width &&
               Input.mousePosition.y < 0 || Input.mousePosition.y >= Screen.height;
    }
    #endregion MonoBehaviour

    #region PointerEvents

    public void OnPointerClick(PointerEventData eventData) {
        Clicked.Invoke(BoardPoint);
    }

    public void OnPointerDown(PointerEventData eventData) {
        TappedDown.Invoke(BoardPoint);
    }

    public void OnPointerUp(PointerEventData eventData) {
        TappedUp.Invoke(BoardPoint);
    }
    #endregion PointerEvents
}
