using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;

namespace LostGen.Display {
    [RequireComponent(typeof(MeshFilter))]
    public class BlockMesh : MonoBehaviour
    {
        public Point Size { get { return _size; } }
        [SerializeField]private Point _size;
        
        private static readonly Vector3[,] _SIDE_VECTORS = new Vector3[,] {
            { Vector3.forward, Vector3.right   }, // Top
            { Vector3.up     , Vector3.forward }, // Right
            { Vector3.forward, Vector3.left    }, // Down
            { Vector3.up     , Vector3.back    }, // Left
            { Vector3.up     , Vector3.left    }, // Forward
            { Vector3.up     , Vector3.right   }  // Backward
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

        private const byte _BLOCK_SIDE_TILE_COUNT = 16;
        public BlockProperties[] BlockProperties;
        private byte[,,] _blocks;
        private MeshFilter _meshFilter;

        public bool InBounds(Point point)
        {
            return point.X >= 0 && point.X < _size.X && point.Y >= 0 && point.Y < _size.Y && point.Z >= 0 && point.Z < _size.Z;
        }

        public void SetBlock(Point point, byte blockType)
        {
            if (point.X >= -1 && point.X <= _size.X &&
                point.Y >= -1 && point.Y <= _size.Y &&
                point.Z >= -1 && point.Z <= _size.Z)
            {
                _blocks[point.X + 1, point.Y + 1, point.Z + 1] = blockType;
            }
            else
            {
                throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + _size + " of this BlockMesh");
            }
        }

        public byte GetBlock(Point point) {
            return _blocks[point.X + 1, point.Y + 1, point.Z + 1];
        }

        public byte GetBlock(int x, int y, int z) {
            return _blocks[x + 1, y + 1, z + 1];
        }

        public void Resize(Point size, bool retainBlocks = false)
        {
            byte[,,] newBlocks = new byte[size.X + 2, size.Y + 2, size.Z + 2];
            if (retainBlocks)
            {
                Array.Copy(_blocks, newBlocks, Math.Min(newBlocks.Length, _blocks.Length));
            }
            _blocks = newBlocks;
            _size = size;
        }

        public void Clear()
        {
            Array.Clear(_blocks, 0, _blocks.Length);
        }

        public void Build()
        {
            Point size = new Point(_blocks.GetLength(0) - 2, _blocks.GetLength(1) - 2, _blocks.GetLength(2) - 2);
            if (size != _size) {
                Resize(size, true);
            }

            Point point = new Point();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            try {

                for (point.X = 0; point.X < _size.X; point.X++)
                {
                    for (point.Y = 0; point.Y < _size.Y; point.Y++)
                    {
                        for (point.Z = 0; point.Z < _size.Z; point.Z++)
                        {	
                            Vector3 blockCenter = PointVector.ToVector(point);
                            int blockType = GetBlock(point);
                            if (blockType != 0 && BlockProperties.Length > blockType && BlockProperties[blockType] != null)
                            {
                                for (int side = 0; side < Point.Neighbors.Length; side++)
                                {
                                    Point neighbor = point + Point.Neighbors[side];
                                    AutoTile autoTile = BlockProperties[blockType].SideTiles[side];
                                    if (autoTile != null && GetBlock(neighbor) != blockType)
                                    {                    
                                        // Get adjacency bitstring for the AutoTile texture
                                        byte tileAdjacency = 0;
                                        int adjacencyIdx = 0;
                                        for (int sideNeighborIdx = 0; sideNeighborIdx < _BLOCK_SIDE_NEIGHBORS.GetLength(1); sideNeighborIdx++)
                                        {   
                                            // Check each neighbor, and the block in front of that neighbor
                                            // If the block in front of the neighbor is occupied, then don't include the neighbor
                                            // in the adjacency string.
                                            // This lets the texture reset its tiling against edges.
                                            Point sideNeighbor = point + _BLOCK_SIDE_NEIGHBORS[side, sideNeighborIdx];
                                            if (InArray(sideNeighbor) && GetBlock(sideNeighbor) == blockType
                                                                    && GetBlock(sideNeighbor + Point.Neighbors[side]) != blockType) {
                                                tileAdjacency |= (byte)(1 << adjacencyIdx);
                                            }
                                            adjacencyIdx++;

                                            // Check the corners
                                            int cornerIdx = (sideNeighborIdx + 1) % _BLOCK_SIDE_NEIGHBORS.GetLength(1);
                                            sideNeighbor = point + _BLOCK_SIDE_NEIGHBORS[side, sideNeighborIdx] 
                                                                + _BLOCK_SIDE_NEIGHBORS[side, cornerIdx];
                                            if (InArray(sideNeighbor) && GetBlock(sideNeighbor) == blockType
                                                                    && GetBlock(sideNeighbor + Point.Neighbors[side]) != blockType) {
                                                tileAdjacency |= (byte)(1 << adjacencyIdx);
                                            }
                                            adjacencyIdx++;
                                        }

                                        Vector3 sideCenter = blockCenter + 0.5f * PointVector.ToVector(Point.Neighbors[side]);

                                        // Generate the side mesh
                                        autoTile.AddTile(
                                            sideCenter,
                                            _SIDE_VECTORS[side, 0],
                                            _SIDE_VECTORS[side, 1],
                                            tileAdjacency,
                                            vertices,
                                            triangles,
                                            uvs
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Debug.Log("BlockMesh: Error at block " + point + ": " + e.Message);
                throw;
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

        private bool InArray(Point point) {
            return point.X >= 0 && point.X < _blocks.GetLength(0) &&
                point.Y >= 0 && point.Y < _blocks.GetLength(1) &&
                point.Z >= 0 && point.Z < _blocks.GetLength(2);
        }

        #region MonoBehaviour
        private void Awake()
        {
            _blocks = new byte[_size.X + 2, _size.Y + 2, _size.Z + 2];
            _meshFilter = GetComponent<MeshFilter>();
        }

        // private void OnEnable() {
        //     for (int i = 0; i < BlockProperties.Length; i++) {
        //         if (BlockProperties[i] != null) {
        //             BlockProperties[i].Setup();
        //         }
        //     }
        // }
        #endregion MonoBehaviour
    }
}