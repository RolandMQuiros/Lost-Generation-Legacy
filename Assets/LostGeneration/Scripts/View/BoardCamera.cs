using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardCamera : MonoBehaviour {
    public Camera Camera;
    public BoardView BoardView;

    #region MotionDefs
    private abstract class Motion {
        protected Transform _camera;
        protected Transform _pivot;
        protected float _duration;
        protected float _time = 0f;

        public Motion(Transform camera, Transform pivot, float duration) {
            _camera = camera;
            _pivot = pivot;
            _duration = duration;
        }
        public virtual void Begin() {
            _time = 0f;
        }
        public abstract bool Update(float deltaTime);
        public virtual void End() { }
    }

    private class PanMotion : Motion {
        private Vector3 _target;

        public PanMotion(Transform pivot, Plane boardPlane, Vector3 target, float duration)
            : base(null, pivot, duration) {
            _target = target;
        }

        public override bool Update(float deltaTime) {
            _time += deltaTime;
            _pivot.position = Vector3.Lerp(_pivot.position, _target, _time / _duration);
            return _time >= _duration;
        }

        public override void End() {
            _pivot.position = _target;
        }
    }

    private class ZoomMotion : Motion {
        private float _scale;
        private Vector3 _offset;
        public ZoomMotion(Transform camera, Transform pivot, Vector3 offset, float scale, float duration)
            : base(camera, pivot, duration) {
            _scale = scale;
            _offset = offset;
        }

        public override bool Update(float deltaTime) {
            _time += deltaTime;
            _camera.localPosition = Vector3.Lerp(_camera.localPosition, _offset / _scale, _time / _duration);
            return _time >= _duration;
        }

        public override void End() {
            _camera.localPosition = _offset / _scale;
        }
    }

    private class RotateMotion : Motion {
        private Quaternion _targetRot;
        public RotateMotion(Transform pivot, float angle, float duration)
            : base(null, pivot, duration) {
            _targetRot = Quaternion.Euler(0f, angle, 0f);
        }

        public override bool Update(float deltaTime) {
            _time += deltaTime;
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, _targetRot, _time / _duration);
            return _time >= _duration;
        }

        public override void End() {
            _pivot.rotation = _targetRot;
        }
    }
    #endregion

    private Queue<Motion> _motions = new Queue<Motion>();
    private Motion _currentMotion;

    private Vector3 _originalOffset;
    private Vector3 _directionToCamera;
    private Vector3 _target;
    private float _timer;
    private float _zoom = 1f;

    public void Awake() {
        Camera = Camera ?? GetComponentInChildren<Camera>() ?? Camera.main;
    }

    public void Start() {
        BoardView = BoardView ?? GetComponentInParent<BoardView>();
        _originalOffset = Camera.transform.localPosition;
        _directionToCamera = _originalOffset.normalized;
    }

    public void Update() {
        if (_currentMotion != null) {
            if (_currentMotion.Update(Time.deltaTime)) {
                _currentMotion.End();
                if (_motions.Count > 0) {
                    _currentMotion = _motions.Dequeue();
                }
            }
        }
    }

    public void Pan(Point point, float duration) {
        Vector3 end = BoardView.Theme.PointToVector3(point);

        if (_currentMotion != null && !(_currentMotion is PanMotion)) {
            _currentMotion.End();
        }
        _currentMotion = new PanMotion(transform, BoardView.Plane, end, duration);
        _motions.Clear();
    }

    public void AddPan(Point point, float duration) {
        Vector3 end = BoardView.Theme.PointToVector3(point);
        _motions.Enqueue(new PanMotion(transform, BoardView.Plane, end, duration));
    }

    public void Zoom(float scale, float duration) {
        if (_currentMotion != null) {
            _currentMotion.End();
        }
        _currentMotion = new ZoomMotion(Camera.transform, transform, _originalOffset, scale, duration);
        _motions.Clear();
    }

    public void AddZoom(float scale, float duration) {
        _motions.Enqueue(new ZoomMotion(Camera.transform, transform, _originalOffset, scale, duration));
    }

    public void Rotate(float angle, float duration) {
        if (_currentMotion != null && !(_currentMotion is RotateMotion)) {
            _currentMotion.End();
        }

        _currentMotion = new RotateMotion(transform, angle, duration);
        _motions.Clear();
    }

    public void AddRotate(float angle, float duration) {
        _motions.Enqueue(new RotateMotion(transform, angle, duration));
    }
}
