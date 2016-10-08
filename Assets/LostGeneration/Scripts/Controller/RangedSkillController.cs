using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour, ISkillController {
    public ISkill Skill {
        get { return _skill; }
        set {
            _skill = (RangedSkill)value;
        }
    }

    public BoardCursor Cursor;
    public bool DebugIsTargeting;

    private RangedSkill _skill;
    private bool _isTargeting = false;
    private bool _initialTargeting = false;

    public void StartTargeting() {
        _isTargeting = true;
        _initialTargeting = true;
        _skill.IsReadyToFire = false;
    }

    public void CancelTargeting() {
        _isTargeting = false;
        _initialTargeting = false;
        _skill.IsReadyToFire = false;
    }

    private void Update() {
        if (_isTargeting) {
            if (_skill.InRange(Cursor.BoardPoint)) {
                if (Cursor.TapDown) {
                    _isTargeting = false;
                    _initialTargeting = false;
                    _skill.IsReadyToFire = true;
                }

                if (_initialTargeting || _skill.Target != Cursor.BoardPoint) {
                    _skill.Target = Cursor.BoardPoint;
                    _initialTargeting = false;
                }
            }
        }

        DebugIsTargeting = _isTargeting;
    }
}
