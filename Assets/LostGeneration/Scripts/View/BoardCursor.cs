using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using LostGen;

public class BoardCursor : MonoBehaviour {
    #region UnityFields
    public Camera Camera;
    #endregion

    public Vector3 ScreenPoint { get; private set; }
    public Vector3 WorldPoint { get; private set; }

    public bool TapUp { get; private set; }
    public bool TapHeld { get; private set; }
    public bool TapDown { get; private set; }

    public Plane Plane { get; private set; }
    public Point BoardPoint { get; private set; }

    private BoardTheme _theme;
    private bool _isWindowFocused = true;

    public void Initialize(BoardTheme theme) {
        _theme = theme;
    }

    #region MonoBehaviour
    private void Awake() {
        Plane = new Plane(Vector3.up, transform.position.y);
        Camera = Camera ?? Camera.main;
    }

    private void LateUpdate() {
        if (!IsMouseOutsideWindow() && _isWindowFocused) {
            ScreenPoint = Input.mousePosition;

            Ray screenCast = Camera.ScreenPointToRay(ScreenPoint);
            float enter;
            Plane.Raycast(screenCast, out enter);
            WorldPoint = screenCast.GetPoint(enter);

            Vector3 snapped = _theme.Snap(WorldPoint);
            snapped.y = transform.position.y;
            transform.position = snapped;

            BoardPoint = _theme.Vector3ToPoint(snapped);
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
