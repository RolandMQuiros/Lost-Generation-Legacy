using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen;


[RequireComponent(typeof(BoardCursor))]
public class BoardCursorMouseController : MonoBehaviour,
                                          IBoardCursorController,
                                          IPointerClickHandler,
                                          IPointerDownHandler,
                                          IPointerUpHandler
{
    [SerializeField]private Camera _camera;
    [SerializeField]private Vector3 _screenPoint;
    [SerializeField]private Vector3 _worldPoint;
    [SerializeField]private Point _boardPoint;
    [SerializeField]private bool _clipThroughSolids = true;
    [SerializeField]private bool _stickToGround = true;
    private BoardCursor _boardCursor;
    private Board _board;
    private bool _isWindowFocused;
    public Point BoardPoint { get { return _boardPoint; } }
    public event Action<Point> Clicked;
    public event Action<Point> TappedDown;
    public event Action<Point> TappedUp;
    public event Action<Point> Moved;

    #region MonoBehaviour
    private void Awake() {
        _boardCursor = GetComponent<BoardCursor>();
        _camera = _camera ?? Camera.main;
    }

    private void Start() {
        _board = _boardCursor.BoardRef.Board;
    }

    private void LateUpdate() {
        if (!IsMouseOutsideWindow() && _isWindowFocused)
        {
            _screenPoint = Input.mousePosition;

            // Get the point on the Collider beneath the cursor
            Ray screenCast = _camera.ScreenPointToRay(_screenPoint);
            RaycastHit hitInfo;
            _boardCursor.ClickCollider.Raycast(screenCast, out hitInfo, 100f);
            
            Point entryPoint = PointVector.ToPoint(hitInfo.point);

            // Get the opposing cast point
            float distanceToBoardCenter = Vector3.Distance(screenCast.origin, _boardCursor.BoardRef.transform.position);
            Ray opposingCast = new Ray(screenCast.GetPoint(2f * distanceToBoardCenter + 1f), -screenCast.direction);
            _boardCursor.ClickCollider.Raycast(opposingCast, out hitInfo, 100f);

            Point exitPoint = PointVector.ToPoint(hitInfo.point);

            // Perform a discrete line cast from the entry and exit points
            foreach (Point point in Point.Line(entryPoint, exitPoint)) {
                if (_board.Blocks.InBounds(point)) {
                    BoardBlock block = _board.Blocks.At(point);
                    if (block.IsSolid) {
                        _boardPoint = point;
                        break;
                    }
                }
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _isWindowFocused = hasFocus;
    }

    private bool IsMouseOutsideWindow()
    {
        return Input.mousePosition.x < 0 || Input.mousePosition.x >= Screen.width &&
               Input.mousePosition.y < 0 || Input.mousePosition.y >= Screen.height;
    }
    #endregion MonoBehaviour

    #region PointerEvents

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clicked != null) { Clicked(_boardPoint); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TappedDown != null) { TappedDown(_boardPoint); }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TappedUp != null) { TappedUp(_boardPoint); }
    }
    #endregion PointerEvents
}
