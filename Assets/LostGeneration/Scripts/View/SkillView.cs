using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class SkillView : MonoBehaviour
{
	private const byte _AOE_TYPE = 1;
	private const byte _RANGE_TYPE = 2;
	private const byte _PATH_TYPE = 3;
    [SerializeField]private BlockMesh _selectorMesh;

	public void SetAreaOfEffect(IEnumerable<Point> areaOfEffect)
	{
		SetBlocks(areaOfEffect, _AOE_TYPE);
	}

	public void SetRange(IEnumerable<Point> range)
	{
		SetBlocks(range, _RANGE_TYPE);
	}

	public void SetPath(IEnumerable<Point> path)
	{
		SetBlocks(path, _PATH_TYPE);
	}

	private void ResizeFromPoints(IEnumerable<Point> points)
	{
		Point size = Point.UpperBound(points) - Point.LowerBound(points);
		if (_selectorMesh.Size.X < size.X ||
			_selectorMesh.Size.Y < size.Y ||
			_selectorMesh.Size.Z < size.Z)
		{
			Point newSize = new Point
			(
				Math.Max(_selectorMesh.Size.X, size.X),
				Math.Max(_selectorMesh.Size.Y, size.Y),
				Math.Max(_selectorMesh.Size.Z, size.Z)
			);

			_selectorMesh.Resize(newSize, true);
		}
	}

	private void SetBlocks(IEnumerable<Point> points, byte blockType)
	{
		ResizeFromPoints(points);
		foreach (Point point in points)
		{
			_selectorMesh.SetBlock(point, blockType);
		}
	}

    private void BuildMesh(BlockMesh mesh, IEnumerable<Point> points)
	{
		mesh.Build();
	}
}