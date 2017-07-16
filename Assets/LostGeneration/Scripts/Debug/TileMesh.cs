using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class TileMesh : MonoBehaviour {
	[SerializeField]private byte _adjacency;
	[SerializeField]private AutoTile _autoTile;
	private MeshFilter _meshFilter;

	private byte _oldAdjacency; 
	private void Awake() {
		_meshFilter = GetComponent<MeshFilter>();
		_autoTile.Setup();

		Build();
	}

	private void Build() {
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();
		
		_autoTile.AddTile(
			Vector3.zero,
			Vector3.up,
			Vector3.right,
			_adjacency,
			vertices,
			tris,
			uvs
		);

		if (_meshFilter.sharedMesh == null) { _meshFilter.sharedMesh = new Mesh(); }
		else { _meshFilter.sharedMesh.Clear(); }
		_meshFilter.sharedMesh.vertices = vertices.ToArray();
		_meshFilter.sharedMesh.uv = uvs.ToArray();
		_meshFilter.sharedMesh.triangles = tris.ToArray();
		_meshFilter.sharedMesh.RecalculateNormals();
	}

	private void Update() {
		if (_oldAdjacency != _adjacency) {
			_oldAdjacency = _adjacency;
			Build();
		}
	}
}
