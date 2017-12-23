using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using LostGen.Model;

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
        #endregion PrivateFields

        #region Private
        private BoardCursor _boardCursor;
        private Board _board;
        private bool _isWindowFocused;

        private Ray _screenCast;
        private Point _entryPoint;
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
            if (!IsMouseOutsideWindow() && _isWindowFocused) {
                _screenPoint = Input.mousePosition;

                // Get the point on the Collider beneath the cursor
                _screenCast = _camera.ScreenPointToRay(_screenPoint);
                RaycastHit hitInfo;
                if (_boardCursor.ClickCollider.Raycast(_screenCast, out hitInfo, 100f)) {

                    Point entry = Point.Clamp(
                        PointVector.ToPoint(hitInfo.point),
                        Point.Zero,
                        _board.Blocks.Size - Point.One
                    );

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
                        foreach (Point point in Point.Line(_entryPoint, _exitPoint)) {
                            if (_board.Blocks.InBounds(point)) {
                                BoardBlock block = _board.Blocks.At(point);
                                if (block.IsSolid) { break; }
                                previous = point;
                            }
                        }

                        // Cast downward from the found point
                        BoardBlock downBlock = _board.Blocks.At(previous);
                        for (Point down = previous + Point.Down; !downBlock.IsSolid; down += Point.Down) {
                            downBlock = _board.Blocks.At(down);
                            if (downBlock.IsSolid) {
                                if (_boardPoint != previous) {
                                    _boardPoint = previous;

                                    if (Moved != null) {
                                        Moved(_boardPoint);
                                    }
                                }
                                break;
                            }
                            previous = down;
                        }
                    }
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
            foreach (Point point in Point.Line(_entryPoint, _exitPoint)) {
                Gizmos.DrawSphere(PointVector.ToVector(point), 0.1f);
            }

            Gizmos.DrawSphere(PointVector.ToVector(_boardPoint), 0.5f);
        }
        #endregion MonoBehaviour

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