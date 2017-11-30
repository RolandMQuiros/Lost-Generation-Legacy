using System;
using UnityEngine;

[RequireComponent(typeof(TransformLimit))]
[ExecuteInEditMode]
public class ControlBone : MonoBehaviour {
    [SerializeField]private Vector3 _initialPosition;
    [SerializeField]private Quaternion _initialRotation;
    [SerializeField]private Vector3 _initialScale;

    public void Reset() {
        transform.localPosition = _initialPosition;
        transform.localRotation = _initialRotation;
        transform.localScale = _initialScale;
    }

    private void OnEnable() {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
        _initialScale = transform.localScale;
    }
}