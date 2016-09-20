using UnityEngine;
using System.Collections;
using LostGen;

public class BoardCursor : MonoBehaviour {
    public Camera Camera;
    public BoardView BoardView;
    public Vector3 ScreenPoint { get; private set; }
    public Vector3 WorldPoint { get; private set; }

    public Plane Plane { get; private set; }
    public Point Point { get; private set; }

    public void Awake() {
        Plane = new Plane(Vector3.up, transform.position.y);
        Camera = Camera ?? Camera.main;
    }

    public void Update() {
        ScreenPoint = Input.mousePosition;

        Ray screenCast = Camera.ScreenPointToRay(ScreenPoint);
        float enter;
        Plane.Raycast(screenCast, out enter);
        WorldPoint = screenCast.GetPoint(enter);

        Vector3 snapped = BoardView.Theme.Snap(WorldPoint);
        snapped.y = transform.position.y;
        transform.position = snapped;

        Point = BoardView.Theme.Vector3ToPoint(snapped);
    }
}
