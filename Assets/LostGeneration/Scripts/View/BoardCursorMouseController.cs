using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen;


[RequireComponent(typeof(BoxCollider))]
public class BoardCursorMouseController : MonoBehaviour,
                                          IBoardCursorController,
                                          IPointerClickHandler,
                                          IPointerDownHandler,
                                          IPointerUpHandler
{
    [SerializeField]private Camera _camera;
    [SerializeField]private BoardRef _boardRef;
    [SerializeField]private Vector3 _screenPoint;
    [SerializeField]private Vector3 _worldPoint;
    [SerializeField]private Point _boardPoint;
    [SerializeField]private bool _clipThroughSolids = true;
    [SerializeField]private bool _stickToGround = true;
    private BoxCollider _clickCollider;
    private bool _isWindowFocused;
    public Point BoardPoint { get { return _boardPoint; } }
    public event Action<Point> Clicked;
    public event Action<Point> TappedDown;
    public event Action<Point> TappedUp;
    public event Action<Point> Moved;

    #region MonoBehaviour
    private void Awake() {
        _clickCollider = GetComponent<BoxCollider>();
        _camera = _camera ?? Camera.main;
    }

    private void Start() {
        Bounds clickBounds = _clickCollider.bounds;
        Vector3 size = PointVector.ToVector(_boardRef.Board.Blocks.Size);
        size.y = 1f;
        clickBounds.SetMinMax(_boardRef.transform.position, size);

        _clickCollider.center = clickBounds.center;
        _clickCollider.size = clickBounds.size;
    }

    private void LateUpdate() {
        if (!IsMouseOutsideWindow() && _isWindowFocused)
        {
            _screenPoint = Input.mousePosition;

            // Get the point on the Collider beneath the cursor
            Ray screenCast = _camera.ScreenPointToRay(_screenPoint);
            RaycastHit hitInfo;
            _clickCollider.Raycast(screenCast, out hitInfo, 100f);
            _worldPoint = hitInfo.point;

            // Convert to integer Point
            Point snapped = PointVector.ToPoint(_worldPoint);
            snapped = new Point
            (
                Math.Min(Math.Max(snapped.X, 0), _boardRef.Board.Blocks.Size.X - 1),
                Math.Min(Math.Max(snapped.Y, 0), _boardRef.Board.Blocks.Size.Y - 1),
                Math.Min(Math.Max(snapped.Z, 0), _boardRef.Board.Blocks.Size.Z - 1)
            );

            Point? boardPoint = null;

            if (_clipThroughSolids)
            {
                boardPoint = snapped;
            }
            else
            {
                // Move up until a non-solid block is found
                BoardBlock block = _boardRef.Board.Blocks.At(snapped);
                if (block.IsSolid)
                {
                    while (block.IsSolid && _boardRef.Board.Blocks.InBounds(block.Point + Point.Up))
                    {
                        block = _boardRef.Board.Blocks.At(block.Point + Point.Up);
                        if (!block.IsSolid)
                        {
                            boardPoint = block.Point;
                        }
                    }
                }
                else
                {
                    // Move down until a solid block is found
                    if (_stickToGround)
                    {
                        while (!block.IsSolid && _boardRef.Board.Blocks.InBounds(block.Point + Point.Down))
                        {
                            BoardBlock below = _boardRef.Board.Blocks.At(block.Point + Point.Down);
                            if (below.IsSolid)
                            {
                                boardPoint = below.Point;
                            }
                            block = below;
                        }
                    }
                    else
                    {
                        boardPoint = block.Point;
                    }
                }
            }

            if (boardPoint != null && boardPoint.Value != _boardPoint) {
                _boardPoint = boardPoint.Value;
                if (Moved != null) { Moved(_boardPoint); }
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
