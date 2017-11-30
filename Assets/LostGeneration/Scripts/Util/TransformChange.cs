using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class TransformChange : MonoBehaviour {
    [Serializable]private class TransformEvent : UnityEvent<Transform> { }
    [Serializable]private class Vector3Event : UnityEvent<Vector3, Vector3> { }
    [Serializable]private class QuaternionEvent : UnityEvent<Quaternion, Quaternion> { }

    [SerializeField]private TransformEvent Changed;
    [SerializeField]private Vector3Event Translated;
    [SerializeField]private Vector3Event LocallyTranslated;
    [SerializeField]private QuaternionEvent Rotated;
    [SerializeField]private QuaternionEvent LocallyRotated;
    [SerializeField]private Vector3Event Scaled;
    [SerializeField]private Vector3Event LocallyScaled;
    
    [SerializeField][HideInInspector]private Vector3 oldPosition;
    [SerializeField][HideInInspector]private Vector3 oldLocalPosition;
    [SerializeField][HideInInspector]private Quaternion oldRotation;
    [SerializeField][HideInInspector]private Quaternion oldLocalRotation;
    [SerializeField][HideInInspector]private Vector3 oldScale;
    [SerializeField][HideInInspector]private Vector3 oldLocalScale;

    private void PostUpdate() {
        if (transform.hasChanged) {
            Changed.Invoke(transform);
            if (transform.position != oldPosition) {
                Translated.Invoke(oldPosition, transform.position);
                oldPosition = transform.position;
            }
            if (transform.localPosition != oldLocalPosition) {
                LocallyTranslated.Invoke(oldLocalPosition, transform.localPosition);
                oldLocalPosition = transform.localPosition;
            }
            if (transform.rotation != oldRotation) {
                Rotated.Invoke(oldRotation, transform.rotation);
                oldRotation = transform.rotation;
            }
            if (transform.localRotation != oldLocalRotation) {
                LocallyRotated.Invoke(oldLocalRotation, transform.localRotation);
                oldLocalRotation = transform.localRotation;
            }
            if (transform.lossyScale != oldScale) {
                Scaled.Invoke(oldScale, transform.lossyScale);
                oldScale = transform.lossyScale;
            }
            if (transform.localScale != oldLocalScale) {
                LocallyScaled.Invoke(oldLocalScale, transform.localScale);
                oldLocalScale = transform.localScale;
            }
        }
        transform.hasChanged = false;
    }
}