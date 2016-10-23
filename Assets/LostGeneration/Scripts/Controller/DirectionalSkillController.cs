using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class DirectionalSkillController : MonoBehaviour, ISkillController {
    public BoardCursor Cursor;

    public Point Origin {
        get { return _origin; }
        set { _origin = value; }
    }

    public string DebugDirection;
    public bool DebugIsTargeting;

    private DirectionalSkill _skill;
    private Point _origin;
    private bool _isTargeting = false;
    private bool _initialTargeting = false;

    public void StartTargeting(Point origin, ISkill skill) {
        _origin = origin;
        _skill = (DirectionalSkill)skill;
        _isTargeting = true;
        _initialTargeting = true;
    }
    
    #region MonoBehaviour
    private void Update() {
        if (_skill != null && _isTargeting && Cursor.BoardPoint != _skill.Owner.Position) {
            CardinalDirection direction = Point.DirectionBetweenPoints(_skill.Owner.Position, Cursor.BoardPoint);

            if (direction != _skill.Direction || _initialTargeting) {
                _skill.SetDirection(direction);
                _initialTargeting = false;
            }

            if (Cursor.TapUp) {
                _isTargeting = false;
                _initialTargeting = false;
            }
        }

        DebugDirection = (_skill != null) ? _skill.Direction.ToString() : "nuthin";
        DebugIsTargeting = _isTargeting;
    }
    #endregion MonoBehaviour
}

