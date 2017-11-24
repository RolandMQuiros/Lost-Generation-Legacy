using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(MeshFilter))]
public class TileField : MonoBehaviour {
	[SerializeField]private BoardRef _boardRef;
	[SerializeField]private AutoTile _tile;
	private MeshFilter _meshFilter;

	private static readonly Vector3[,] _SIDE_VECTORS = new Vector3[,] {
		{ Vector3.forward, Vector3.right   }, // Top
		{ Vector3.up     , Vector3.forward }, // Right
		{ Vector3.forward, Vector3.left    }, // Down
		{ Vector3.up     , Vector3.back    }, // Left
		{ Vector3.up     , Vector3.left    }, // Forward
		{ Vector3.up     , Vector3.right   }  // Backward
	};
	
	public void Build(IEnumerable<Point> points) {
		if (_tile == null) {
			throw new NullReferenceException("This TileField has no AutoTile assigned"); 
		}

		if (_boardRef == null) {
			throw new NullReferenceException("This TileField has no Board assigned");
		}

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		
		foreach (Point point in points) {
			// Create a bitstring that marks which adjacent blocks are solid
			byte adjacency = Adjacency(point);
			byte cumulativeAdjacency = adjacency;
			
			// Get the solid adjacencies of neighboring blocks that exist in the collection
			for (int i = 0; i < Point.Neighbors.Length; i++) {
				Point offset = Point.Neighbors[i];
				Point neighbor = point + offset;
				if (points.Contains(neighbor)) {
					// Find the common solid sides between the current and neighboring blocks
					byte neighborAdjacency = Adjacency(neighbor);
					cumulativeAdjacency |= (byte)(adjacency & neighborAdjacency);
				} else {
					cumulativeAdjacency &= (byte)~(1 << i);
				}
			}

			for (int i = 0; i < Point.Neighbors.Length; i++) {
				if ((cumulativeAdjacency & (1 << i)) != 0) {
					Vector3 normal = PointVector.ToVector(Point.Neighbors[i]);
					_tile.AddTile(
						PointVector.ToVector(point) + (0.5f * normal),
						_SIDE_VECTORS[i, 0],
						_SIDE_VECTORS[i, 1],
						cumulativeAdjacency,
						vertices,
						triangles,
						uvs
					);
				}
			}
		}

		if (_meshFilter.sharedMesh == null) {
            _meshFilter.sharedMesh = new Mesh();
        } else {
		    _meshFilter.sharedMesh.Clear();
        }
		_meshFilter.sharedMesh.vertices = vertices.ToArray();
		_meshFilter.sharedMesh.triangles = triangles.ToArray();
		_meshFilter.sharedMesh.uv = uvs.ToArray();
		_meshFilter.sharedMesh.RecalculateNormals();
	}

	private byte Adjacency(Point point) {
		byte adjacency = 0;
		Board board = _boardRef.Board;
		for (int i = 0; i < Point.Neighbors.Length; i++) {
			Point offset = Point.Neighbors[i];
			Point neighbor = point + offset;

			if (board.Blocks.InBounds(neighbor) && board.Blocks.At(neighbor).IsSolid) {
				adjacency |= (byte)(1 << i);
			}
		}
		return adjacency;
	}

	private void Awake() {
		_meshFilter = GetComponent<MeshFilter>();
		_tile.Setup();
	}
}
