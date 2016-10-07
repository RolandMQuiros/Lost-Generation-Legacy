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
    public BoardGridField BoardGridField;
    public Sprite RangeSprite;
    public Sprite AreaOfEffectSprite;
    public Sprite PathSprite;

    public bool DebugIsTargeting;

    private RangedSkill _skill;
    private bool _isTargeting = false;
    private bool _initialTargeting = false;

    public void StartTargeting() {
        _isTargeting = true;
        _initialTargeting = true;
    }

    public void CancelTargeting() {
        _isTargeting = false;
        _initialTargeting = false;
    }

    private void Update() {
        if (_isTargeting) {
            if (_skill.InRange(Cursor.BoardPoint)) {
                if (Cursor.TapDown) {
                    _isTargeting = false;
                    _initialTargeting = false;
                    BoardGridField.ClearPoints();
                }

                if (_initialTargeting || _skill.Target != Cursor.BoardPoint) {
                    _skill.Target = Cursor.BoardPoint;
                    IEnumerable<Point> range = _skill.GetRange();
                    IEnumerable<Point> areaOfEffect = _skill.GetAreaOfEffect();
                    IEnumerable<Point> path = _skill.GetPath();

                    BoardGridField.ClearPoints();
                    BoardGridField.AddPoints(range, RangeSprite);
                    BoardGridField.AddPoints(areaOfEffect, AreaOfEffectSprite);
                    BoardGridField.AddPoints(path, PathSprite);

                    _initialTargeting = false;
                }
            }
        }

        DebugIsTargeting = _isTargeting;
    }
}
