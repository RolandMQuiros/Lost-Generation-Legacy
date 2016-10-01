using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardCameraController : MonoBehaviour {
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
        public virtual void Begin() { }
        public abstract bool Update(float deltaTime);
    }

    private class PanMotion : Motion {
        private Vector3 _target;

        public PanMotion(Transform pivot, Plane boardPlane, Vector3 target, float duration)
            : base(null, pivot, duration) {
            _target = target;
        }

        public override bool Update(float deltaTime) {
            _time += deltaTime;
            _pivot.position = Vector3.Lerp(_pivot.position, _target, deltaTime / _duration);
            return _time >= _duration;
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
            _camera.localPosition = Vector3.Lerp(_camera.localPosition, _scale * _offset, _time / _duration);
            return _time >= _duration;
        }
    }
    #endregion

    private Queue<Motion> _motions = new Queue<Motion>(); 
    private Vector3 _originalOffset;
    private Vector3 _directionToCamera;
    private Vector3 _target;
    private float _timer;

    public void Awake() {
        Camera = Camera ?? GetComponentInChildren<Camera>() ?? Camera.main;
    }

    public void Start() {
        BoardView = BoardView ?? GetComponentInParent<BoardView>();
        _originalOffset = transform.localPosition;
        _directionToCamera = _originalOffset.normalized;
    }

    public void Update() {
        if (_motions.Count > 0) {
            if (_motions.Peek().Update(Time.deltaTime)) {
                _motions.Dequeue();
            }
        }
    }

    public void AddPan(Point point, float duration) {
        Vector3 end = BoardView.Theme.PointToVector3(point);
        _motions.Enqueue(new PanMotion(transform, BoardView.Plane, end, duration));
    }

    public void AddZoom(float scale, float duration) {
        _motions.Enqueue(new ZoomMotion(Camera.transform, transform, _originalOffset, scale, duration));
    }
}
