using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour, ISkillController {
    private RangedSkill _skill;

    public void StartTargeting(ISkill skill) {
        _skill = (RangedSkill)skill;
    }

    public void OnCursorMove(Point point) {
        if (_skill != null && _skill.InRange(point)) {
            _skill.SetTarget(point);
        }
    }

    public void OnTap(Point point) {
        if (_skill != null && _skill.InRange(point)) {
            _skill.Owner.FireActiveSkill();
            _skill = null;
        }
    }
}
