using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TileMesh : MonoBehaviour {
	[SerializeField]private byte _adjacency;
	[SerializeField]private AutoTile2 _autoTile;
	private MeshFilter _meshFilter;

	private byte _oldAdjacency; 
	private void Awake() {
		_meshFilter = GetComponent<MeshFilter>();
		_autoTile.Init();

		Build();
	}

	private void Build() {
		Vector3[] quad = new Vector3[] {
			Vector3.up + Vector3.left,
			Vector3.up + Vector3.right,
			Vector3.down + Vector3.left,
			Vector3.down + Vector3.right
		};

		Vector3[] offsets = new Vector3[] {
			Vector3.up + Vector3.right,
			Vector3.down + Vector3.right,
			Vector3.down + Vector3.left,
			Vector3.up + Vector3.left
		};

		Rect[] miniTiles = _autoTile.GetTiles(_adjacency);

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();
		for (int i = 0; i < offsets.Length; i++) {
			tris.Add(vertices.Count);
			tris.Add(vertices.Count + 1);
			tris.Add(vertices.Count + 2);
			tris.Add(vertices.Count + 1);
			tris.Add(vertices.Count + 3);
			tris.Add(vertices.Count + 2);

			for (int j = 0; j < quad.Length; j++) {
				Vector3 vertex = (0.5f * (quad[j] + offsets[i]));
				vertices.Add(vertex);
			}
			Rect miniTile = miniTiles[i];
			uvs.Add(new Vector2(miniTile.x, miniTile.yMax));
			uvs.Add(new Vector2(miniTile.xMax, miniTile.yMax));
			uvs.Add(new Vector2(miniTile.x, miniTile.y));
			uvs.Add(new Vector2(miniTile.xMax, miniTile.y));
		}

		_meshFilter.mesh.Clear();
		_meshFilter.mesh.vertices = vertices.ToArray();
		_meshFilter.mesh.uv = uvs.ToArray();
		_meshFilter.mesh.triangles = tris.ToArray();
		_meshFilter.mesh.RecalculateNormals();
	}

	private void Update() {
		if (_oldAdjacency != _adjacency) {
			_oldAdjacency = _adjacency;
			Build();
		}
	}
}
