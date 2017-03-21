using System;
using System.Collections.Generic;
using UnityEngine;

using LostGen;

public class BlockMesh : MonoBehaviour {
	public Point Size;
	private static readonly int[,] _BLOCK_SIDE_INDICES = new int[,] {
		{1, 3, 0, 1, 2, 3}, // Top
		{1, 6, 2, 6, 5, 1}, // Right
		{4, 6, 5, 6, 7, 4}, // Down
		{3, 4, 0, 4, 7, 3}, // Left
		{0, 5, 1, 5, 4, 0}, // Forward
		{2, 7, 3, 7, 6, 2}, // Backward
	};
	private static readonly Vector3[] _BLOCK_VERTICES = new Vector3[] {
		Vector3.up + Vector3.forward + Vector3.left,    // 0
		Vector3.up + Vector3.forward + Vector3.right,	// 1
		Vector3.up + Vector3.back + Vector3.right, 		// 2
		Vector3.up + Vector3.back + Vector3.left,		// 3
		Vector3.down + Vector3.forward + Vector3.left,  // 4
		Vector3.down + Vector3.forward + Vector3.right, // 5
		Vector3.down + Vector3.back + Vector3.right,    // 6
		Vector3.down + Vector3.back + Vector3.left		// 7
	};

	private int[,,] _blocks;
	private MeshFilter _meshFilter;
	public void SetBlock(Point point, int blockType) {
		if (point.X >= 0 && point.X < Size.X && point.Y >= 0 && point.Y < Size.Y && point.Z >= 0 && point.Z < Size.Z) {
			_blocks[point.X, point.Y, point.Z] = blockType;
		} else {
			throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + Size + " of this BlockMesh");
		}
	}

	public void Build() {
		// Create a new block array based on new Size, if it changed
		Point size = new Point(_blocks.GetLength(0), _blocks.GetLength(1), _blocks.GetLength(2));
		if (size != Size) {
			int[,,] newBlocks = new int[Size.X, Size.Y, Size.Z];
			Array.Copy(_blocks, newBlocks, Math.Min(newBlocks.Length, _blocks.Length));
			_blocks = newBlocks;
		}

		Point point = new Point();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> triangles = new List<int>();


		List<int> gridIndices = new List<int>();
		Point gridSize = Size + Point.One;

		// Find which vertices in the BlockMesh's grid are actually used in the BlockMesh
		for (point.X = 0; point.X < Size.X; point.X++) {
			for (point.Y = 0; point.Y < Size.Y; point.Y++) {
				for (point.Z = 0; point.Z < Size.Z; point.Z++) {
					
					// Calculate the vertex indices of the current block
					int sliceArea = gridSize.X * gridSize.Z;
					int vertexLevelStart = point.Y * sliceArea; // The top-left corner of the current slice of the grid above point.Z
					
					int topBackLeft = vertexLevelStart + point.X + point.Z * gridSize.X;
					int topBackRight = topBackLeft + 1;
					int topFrontLeft = vertexLevelStart + point.X + (point.Z + 1) * gridSize.X;
					int topFrontRight = topFrontLeft + 1;

					vertexLevelStart = (point.Y + 1) * sliceArea; // The top-left corner of the slice just below point.Z
					int bottomBackLeft = vertexLevelStart + point.X + point.Z * gridSize.X;
					int bottomBackRight = bottomBackLeft + 1;
					int bottomFrontLeft = vertexLevelStart + point.X + (point.Z + 1) * gridSize.X;
					int bottomFrontRight = bottomFrontLeft + 1;

					// Stick the above indices into an array so we can loop through the comparisons
					// It's important that blockIndices follows the same ordering as _TRIANGLE_ORDER and _BLOCK_VERTICES
					int[] blockIndices = new int[] {
						topFrontLeft, topFrontRight, topBackRight, topBackLeft, bottomFrontLeft, bottomFrontRight, bottomBackRight, bottomBackLeft
					};

					// Check neighboring blocks
					for (int side = 0; side < Point.Neighbors.Length; side++) {
						Point offset = point + Point.Neighbors[side];

						// If a neighboring block is free, add its side indices to list
						if (_blocks[point.X, point.Y, point.Z] != 0 &&
							(offset.X < 0 || offset.X >= Size.X || offset.Y < 0 || offset.Y >= Size.Y || offset.Z < 0 || offset.Z >= Size.Z ||
							 _blocks[offset.X, offset.Y, offset.Z] == 0)) {
							
							// Get the block indices for the given side and add them to the gridIndices list
							for (int i = 0; i < _BLOCK_SIDE_INDICES.GetLength(1); i++) {
								int sideIndex = _BLOCK_SIDE_INDICES[side, i];
								int blockIndex = blockIndices[sideIndex];

								gridIndices.Add(blockIndex);
							}
						}
					}
				}
			}
		}

		// Maps grid indices to mesh indices
		Dictionary<int, int> gridMeshIndices = new Dictionary<int, int>();

		// Generate vertices from the gridIndices
		for (int i = 0; i < gridIndices.Count; i++) {
			int gridIndex = gridIndices[i];
			int meshIndex;
			
			// If a vertex already exists for the gridIndex, use that instead of creating a new one
			if (gridMeshIndices.TryGetValue(gridIndex, out meshIndex)) {
				triangles.Add(meshIndex);
			// Otherwise, create a new vertex and cache the mapping in gridMeshIndices
			} else {
				// Turn the gridIndex into a vertex and add it to the vertex list
				int sliceArea = gridSize.X * gridSize.Z;
				int y = gridIndex / sliceArea;
				int z = (gridIndex - y * sliceArea) / gridSize.X;
				int x = (gridIndex - y * sliceArea - z * gridSize.X);

				Vector3 vertex = new Vector3((float)x, (float)y, (float)z);
				meshIndex = vertices.Count;
				vertices.Add(vertex);
				triangles.Add(meshIndex);

				// Map the gridIndex to the meshIndex
				gridMeshIndices[gridIndex] = meshIndex;
			}
		}

		_meshFilter.mesh.vertices = vertices.ToArray();
		_meshFilter.mesh.uv = uvs.ToArray();
		_meshFilter.mesh.triangles = triangles.ToArray();

		_meshFilter.mesh.RecalculateNormals();
	}

	#region MonoBehaviour
	private void Awake() {
		_blocks = new int[Size.X, Size.Y, Size.Z];
		_meshFilter = GetComponent<MeshFilter>();
	}
	#endregion MonoBehaviour
}
