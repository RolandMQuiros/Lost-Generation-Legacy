using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class SkillView : MonoBehaviour
{
	private const byte _AOE_TYPE = 1;
	private const byte _RANGE_TYPE = 2;
	private const byte _PATH_TYPE = 3;
    [SerializeField]private BlockMesh _areaOfEffectMesh;
	[SerializeField]private BlockMesh _rangeMesh;
	[SerializeField]private BlockMesh _pathMesh;

	private AreaOfEffectSkill _skill;

	public void SetSkill(ISkill skill)
	{
		AreaOfEffectSkill aoe = skill as AreaOfEffectSkill;
		if (aoe != null)
		{
			if (_skill != aoe)
			{
				_skill.AreaOfEffectChanged -= OnAreaOfEffectChange;
			}
			_skill = aoe;

			RangedSkill ranged = skill as RangedSkill;
			if (ranged != null)
			{
				SetBlocks(_rangeMesh, ranged.GetRange(), 1);
				_rangeMesh.Build();
			}
		}
	}

	private void OnAreaOfEffectChange()
	{
		SetBlocks(_areaOfEffectMesh, _skill.GetAreaOfEffect(), 1);

		RangedSkill ranged = _skill as RangedSkill;
		if (ranged != null)
		{
			SetBlocks(_pathMesh, ranged.GetRange(), 1);
		}

		_areaOfEffectMesh.Build();
		_rangeMesh.Build();
	}

	private bool ResizeFromPoints(BlockMesh mesh, IEnumerable<Point> points)
	{
		bool wasResized = false;
		Point size = Point.UpperBound(points) - Point.LowerBound(points);
		if (mesh.Size.X < size.X ||
			mesh.Size.Y < size.Y ||
			mesh.Size.Z < size.Z)
		{
			Point newSize = new Point
			(
				Math.Max(mesh.Size.X, size.X),
				Math.Max(mesh.Size.Y, size.Y),
				Math.Max(mesh.Size.Z, size.Z)
			);

			mesh.Resize(newSize, true);
			wasResized = true;
		}

		return wasResized;
	}

	private void SetBlocks(BlockMesh mesh, IEnumerable<Point> points, byte blockType)
	{
		ResizeFromPoints(mesh, points);
		foreach (Point point in points)
		{
			mesh.SetBlock(point, blockType);
		}
	}

}