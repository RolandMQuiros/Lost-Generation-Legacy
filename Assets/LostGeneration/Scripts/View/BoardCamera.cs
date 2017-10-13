using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardCamera : MonoBehaviour {
    #region UnityFields
    [SerializeField]private Camera _camera;
    #endregion UnityFields

    private enum MotionType {
        Pan, Zoom, Rotate
    };
    private MotionType _motionType;
    private IEnumerator _motion;
    private Queue<IEnumerator> _motions = new Queue<IEnumerator>();

    private Vector3 _originalOffset;

     #region MotionDefs
    private IEnumerator Pan(Vector3 target, float duration, Action after = null) {
        _motionType = MotionType.Pan;
        float time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target, time / duration);
            yield return null;
        }
        transform.position = target;

        if (after != null) { after(); }
    }

    private IEnumerator Zoom(Vector3 offset, float scale, float duration, Action after = null) {
        _motionType = MotionType.Zoom;
        float time = 0f;
        
        while (time < duration) {
            time += Time.deltaTime;
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, offset / scale, time / duration);
            yield return null;
        }
        _camera.transform.localPosition = offset / scale;

        if (after != null) { after(); }
    }

    private IEnumerator Rotate(float angle, float duration, Action after = null) {
        _motionType = MotionType.Rotate;
        Quaternion target = Quaternion.Euler(0f, angle, 0f);
        float time = 0f;

        while (time < duration) {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, time / duration);
            yield return null;
        }
        transform.rotation = target;
        if (after != null) { after(); }
    }
    #endregion MotionDefs

    #region MonoBehaviour
    private void Start() {
        _originalOffset = _camera.transform.localPosition;
    }

    #endregion MonoBehaviour

    #region MotionMethods
    public void CancelPan() {
        if (_motion != null && _motionType == MotionType.Pan) {
            StopCoroutine(_motion);
        }
    }

    public void Pan(Point point, float duration) {
        Vector3 end = PointVector.ToVector(point);

        if (_motion != null && !(_motionType == MotionType.Pan)) {
            StopCoroutine(_motion);
        }
        _motion = Pan(end, duration);
        StartCoroutine(_motion);
        _motions.Clear();
    }

    public void AddPan(Point point, float duration) {
        Vector3 end = PointVector.ToVector(point);
        _motions.Enqueue(Pan(end, duration, EndMotion));
    }

    public void Zoom(float scale, float duration) {
        if (_motion != null) {
            StopCoroutine(_motion);
        }
        _motion = Zoom(_originalOffset, scale, duration);
        StartCoroutine(_motion);
        _motions.Clear();
    }

    public void AddZoom(float scale, float duration) {
        _motions.Enqueue(Zoom(_originalOffset, scale, duration, EndMotion));
    }

    public void Rotate(float angle, float duration) {
        if (_motion != null && !(_motionType == MotionType.Rotate)) {
            StopCoroutine(_motion);
        }
        _motion = Rotate(angle, duration, null);
        StartCoroutine(_motion);
        _motions.Clear();
    }

    public void AddRotate(float angle, float duration) {
        _motions.Enqueue(Rotate(angle, duration, EndMotion));
    }

    private void EndMotion() {
        if (_motions.Count > 0) {
            _motion = _motions.Dequeue();
            StartCoroutine(_motion);
        } else {
            _motion = null;
        }
    }
    #endregion MotionMethods
}
