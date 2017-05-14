using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LostGen;

[RequireComponent(typeof(MeshFilter))]
public class BlockMesh : MonoBehaviour
{
    public Point Size;
    private static readonly int[,] _BLOCK_SIDE_INDICES = new int[,]
	{
        {0, 1, 2, 3}, // Top
		{2, 1, 5, 6}, // Right
		{5, 4, 7, 6}, // Down
		{0, 3, 7, 4}, // Left
		{1, 0, 4, 5}, // Forward
		{3, 2, 6, 7}, // Backward
	};
	
	private static readonly int[] _TRI_ORDER = new int[] { 0, 1, 3, 1, 2, 3 };//{ 0, 1, 3, 3, 2, 1 };

    private static readonly Vector3[] _BLOCK_VERTICES = new Vector3[]
	{
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
    public void SetBlock(Point point, int blockType)
    {
        if (point.X >= 0 && point.X < Size.X && point.Y >= 0 && point.Y < Size.Y && point.Z >= 0 && point.Z < Size.Z)
        {
            _blocks[point.X, point.Y, point.Z] = blockType;
        }
        else
        {
            throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + Size + " of this BlockMesh");
        }
    }

    public void Resize(Point size, bool retainBlocks = false)
    {
        int[,,] newBlocks = new int[size.X, size.Y, size.Z];
        if (retainBlocks)
        {
            Array.Copy(_blocks, newBlocks, Math.Min(newBlocks.Length, _blocks.Length));
        }
        _blocks = newBlocks;
        Size = size;
    }

    public void Resize(bool retainBlocks = false)
    {
        Resize(Size, retainBlocks);
    }

    public void Build()
    {
		Point size = new Point(_blocks.GetLength(0), _blocks.GetLength(1), _blocks.GetLength(2));
		if (size != Size) {
			Resize(true);
		}

		Point point = new Point();
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (point.X = 0; point.X < Size.X; point.X++)
		{
			for (point.Y = 0; point.Y < Size.Y; point.Y++)
			{
				for (point.Z = 0; point.Z < Size.Z; point.Z++)
				{	
					Vector3 blockCenter = PointVector.ToVector(point);
					int blockType = _blocks[point.X, point.Y, point.Z];
					if (blockType != 0)
					{
						for (int side = 0; side < Point.Neighbors.Length; side++)
						{
							Point neighbor = point + Point.Neighbors[side];
							
							if (neighbor.X < 0 || neighbor.X >= Size.X ||
								neighbor.Y < 0 || neighbor.Y >= Size.Y ||
								neighbor.Z < 0 || neighbor.Z >= Size.Z ||
								_blocks[neighbor.X, neighbor.Y, neighbor.Z] != blockType)
							{
								int vertexCount = vertices.Count;
								for (int sideIndex = 0; sideIndex < _BLOCK_SIDE_INDICES.GetLength(1); sideIndex++)
								{
									int corner = _BLOCK_SIDE_INDICES[side, sideIndex];
									vertices.Add(blockCenter + _BLOCK_VERTICES[corner] * 0.5f);
								}

								for (int triIndex = 0; triIndex < _TRI_ORDER.Length; triIndex++)
								{
									triangles.Add(vertexCount + _TRI_ORDER[triIndex]);
								}
							}
						}
					}
				}
			}
		}

		_meshFilter.mesh.Clear();
		_meshFilter.mesh.vertices = vertices.ToArray();
		_meshFilter.mesh.triangles = triangles.ToArray();
		_meshFilter.mesh.uv = uvs.ToArray();
		_meshFilter.mesh.RecalculateNormals();
    }

    #region MonoBehaviour
    private void Awake()
    {
        _blocks = new int[Size.X, Size.Y, Size.Z];
        _meshFilter = GetComponent<MeshFilter>();
    }
    #endregion MonoBehaviour
}
