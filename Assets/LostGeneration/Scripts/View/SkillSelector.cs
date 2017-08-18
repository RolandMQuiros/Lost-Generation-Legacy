using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class SkillSelector : MonoBehaviour {
    
	private ISkill _skill;
	private AreaOfEffectSkill _aoe;
	private RangedSkill _ranged;
	private DirectionalSkill _directional;

    [SerializeField]private BlockMesh _areaOfEffectMesh;
    [SerializeField]private BlockMesh _rangeMesh;
    [SerializeField]private BlockMesh _pathMesh;

	public ISkill Skill {
        get { return _skill; }
        set { SetSkill(value); }
    }

	public void SetSkill(ISkill skill)
	{
		if (skill == null)
		{
			_skill = null;
			_aoe = null;
			_ranged = null;
			_directional = null;
			
			_areaOfEffectMesh.gameObject.SetActive(false);
			_rangeMesh.gameObject.SetActive(false);
			_pathMesh.gameObject.SetActive(false);
		}
		else if (_skill != skill)
		{	
			_areaOfEffectMesh.gameObject.SetActive(false);
			_rangeMesh.gameObject.SetActive(false);
			_pathMesh.gameObject.SetActive(false);

			_skill = skill;
			_aoe = _skill as AreaOfEffectSkill;
			_ranged = _skill as RangedSkill;
			_directional = _skill as DirectionalSkill;
			
			if (_ranged != null)
			{
				SetBlocks(_rangeMesh, _ranged.GetRange(), 1);
				_rangeMesh.Build();
				_rangeMesh.gameObject.SetActive(true);
			}
		}
	}

	public void ClearSkill()
	{
		if (_skill != null)
		{
			_skill = null;
			_aoe = null;
			_ranged = null;
			_directional = null;
			
			_areaOfEffectMesh.gameObject.SetActive(false);
			_rangeMesh.gameObject.SetActive(false);
			_pathMesh.gameObject.SetActive(false);
		}
	}

	public void SetTarget(Point point)
	{
		if (_ranged != null)
		{
			if (_ranged.SetTarget(point))
			{
				UpdateAreaOfEffect();
			}
		}
		if (_directional != null)
		{
			Point delta = point - _directional.Pawn.Position;
			Point absDelta = Point.Abs(delta);

			CardinalDirection direction = _directional.Direction;
			if (absDelta.X > absDelta.Y && absDelta.X > absDelta.Z)
			{
				if (delta.X > 0) { direction = CardinalDirection.East; }
				else if (delta.X < 0) { direction = CardinalDirection.West; }
			}
			// else if (absDelta.Y > absDelta.X && absDelta.Y > absDelta.Z)
			// {
			// 	if (delta.Y > 0) { direction = CardinalDirection.East; }
			// 	else if (delta.Y < 0) { direction = CardinalDirection.West; }
			// }
			else if (absDelta.Z > absDelta.X && absDelta.Z > absDelta.Y)
			{
				if (delta.Z > 0) { direction = CardinalDirection.North; }
				else if (delta.Z < 0) { direction = CardinalDirection.South; }
			}
			
			if (direction != _directional.Direction)
			{
				_directional.SetDirection(direction);
				UpdateAreaOfEffect();
			}
		}
	}

    private void UpdateAreaOfEffect() {
		if (_aoe != null)
		{
			SetBlocks(_areaOfEffectMesh, _aoe.GetAreaOfEffect(), 1);
			_areaOfEffectMesh.Build();
			_areaOfEffectMesh.gameObject.SetActive(true);
		}

		if (_ranged != null)
		{
			SetBlocks(_pathMesh, _ranged.GetPath(), 1);
			_pathMesh.Build();
			_pathMesh.gameObject.SetActive(true);
		}
    }

    private bool ResizeFromPoints(BlockMesh mesh, IEnumerable<Point> points)
	{
		bool wasResized = false;
		Point upper = Point.UpperBound(points);
		Point lower = Point.LowerBound(points);
		Point size = new Point
		(
			Math.Abs(upper.X - lower.X) + 1,
			Math.Abs(upper.Y - lower.Y) + 1,
			Math.Abs(upper.Z - lower.Z) + 1
		);

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

			mesh.Resize(newSize);
			wasResized = true;
		}

		return wasResized;
	}

	private void SetBlocks(BlockMesh mesh, IEnumerable<Point> points, byte blockType) {
		if (!ResizeFromPoints(mesh, points)) {
			mesh.Clear();
		}

		Point lower = Point.LowerBound(points);
		foreach (Point point in points) {
			mesh.SetBlock(point - lower, blockType);
			mesh.transform.position = PointVector.ToVector(lower);
		}
	}
}