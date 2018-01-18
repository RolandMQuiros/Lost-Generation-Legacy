using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
    [RequireComponent(typeof(MeshFilter))]
    public class BlockMesh : MonoBehaviour {
        public Point Size { get { return _size; } }
        [SerializeField]private Point _size;
        
        /* The up and right vectors of each block face, used to determine block face orientation. */
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

        public bool InBounds(Point point) {
            return Point.WithinBox(point, Point.Zero, _size);
        }

        /// <summary>
        /// Sets the block data at the given Point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="blockType"></param>
        public void SetBlock(Point point, byte blockType) {            
            if (Point.WithinBox(point, Point.Zero - Point.One, _size + Point.One)) {
                _blocks[point.X + 1, point.Y + 1, point.Z + 1] = blockType;
            }
            else {
                throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + _size + " of this BlockMesh");
            }
        }

        /// <summary>
        /// Retrieves the Block data at the given <see cref="Point"/>
        /// </summary>
        /// <param name="Point">The Point to check</Point>
        /// <returns>The block data value at the given Point</returns>
        public byte GetBlock(Point point) {
            if (Point.WithinBox(point, Point.Zero - Point.One, _size + Point.One)) {
                return _blocks[point.X + 1, point.Y + 1, point.Z + 1];
            } else {
                throw new ArgumentOutOfRangeException("point", "Given point " + point + " is outside the bounds " + _size + " of this BlockMesh");
            }
        }

        /// <summary>
        /// Retrieves the Block data at the given point
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <param name="z">Z-coordinate</param>
        /// <returns>The block data value at the given point</returns>
        public byte GetBlock(int x, int y, int z) {
            return GetBlock(new Point(x, y, z));
        }

        /// <summary>
        /// Resizes the BlockMesh.
        /// </summary>
        /// <param name="size">The new size</param>
        /// <param name="retainBlocks">`true` to retain the original block data. If the size is smaller than the
        /// original, the data is trimmed.</param>
        public void Resize(Point size, bool retainBlocks = false) {
            byte[,,] newBlocks = new byte[size.X + 2, size.Y + 2, size.Z + 2];
            if (retainBlocks) {
                Array.Copy(_blocks, newBlocks, Math.Min(newBlocks.Length, _blocks.Length));
            }
            _blocks = newBlocks;
            _size = size;
        }

        /// <summary>
        /// Clears this BlockMesh's block data
        /// </summary>
        public void Clear() {
            Array.Clear(_blocks, 0, _blocks.Length);
        }

        /// <summary>
        /// Build this BlockMesh's geometry
        /// </summary>
        public void Build() {
            Point size = new Point(_blocks.GetLength(0) - 2, _blocks.GetLength(1) - 2, _blocks.GetLength(2) - 2);
            if (size != _size) {
                Resize(size, true);
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Start building the block sides
            Point.ForEachXYZ(Point.Zero, _size, (Point point) => {
                Vector3 blockCenter = PointVector.ToVector(point);
                int blockType = GetBlock(point);
                // Only create geometry for blocks that have a nonzero type, and have a BlockProperty associated to its type
                if (blockType != 0 && BlockProperties.Length > blockType && BlockProperties[blockType] != null) {

                    // For each side of the block, create an 8-bit adjacency bitstring, with each bit represents that side's neighbor
                    // blocks, in clockwise order from LSB to MSB. If the neighbor has the same block type, the bit is 1, 0 otherwise.
                    // The AutoTile then uses that adjacency bitstring to determine how to create the side's geometry.
                    for (int side = 0; side < Point.Neighbors.Length; side++) {
                        Point sideForward = Point.Neighbors[side];
                        Point neighbor = point + sideForward;
                        AutoTile autoTile = BlockProperties[blockType].SideTiles[side];
                        if (autoTile != null && GetBlock(neighbor) != blockType) {                    
                            // Get adjacency bitstring for the AutoTile texture
                            byte tileAdjacency = 0;
                            int adjacencyIdx = 0;
                            for (int sideNeighborIdx = 0; sideNeighborIdx < _BLOCK_SIDE_NEIGHBORS.GetLength(1); sideNeighborIdx++) {
                                // Check each neighbor, and the block in front of that neighbor
                                // If the block in front of the neighbor is occupied, then don't include the neighbor
                                // in the adjacency string.
                                // This lets the texture reset its tiling against edges.
                                Point sideNeighbor = point + _BLOCK_SIDE_NEIGHBORS[side, sideNeighborIdx];

                                byte neighborType = GetBlock(sideNeighbor);
                                byte neighborFrontType = GetBlock(sideNeighbor + sideForward);
                                if (InArray(sideNeighbor) && neighborType == blockType
                                                          && neighborFrontType != blockType) {
                                    tileAdjacency |= (byte)(1 << adjacencyIdx);
                                }
                                adjacencyIdx++;

                                // Check the corners
                                int cornerIdx = (sideNeighborIdx + 1) % _BLOCK_SIDE_NEIGHBORS.GetLength(1);
                                sideNeighbor = point + _BLOCK_SIDE_NEIGHBORS[side, sideNeighborIdx] 
                                                     + _BLOCK_SIDE_NEIGHBORS[side, cornerIdx];
                                neighborType = GetBlock(sideNeighbor);
                                neighborFrontType = GetBlock(sideNeighbor + sideForward);
                                if (InArray(sideNeighbor) && neighborType == blockType
                                                          && neighborFrontType != blockType) {
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
            });

            // Assign the geometry to the MeshFilter
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

        /// <summary>
        /// Returns whether or not the given <see cref="Point"> is in the internal array.
        /// </summary>
        /// <remarks>
        /// <p>This differs from InBounds as it checks against the actual size of the internal array, which is two tiles larger
        /// than the bounds in every direction. That extra padding is used to hold the blocks of adjacent cells, so that the
        /// tiling is contiguous between them.
        /// </p>
        /// <p>Be careful with the off-by-one errors in this function. You've wasted hours tracking them down.</p>
        /// </remarks>
        /// <param name="point">The Point to check</param>
        /// <returns>True if the point is within the internal block array</returns>
        private bool InArray(Point point) {
            point += Point.One;

            return point.X >= 0 && point.X <= _size.X + 1 &&
                   point.Y >= 0 && point.Y <= _size.Y + 1 &&
                   point.Z >= 0 && point.Z <= _size.Z + 1;
        }

        #region MonoBehaviour
        private void Awake() {
            _blocks = new byte[_size.X + 2, _size.Y + 2, _size.Z + 2];
            _meshFilter = GetComponent<MeshFilter>();
        }
        #endregion MonoBehaviour
    }
}