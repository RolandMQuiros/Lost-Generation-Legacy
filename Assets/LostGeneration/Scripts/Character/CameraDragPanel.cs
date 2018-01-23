using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraDragPanel : UIBehaviour, IDragHandler, IScrollHandler {
	[SerializeField]private Transform _target;
	[SerializeField]private Transform _camera;
	[SerializeField]private float _rotationRate;
	[SerializeField]private float _offsetSpeed;
	[SerializeField]private Vector3 _offsetLower;
	[SerializeField]private Vector3 _offsetUpper;

    public void OnDrag(PointerEventData eventData) {
        _target.rotation *= Quaternion.Euler(0f, -eventData.delta.x * _rotationRate * Time.deltaTime, 0f);
		Vector3 newPosition = _camera.transform.position + (eventData.delta.y * _offsetSpeed * Time.deltaTime) * Vector3.up;
		_camera.transform.position = newPosition;
    }

    public void OnScroll(PointerEventData eventData) {
        Vector3 newPosition = _camera.transform.position + (eventData.scrollDelta.y * _offsetSpeed) * Vector3.forward;
		_camera.transform.position = newPosition;
    }
}