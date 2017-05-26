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

	private static readonly int[] _TRI_ORDER = new int[] { 0, 1, 3, 1, 2, 3 };

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

	/* Holds the neighbor direction vectors used when checking surrounding blocks for
	   autotiling purposes. Each index represents a side, with the subindices holding
	   the direction vectors in clockwise order, starting from the top. */
	private static readonly Point[,] _BLOCK_SIDE_NEIGHBORS = new Point[,]
	{
		{ Point.Forward, Point.Right   , Point.Backward, Point.Left     }, // Top
		{ Point.Up     , Point.Forward , Point.Down    , Point.Backward }, // Right
		{ Point.Forward, Point.Left    , Point.Backward, Point.Right    }, // Down
		{ Point.Up	   , Point.Backward, Point.Down    , Point.Forward  }, // Left
		{ Point.Up     , Point.Left    , Point.Down    , Point.Right    }, // Forward
		{ Point.Up     , Point.Right   , Point.Down    , Point.Left     }  // Backward
	};

	private const int _BLOCK_SIDE_TILE_COUNT = 16;
	public BlockProperties[] BlockProperties;
    private int[,,] _blocks;
    private MeshFilter _meshFilter;

	public bool InBounds(Point point)
	{
		return point.X >= 0 && point.X < Size.X && point.Y >= 0 && point.Y < Size.Y && point.Z >= 0 && point.Z < Size.Z;
	}
    public void SetBlock(Point point, int blockType)
    {
        if (point.X >= -1 && point.X <= Size.X &&
			point.Y >= -1 && point.Y <= Size.Y &&
			point.Z >= -1 && point.Z <= Size.Z)
        {
            _blocks[point.X + 1, point.Y + 1, point.Z + 1] = blockType;
        }
        else
        {
            throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + Size + " of this BlockMesh");
        }
    }

    public void Resize(Point size, bool retainBlocks = false)
    {
        int[,,] newBlocks = new int[size.X + 2, size.Y + 2, size.Z + 2];
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
		Point size = new Point(_blocks.GetLength(0) - 2, _blocks.GetLength(1) - 2, _blocks.GetLength(2) - 2);
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
					int blockType = _blocks[point.X + 1, point.Y + 1, point.Z + 1];
					if (blockType != 0 && BlockProperties.Length > blockType && BlockProperties[blockType] != null)
					{
						for (int side = 0; side < Point.Neighbors.Length; side++)
						{
							Point neighbor = point + Point.Neighbors[side];
							
							if (BlockProperties[blockType].SideSprites[side] != null &&
								(!InBounds(neighbor) || _blocks[neighbor.X + 1, neighbor.Y + 1, neighbor.Z + 1] != blockType))
							{
								// Get the vertex count before adding the new ones, which will act as the offset
								// for the triangle windings
								int vertexCount = vertices.Count;

								// Push the vertices for the new quad onto the vertex list
								for (int sideIndex = 0; sideIndex < _BLOCK_SIDE_INDICES.GetLength(1); sideIndex++)
								{
									int corner = _BLOCK_SIDE_INDICES[side, sideIndex];
									vertices.Add(blockCenter + _BLOCK_VERTICES[corner] * 0.5f);
								}

								// Push the triangle windings for the new quad onto the triangle list
								for (int triIndex = 0; triIndex < _TRI_ORDER.Length; triIndex++)
								{
									triangles.Add(vertexCount + _TRI_ORDER[triIndex]);
								}

								// Create UVs from autotiles
								if (blockType != 0 && BlockProperties.Length > blockType)
								{	
									// Get the index of the tile
									int tileAdjacency = 0;
									for (int sideNeighborIdx = 0; sideNeighborIdx < _BLOCK_SIDE_NEIGHBORS.GetLength(1); sideNeighborIdx++)
									{
										Point sideNeighbor = point + Point.One + _BLOCK_SIDE_NEIGHBORS[side, sideNeighborIdx];
										if (sideNeighbor.X >= 0 && sideNeighbor.X < _blocks.GetLength(0) &&
											sideNeighbor.Y >= 0 && sideNeighbor.Y < _blocks.GetLength(1) &&
											sideNeighbor.Z >= 0 && sideNeighbor.Z < _blocks.GetLength(2) &&
										 	_blocks[sideNeighbor.X, sideNeighbor.Y, sideNeighbor.Z] == blockType)
										{
											tileAdjacency |= (1 << sideNeighborIdx);
										}
									}

									// Apply that tile's UV coordinates to the quad
									Sprite sprite = BlockProperties[blockType].SideSprites[side];
									Vector2 boundsMin = new Vector2(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height);
									Vector2 boundsMax = new Vector2(sprite.rect.size.x / sprite.texture.width, sprite.rect.size.y / sprite.texture.height);

									float tileWidth = (boundsMax.x - boundsMin.x) / _BLOCK_SIDE_TILE_COUNT;
									float tileHeight = (boundsMax.y - boundsMin. y);
									float tileX = boundsMin.x + tileWidth * tileAdjacency;
									
									uvs.Add(boundsMin + new Vector2(tileX, boundsMax.y));
									uvs.Add(boundsMin + new Vector2(tileX + tileWidth, boundsMax.y));
									uvs.Add(boundsMin + new Vector2(tileX + tileWidth, boundsMin.y));
									uvs.Add(boundsMin + new Vector2(tileX, boundsMin.y));
								}
								else 
								{
									for (int u = 0; u < 4; u++) { uvs.Add(Vector2.zero); }
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
        _blocks = new int[Size.X + 2, Size.Y + 2, Size.Z + 2];
        _meshFilter = GetComponent<MeshFilter>();
    }
    #endregion MonoBehaviour
}
