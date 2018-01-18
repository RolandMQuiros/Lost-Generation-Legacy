using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
    public class BoardMesh : MonoBehaviour {
        [SerializeField]private GameObject _cellPrefab;
        [SerializeField]private Point _cellSize;
        [SerializeField]private BoardRef _boardRef;
        [SerializeField]private BlockProperties[] _blockProperties;
        [SerializeField]private Material _blockMaterial;
        private BlockMesh[,,] _cells;

        public BlockMesh GetCell(Point cellPosition) {
            return _cells[cellPosition.X, cellPosition.Y, cellPosition.Z];
        }

        public BlockMesh GetCellAt(Point blockPosition) {
            return _cells
            [
                blockPosition.X / _cellSize.X,
                blockPosition.Y / _cellSize.Y,
                blockPosition.Z / _cellSize.Z
            ];
        }

        public void BuildCell(int cx, int cy, int cz) {
            Point start = new Point
            (
                cx * _cellSize.X,
                cy * _cellSize.Y,
                cz * _cellSize.Z
            );

            BlockMesh cell = _cells[cx, cy, cz];
            GameObject cellObj;

            // If no cell exists, build one
            if (cell == null) {
                Vector3 lower = PointVector.ToVector(start);
                cellObj = GameObject.Instantiate(_cellPrefab, transform);
                cellObj.transform.localPosition = lower;
                cellObj.name = "cell(" + cx + "," + cy + "," + cz + ")";

                MeshRenderer renderer = cellObj.GetComponent<MeshRenderer>();
                cell = cellObj.GetComponent<BlockMesh>();
                cell.Resize(_cellSize);
                cell.BlockProperties = _blockProperties;
                renderer.sharedMaterial = _blockMaterial;
            }
            // Otherwise, resize the existing one
            else {
                cellObj = cell.gameObject;
                cell.Resize(_cellSize);
            }

            // Set the cell's block data
            Point.ForEachXYZ(Point.Zero - Point.One, _cellSize + Point.One, (Point cellPoint) => {
                Point blockPoint = cellPoint + start;
                if (_boardRef.Board.Blocks.InBounds(blockPoint)) {
                    BoardBlock block = _boardRef.Board.Blocks.At(blockPoint);
                    cell.SetBlock(cellPoint, block.BlockType);
                }
            });

            // Create the cell's mesh
            cell.Build();

            // Set the collider's bounding box to that of the newly-built mesh
            MeshFilter meshFilter = cellObj.GetComponent<MeshFilter>();
            BoxCollider collider = cellObj.GetComponent<BoxCollider>();
            collider.center = meshFilter.mesh.bounds.center;
            collider.size = meshFilter.mesh.bounds.size;
        }

        private void OnBlocksChanged(Dictionary<BoardBlock, BoardBlock> blocksChanged) {
            HashSet<Point> cellsToUpdate = new HashSet<Point>();
            foreach (KeyValuePair<BoardBlock, BoardBlock> block in blocksChanged) {
                if (block.Key.BlockType != block.Value.BlockType) {
                    Point cell = new Point
                    (
                        block.Value.Point.X / _cellSize.X,
                        block.Value.Point.Y / _cellSize.Y,
                        block.Value.Point.Z / _cellSize.Z
                    );

                    cellsToUpdate.Add(cell);
                }
            }

            foreach (Point cell in cellsToUpdate) {
                BuildCell(cell.X, cell.Y, cell.Z);
            }
        }

        #region MonoBehaviour
        private void Start() {
            _cells = new BlockMesh
            [
                Mathf.CeilToInt((float)_boardRef.Board.Blocks.Size.X / _cellSize.X),
                Mathf.CeilToInt((float)_boardRef.Board.Blocks.Size.Y / _cellSize.Y),
                Mathf.CeilToInt((float)_boardRef.Board.Blocks.Size.Z / _cellSize.Z)
            ];

            _boardRef.Board.Blocks.Changed += OnBlocksChanged;

            for (int x = 0; x < _cells.GetLength(0); x++) {
                for (int y = 0; y < _cells.GetLength(1); y++) {
                    for (int z = 0; z < _cells.GetLength(2); z++) {
                        BuildCell(x, y, z);
                    }
                }
            }
        }
        #endregion MonoBehaviour
    }
}