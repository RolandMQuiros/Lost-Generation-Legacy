using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardMesh : MonoBehaviour {
    public GameObject CellPrefab;
	public Point CellSize;
	public BoardRef BoardRef;
    public BlockProperties[] BlockProperties;
    public Material BlockMaterial;
    private BlockMesh[,,] _cells;

    public BlockMesh GetCell(Point cellPosition)
    {
        return _cells[cellPosition.X, cellPosition.Y, cellPosition.Z];
    }

    public BlockMesh GetCellAt(Point blockPosition)
    {
        return _cells
        [
            blockPosition.X / CellSize.X,
            blockPosition.Y / CellSize.Y,
            blockPosition.Z / CellSize.Z
        ];
    }

	public void BuildCell(int cx, int cy, int cz)
    {
        Point start = new Point
        (
            cx * CellSize.X,
            cy * CellSize.Y,
            cz * CellSize.Z
        );
        Point blockCoords = new Point();

        BlockMesh cell = _cells[cx, cy, cz];
        if (cell == null)
        {
            GameObject cellObj = GameObject.Instantiate(CellPrefab, transform);
            cellObj.transform.localPosition = PointVector.ToVector(start);
            cellObj.name = "cell(" + cx + "," + cy + "," + cz + ")";

            MeshRenderer renderer = cellObj.GetComponent<MeshRenderer>();//cellObj.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            cell = cellObj.GetComponent<BlockMesh>();//AddComponent(typeof(BlockMesh)) as BlockMesh;
            cell.Resize(CellSize);
            cell.BlockProperties = BlockProperties;
            renderer.sharedMaterial = BlockMaterial;
        }

        for (blockCoords.X = -1; blockCoords.X <= CellSize.X; blockCoords.X++)
        {
            for (blockCoords.Y = -1; blockCoords.Y <= CellSize.Y; blockCoords.Y++)
            {
                for (blockCoords.Z = -1; blockCoords.Z <= CellSize.Z; blockCoords.Z++)
                {   
                    Point point = blockCoords + start;
                    if (BoardRef.Board.InBounds(point))
                    {
                        BoardBlock block = BoardRef.Board.GetBlock(point);
                        cell.SetBlock(blockCoords, block.BlockType);
                    }
                }   
            }
        }

        cell.Build();
	}

    private void OnBlocksChanged(Dictionary<BoardBlock, BoardBlock> blocksChanged)
    {
        HashSet<Point> cellsToUpdate = new HashSet<Point>();
        foreach (KeyValuePair<BoardBlock, BoardBlock> block in blocksChanged)
        {
            if (block.Key.BlockType != block.Value.BlockType)
            {
                Point cell = new Point
                (
                    block.Value.Point.X / CellSize.X,
                    block.Value.Point.Y / CellSize.Y,
                    block.Value.Point.Z / CellSize.Z
                );

                cellsToUpdate.Add(cell); 
            }
        }

        foreach (Point cell in cellsToUpdate)
        {
            BuildCell(cell.X, cell.Y, cell.Z);
        }
    }

	#region MonoBehaviour
    private void Start()
    {   
        _cells = new BlockMesh
        [
            Mathf.CeilToInt((float)BoardRef.Board.Size.X / CellSize.X),
            Mathf.CeilToInt((float)BoardRef.Board.Size.Y / CellSize.Y),
            Mathf.CeilToInt((float)BoardRef.Board.Size.Z / CellSize.Z)
        ];

        BoardRef.Board.BlocksChanged += OnBlocksChanged;

        for (int x = 0; x < _cells.GetLength(0); x++)
        {
            for (int y = 0; y < _cells.GetLength(1); y++)
            {
                for (int z = 0; z < _cells.GetLength(2); z++)
                {
                    BuildCell(x, y, z);
                }
            }
        }
    }
	#endregion MonoBehaviour
}
