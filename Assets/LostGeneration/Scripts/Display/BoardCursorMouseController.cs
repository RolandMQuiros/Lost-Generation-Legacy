using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
    [RequireComponent(typeof(BoardCursor))]
    public class BoardCursorMouseController : MonoBehaviour,
                                              IBoardCursorController,
                                              IPointerClickHandler,
                                              IPointerDownHandler,
                                              IPointerUpHandler
    {
        #region PrivateFields
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _screenPoint;
        [SerializeField] private Vector3 _worldPoint;
        [SerializeField] private Point _boardPoint;
        [SerializeField] private bool _clipThroughSolids = true;
        [SerializeField] private bool _stickToGround = true;
        [SerializeField] private LayerMask _blockLayer;
        #endregion PrivateFields

        #region Private
        private BoardCursor _boardCursor;
        private Board _board;
        private bool _isWindowFocused;

        private Ray _screenCast;
        private Point _entryPoint;
        private Vector3 _entryV3;
        private Point _exitPoint;
        #endregion Private

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
            if (IsMouseOutsideWindow() || !_isWindowFocused) { return; }
            _screenPoint = Input.mousePosition;

            // Get the point on the Collider beneath the cursor
            _screenCast = _camera.ScreenPointToRay(_screenPoint);
            RaycastHit hitInfo;
            if (!Physics.Raycast(_screenCast, out hitInfo, 100f, _blockLayer)) { return; }

            Vector3 hitVector = _screenCast.GetPoint(hitInfo.distance - 0.5f);
            Point entry = Point.Clamp(
                PointVector.ToPoint(hitVector),
                Point.Zero,
                _board.Blocks.Size - Point.One
            );
            _entryV3 = hitVector;
            

            if (entry != _entryPoint) {
                _entryPoint = entry;

                // Get the opposing cast point
                float distanceToBoardCenter = Vector3.Distance(_screenCast.origin, _boardCursor.BoardRef.transform.position);
                Ray opposingCast = new Ray(_screenCast.GetPoint(2f * distanceToBoardCenter + 1f), -_screenCast.direction);
                _boardCursor.ClickCollider.Raycast(opposingCast, out hitInfo, 100f);

                _exitPoint = Point.Clamp(
                    PointVector.ToPoint(hitInfo.point),
                    Point.Zero,
                    _board.Blocks.Size - Point.One
                );

                // Perform a discrete line cast from the entry and exit points.
                // At the first solid block encountered, grab the previous point in the line
                Point previous = _entryPoint;
                // The linecast is performed on a grid that's twice as granular as the usual Board coordinate space
                int precision = 2;
                foreach (Point point in Point.Line(precision * _entryPoint, precision * _exitPoint)) {
                    Point boardPoint = new Point(
                        Mathf.RoundToInt((float)point.X / precision),
                        Mathf.RoundToInt((float)point.Y / precision),
                        Mathf.RoundToInt((float)point.Z / precision)
                    );
                    if (_board.Blocks.InBounds(boardPoint)) {
                        BoardBlock block = _board.Blocks.At(boardPoint);
                        if (block.IsSolid) { break; }
                        previous = boardPoint;
                    }
                }
                DownCast(previous);
            }
        }

        private void DownCast(Point from) {
            // Cast downward from the found point
            if (_board.Blocks.InBounds(from)) {
                BoardBlock downBlock = _board.Blocks.At(from);
                for (Point current = from + Point.Down;
                    _board.Blocks.InBounds(current) && !downBlock.IsSolid;
                    current += Point.Down) {
                    // If the current block is solid, the block above it is a valid cursor position
                    downBlock = _board.Blocks.At(current);
                    if (downBlock.IsSolid && _boardPoint != from && Moved != null) {
                        Moved(_boardPoint = from);
                    }
                    from = current;
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus) {
            _isWindowFocused = hasFocus;
        }

        private bool IsMouseOutsideWindow() {
            return Input.mousePosition.x < 0 || Input.mousePosition.x >= Screen.width &&
                Input.mousePosition.y < 0 || Input.mousePosition.y >= Screen.height;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(_screenCast.origin, _screenCast.GetPoint(1000f));
            Gizmos.DrawSphere(PointVector.ToVector(_entryPoint), 0.5f);
            Gizmos.DrawSphere(PointVector.ToVector(_exitPoint), 0.5f);

            Gizmos.color = Color.blue;
            foreach (Point point in Point.Line(2 * _entryPoint, 2 * _exitPoint)) {
                Gizmos.DrawSphere(PointVector.ToVector(point / 2), 0.1f);
            }
            Gizmos.DrawSphere(PointVector.ToVector(_boardPoint), 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_entryV3, 0.25f);
        }
        #endregion MonoBehaviour

        // I need to rely on the IPointer*s interfaces, since raw Input.GetMouseButton checks fire regardless of if
        // the hardware cursor is on top a UI element. IPointer* events are blocked by UI.
        #region PointerEvents

        public void OnPointerClick(PointerEventData eventData) {
            if (Clicked != null) { Clicked(_boardPoint); }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (TappedDown != null) { TappedDown(_boardPoint); }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (TappedUp != null) { TappedUp(_boardPoint); }
        }
        #endregion PointerEvents
    }
}