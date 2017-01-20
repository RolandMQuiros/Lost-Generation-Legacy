using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen;


public class BoardCursor : MonoBehaviour {
	public Camera Camera;
	public BoardData BoardData;
	public enum CursorAxis {
		XZ = 0, XY = 1, ZY = 2
	}
	public CursorAxis Axis = CursorAxis.XZ;
    public int PlanePosition = 0;
	public Vector3 ScreenPoint;
	public Vector3 WorldPoint;
    public Point BoardPoint;
	private Collider _clickCollider;
	private bool _isWindowFocused;
	#region MonoBehaviour
    private void Awake() {
		_clickCollider = GetComponent<Collider>();
        Camera = Camera ?? Camera.main;
    }

    private void Update() {
        float shiftPlane = Input.GetAxis("Shift Plane");
        if (shiftPlane > 0) {
            PlanePosition++;
        } else if (shiftPlane < 0) {
            PlanePosition--;
        }
        
        if (shiftPlane != 0) {
            switch (Axis) {
                case CursorAxis.XY:
                    transform.position.Set(transform.position.x, transform.position.y, (float)PlanePosition);
                    break;
                case CursorAxis.XZ:
                    transform.position.Set(transform.position.x, (float)PlanePosition, transform.position.z);
                    break;
                case CursorAxis.ZY:
                    transform.position.Set((float)PlanePosition, transform.position.y, transform.position.z);
                    break;
            }
        }

        bool flipPlane = Input.GetButtonDown("Flip Plane");
        if (flipPlane) {
            Axis = (CursorAxis)(((int)Axis + 1) % 3);
            switch (Axis) {
                case CursorAxis.XY:
                    transform.rotation = Quaternion.AngleAxis(90f, Vector3.right);
                    break;
                case CursorAxis.XZ:
                    transform.rotation = Quaternion.identity;
                    break;
                case CursorAxis.ZY:
                    transform.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
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

            // Set the controlled axis on the point to the Plane position
            switch (Axis) {
                case CursorAxis.XY:
                    boardPoint.Z = PlanePosition;
                    break;
                case CursorAxis.XZ:
                    boardPoint.Y = PlanePosition;
                    break;
            }

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
}
