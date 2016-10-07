using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class BoardCursor : MonoBehaviour {
    public Camera Camera;
    public BoardView BoardView;
    public Vector3 ScreenPoint { get; private set; }
    public Vector3 WorldPoint { get; private set; }

    public bool TapDown { get; private set; }
    public bool TapHeld { get; private set; }
    public bool TapUp { get; private set; }

    public bool DebugTapDown;
    public bool DebugTapHeld;
    public bool DebugTapUp;

    public Plane Plane { get; private set; }
    public Point BoardPoint { get; private set; }

    private bool _isWindowFocused = true;

    public void Awake() {
        Plane = new Plane(Vector3.up, transform.position.y);
        Camera = Camera ?? Camera.main;
        BoardView = BoardView ?? GetComponentInParent<BoardView>();
    }

    public void LateUpdate() {
        if (!IsMouseOutsideWindow() && _isWindowFocused) {
            ScreenPoint = Input.mousePosition;

            Ray screenCast = Camera.ScreenPointToRay(ScreenPoint);
            float enter;
            Plane.Raycast(screenCast, out enter);
            WorldPoint = screenCast.GetPoint(enter);

            Vector3 snapped = BoardView.Theme.Snap(WorldPoint);
            snapped.y = transform.position.y;
            transform.position = snapped;

            BoardPoint = BoardView.Theme.Vector3ToPoint(snapped);

            TapDown = Input.GetMouseButtonDown(0);
            TapHeld = Input.GetMouseButton(0);
            TapUp = Input.GetMouseButtonUp(0);
        }

        DebugTapDown = TapDown;
        DebugTapHeld = TapHeld;
        DebugTapUp = TapUp;
    }

    public void OnApplicationFocus(bool hasFocus) {
        _isWindowFocused = hasFocus;
    }

    private bool IsMouseOutsideWindow() {
        return Input.mousePosition.x < 0 || Input.mousePosition.x >= Screen.width &&
               Input.mousePosition.y < 0 || Input.mousePosition.y >= Screen.height;
    }
}
