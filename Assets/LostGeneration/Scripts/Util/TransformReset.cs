using System;
using UnityEngine;

[RequireComponent(typeof(TransformLimit))]
[ExecuteInEditMode]
public class TransformReset : MonoBehaviour {
    public Vector3 InitialPosition {
        get { return _initialPosition; }
        set { _initialPosition = value; }
    }

    public Quaternion InitialRotation {
        get { return _initialRotation; }
        set { _initialRotation = value; }
    }

    public Vector3 InitialScale {
        get { return _initialScale; }
        set { _initialScale = value; }
    }

    [SerializeField]private Vector3 _initialPosition;
    [SerializeField]private Quaternion _initialRotation;
    [SerializeField]private Vector3 _initialScale;

    public void Set() {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
        _initialScale = transform.localScale;
    }

    public void Reset() {
        transform.localPosition = _initialPosition;
        transform.localRotation = _initialRotation;
        transform.localScale = _initialScale;
    }
}