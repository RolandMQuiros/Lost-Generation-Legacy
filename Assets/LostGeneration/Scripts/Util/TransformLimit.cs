using System;
using UnityEngine;

// Limits the transforms of the attached bone
[RequireComponent(typeof(TransformChange))]
[ExecuteInEditMode]
public class TransformLimit : MonoBehaviour {
    [Serializable]
    public struct AxisLimits {
        public Vector3 Min;
        public Vector3 Max;
        public bool LimitMinX;
        public bool LimitMaxX;
        public bool LimitMinY;
        public bool LimitMaxY;
        public bool LimitMinZ;
        public bool LimitMaxZ;

        public Vector3 Clamp(Vector3 v) {
            if (LimitMinX) { v.x = Mathf.Max(v.x, Min.x); }
            if (LimitMaxX) { v.x = Mathf.Min(v.x, Max.x); }

            if (LimitMinY) { v.y = Mathf.Max(v.y, Min.y); }
            if (LimitMaxY) { v.y = Mathf.Min(v.y, Max.y); }

            if (LimitMinZ) { v.z = Mathf.Max(v.z, Min.z); }
            if (LimitMaxZ) { v.z = Mathf.Min(v.z, Max.z); }

            return v;
        }
    }
    [SerializeField]private AxisLimits _position;
    [SerializeField]private bool _localPosition = true;
    [SerializeField]private AxisLimits _rotation;
    [SerializeField]private bool _localRotation = true;
    [SerializeField]private AxisLimits _scale = new AxisLimits() {
        Min = Vector3.one,
        Max = Vector3.one
    };

    public void OnTranslated() {
        if (_localPosition) {
            transform.localPosition = _position.Clamp(transform.localPosition);
        } else {
            transform.position = _position.Clamp(transform.position);
        }
    }

    public void OnRotated() {
        if (_localRotation) {
            transform.localEulerAngles = _rotation.Clamp(transform.localEulerAngles);
        } else {
            transform.eulerAngles = _rotation.Clamp(transform.eulerAngles);
        }
    }

    public void OnScaled() {
        transform.localScale = _scale.Clamp(transform.localScale);
    }

    private void Update() {
        if (transform.hasChanged) {
            OnTranslated();
            OnRotated();
            OnScaled();
        }
    }
} 