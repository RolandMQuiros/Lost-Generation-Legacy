using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour
{
    public RangedSkill Skill
    {
        get { return _skill; }
        set
        {
            if (_skill != value)
            {
                _skill.AreaOfEffectChanged -= UpdateAreaOfEffect;
				_skill = value;
				_skill.AreaOfEffectChanged += UpdateAreaOfEffect;
            }

			SetBlocks(_rangeMesh, _skill.GetRange(), 1);
			_rangeMesh.Build();
        }
    }
    private RangedSkill _skill;
    [SerializeField]private BlockMesh _areaOfEffectMesh;
    [SerializeField]private BlockMesh _rangeMesh;
    [SerializeField]private BlockMesh _pathMesh;

	public void SetSkill(ISkill skill)
	{
		RangedSkill ranged = skill as RangedSkill;
		if (ranged != null)
		{
			SetSkill(ranged);
		}
	}

	public void SetSkill(RangedSkill skill)
	{
		if (skill != null)
		{
			_skill = skill;

			_areaOfEffectMesh.gameObject.SetActive(true);
			_rangeMesh.gameObject.SetActive(true);
			_pathMesh.gameObject.SetActive(true);

			SetBlocks(_rangeMesh, _skill.GetRange(), 1);
			_rangeMesh.Build();
		}
	}

	public void ClearSkill()
	{
		if (_skill != null)
		{
			_skill = null;
			
			_areaOfEffectMesh.gameObject.SetActive(false);
			_rangeMesh.gameObject.SetActive(false);
			_pathMesh.gameObject.SetActive(false);
		}
	}

	public void SetTarget(Point point)
	{
		if (_skill != null)
		{
			if (_skill.SetTarget(point))
			{
				UpdateAreaOfEffect();
			}
		}
	}

    private void UpdateAreaOfEffect()
    {
		SetBlocks(_areaOfEffectMesh, _skill.GetAreaOfEffect(), 1);
		SetBlocks(_pathMesh, _skill.GetPath(), 1);

		_areaOfEffectMesh.Build();
		_pathMesh.Build();
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

	private void SetBlocks(BlockMesh mesh, IEnumerable<Point> points, byte blockType)
	{
		if (!ResizeFromPoints(mesh, points))
		{
			mesh.Clear();
		}

		Point lower = Point.LowerBound(points);
		foreach (Point point in points)
		{
			mesh.SetBlock(point - lower, blockType);
			mesh.transform.position = PointVector.ToVector(lower);
		}
	}
}