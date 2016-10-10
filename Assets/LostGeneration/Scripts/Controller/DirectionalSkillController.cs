using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class DirectionalSkillController : MonoBehaviour, ISkillController {
    public BoardCursor Cursor;

    public string DebugDirection;
    public bool DebugIsTargeting;

    private DirectionalSkill _skill;
    private bool _isTargeting = false;
    private bool _initialTargeting = false;

    #region MonoBehaviour
    private void Update() {
        if (_skill != null && _isTargeting && Cursor.BoardPoint != _skill.Owner.Position) {
            CardinalDirection direction = Point.DirectionBetweenPoints(Cursor.BoardPoint, _skill.Owner.Position);

            if (direction != _skill.Direction || _initialTargeting) {
                _skill.Direction = direction;
                _initialTargeting = false;
                _skill.IsReadyToFire = false;
            }

            if (Cursor.TapUp) {
                _isTargeting = false;
                _initialTargeting = false;
                _skill.IsReadyToFire = true;
            }
        }

        DebugDirection = (_skill != null) ? _skill.Direction.ToString() : "nuthin";
        DebugIsTargeting = _isTargeting;
    }
    #endregion MonoBehaviour

    public void StartTargeting(ISkill skill) {
        _skill = (DirectionalSkill)skill;
        _skill.IsReadyToFire = false;
        _isTargeting = true;
        _initialTargeting = true;
    }

    public void CancelTargeting() {
        _isTargeting = false;
        _initialTargeting = false;

        if (_skill != null) {
            _skill.IsReadyToFire = false;
        }

        _skill = null;
    }
}

