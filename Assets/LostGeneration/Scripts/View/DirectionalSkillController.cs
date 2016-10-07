using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class DirectionalSkillController : MonoBehaviour, ISkillController {
    public ISkill Skill {
        get { return _skill; }
        set {
            _skill = (DirectionalSkill)value;
        }
    }

    public BoardCursor Cursor;
    public BoardGridField BoardGridField;
    public Sprite AreaOfEffectSprite;

    public string DebugDirection;
    public bool DebugIsTargeting;

    private DirectionalSkill _skill;
    private bool _isTargeting = false;
    private bool _initialTargeting = false;

    public void StartTargeting() {
        _isTargeting = true;
        _initialTargeting = true;
    }

    public void CancelTargeting() {
        BoardGridField.ClearPoints();
        _isTargeting = false;
        _initialTargeting = false;
    }

    private void Update() {
        if (_isTargeting && Cursor.BoardPoint != _skill.Owner.Position) {
            CardinalDirection direction = Point.DirectionBetweenPoints(Cursor.BoardPoint, _skill.Owner.Position);

            if (direction != _skill.Direction || _initialTargeting) {
                IEnumerable<Point> areaOfEffect = _skill.GetAreaOfEffect(_skill.Direction);

                _skill.Direction = direction;
                BoardGridField.ClearPoints();
                BoardGridField.AddPoints(areaOfEffect, AreaOfEffectSprite);

                _initialTargeting = false;
            }

            if (Cursor.TapUp) {
                CancelTargeting();
            }
        }

        DebugDirection = (_skill != null) ? _skill.Direction.ToString() : "nuthin";
        DebugIsTargeting = _isTargeting;
    }
}

