using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(BoardRef))]
public class BoardMesh : MonoBehaviour {
	public Point CellSize;
	private BoardRef _boardRef;
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

	public void BuildCell(Point cellPosition)
    {
        Point start = new Point
        (
            cellPosition.X * CellSize.X,
            cellPosition.Y * CellSize.Y,
            cellPosition.Z * CellSize.Z
        );
        Point blockCoords = new Point();

        BlockMesh cell = _cells[cellPosition.X, cellPosition.Y, cellPosition.Z];
        if (cell == null)
        {
            GameObject cellObj = new GameObject();
            cellObj.transform.parent = transform;
            cellObj.transform.localPosition = PointVector.ToVector(start);

            cell = cellObj.AddComponent(typeof(BlockMesh)) as BlockMesh;
        }

        for (blockCoords.X = 0; blockCoords.X < CellSize.X; blockCoords.X++)
        {
            for (blockCoords.Y = 0; blockCoords.X < CellSize.Y; blockCoords.Y++)
            {
                for (blockCoords.Z = 0; blockCoords.Z < CellSize.Z; blockCoords.Z++)
                {
                    BoardBlock block = _boardRef.Board.GetBlock(blockCoords + start);
                    cell.SetBlock(blockCoords, block.BlockType);
                }   
            }
        }

        cell.Build();
	}

	#region MonoBehaviour
    private void Awake()
    {   
        _boardRef = GetComponent<BoardRef>();
        _cells = new BlockMesh
        [
            _boardRef.Board.Size.X / CellSize.X,
            _boardRef.Board.Size.Y / CellSize.Y,
            _boardRef.Board.Size.Z / CellSize.Z
        ];  
    }
	#endregion MonoBehaviour
}
