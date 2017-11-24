using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AreaOfEffect : MonoBehaviour,
							IPointerEnterHandler,
							IPointerExitHandler {
	[SerializeField]
	private Camera _camera;
	private Coroutine _hover;
	private MeshFilter _meshFilter;
	private MeshCollider _collider;

	[SerializeField]
	private bool _debugHover;

	public void OnPointerEnter(PointerEventData data) {
		_hover = StartCoroutine(Hover());
	}

	public void OnPointerExit(PointerEventData data) {
		StopCoroutine(_hover);
		_debugHover = false;
	}

	public void OnMeshChanged() {
		_collider.sharedMesh = null;
		_collider.sharedMesh = _meshFilter.sharedMesh;
	}

	private IEnumerator Hover() {
		_debugHover = true;
		yield return true;
	}

	#region MonoBehaviour
	private void Awake() {
		_collider = GetComponent<MeshCollider>();
		_meshFilter = GetComponent<MeshFilter>();
	}
	#endregion MonoBehaviour
}
