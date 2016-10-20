using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour, ISkillController {
    public Point Origin {
        get { return _origin; }
        set { _origin = value; }
    }

    private RangedSkill _skill;
    private Point _origin;

    public void StartTargeting(Point origin, ISkill skill) {
        _skill = (RangedSkill)skill;
        _origin = origin;
    }

    public void OnCursorMove(Point point) {
        if (_skill != null && _skill.InRange(_origin, point)) {
            _skill.Target = point;
        }
    }

    public void OnTap(Point point) {
        if (_skill != null && _skill.InRange(_origin, point)) {
            _skill.Fire();
            _skill.Owner.ClearActiveSkill();
            _skill = null;
        }
    }
}
