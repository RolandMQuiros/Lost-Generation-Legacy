using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen;


[RequireComponent(typeof(BoxCollider))]
public class BoardCursor : MonoBehaviour,
                           IPointerClickHandler,
                           IPointerDownHandler,
                           IPointerUpHandler
{
    [SerializeField] private Camera _camera;
    [SerializeField] private BoardRef _boardRef;
    [SerializeField] private Transform _pointer;
    public Vector3 ScreenPoint;
    public Vector3 WorldPoint;
    public Point BoardPoint;
    public bool ClipThroughSolids = true;
    public bool StickToGround = true;

    [Serializable]
    public class CursorEvent : UnityEvent<Point> { }

    public CursorEvent Clicked;
    public CursorEvent TappedDown;
    public CursorEvent TappedUp;
    public CursorEvent Moved;

    private BoxCollider _clickCollider;
    private bool _isWindowFocused;


    #region MonoBehaviour
    private void Awake()
    {
        _clickCollider = GetComponent<BoxCollider>();
        _camera = _camera ?? Camera.main;
    }

    private void Start()
    {
        Bounds clickBounds = _clickCollider.bounds;
        Vector3 size = PointVector.ToVector(_boardRef.Board.Blocks.Size);
        size.y = 1f;
        clickBounds.SetMinMax(_boardRef.transform.position, size);

        _clickCollider.center = clickBounds.center;
        _clickCollider.size = clickBounds.size;
    }

    private void LateUpdate()
    {
        if (!IsMouseOutsideWindow() && _isWindowFocused)
        {
            ScreenPoint = Input.mousePosition;

            // Get the point on the Collider beneath the cursor
            Ray screenCast = _camera.ScreenPointToRay(ScreenPoint);
            RaycastHit hitInfo;
            _clickCollider.Raycast(screenCast, out hitInfo, 100f);
            WorldPoint = hitInfo.point;

            // Convert to integer Point
            Point snapped = PointVector.ToPoint(WorldPoint);
            snapped = new Point
            (
                Math.Min(Math.Max(snapped.X, 0), _boardRef.Board.Blocks.Size.X - 1),
                Math.Min(Math.Max(snapped.Y, 0), _boardRef.Board.Blocks.Size.Y - 1),
                Math.Min(Math.Max(snapped.Z, 0), _boardRef.Board.Blocks.Size.Z - 1)
            );

            Point? boardPoint = null;

            if (ClipThroughSolids)
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
                    if (StickToGround)
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

            if (boardPoint != null && boardPoint.Value != BoardPoint)
            {
                BoardPoint = boardPoint.Value;
                Moved.Invoke(BoardPoint);
            }

            if (_pointer)
            {
                _pointer.transform.position = PointVector.ToVector(BoardPoint);
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
        Clicked.Invoke(BoardPoint);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TappedDown.Invoke(BoardPoint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TappedUp.Invoke(BoardPoint);
    }
    #endregion PointerEvents
}
