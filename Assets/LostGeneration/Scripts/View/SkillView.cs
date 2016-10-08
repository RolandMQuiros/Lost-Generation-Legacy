using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(BoardGridField))]
public class SkillView : MonoBehaviour {
    public ISkill Skill {
        get { return _skill; }
        set {
            _skill = value;
            _oldTarget = null;
            _oldDirection = null;
        }
    }

    #region EditorFields
    public Sprite RangeSprite;
    public Sprite AreaOfEffectSprite;
    public Sprite PathSprite;
    #endregion EditorFields

    #region Components
    private BoardGridField _gridField;
    #endregion Components

    private ISkill _skill;
    private Point? _oldTarget;
    private CardinalDirection? _oldDirection;

    #region MonoBehaviour
    private void Awake() {
        _gridField = GetComponent<BoardGridField>();
    }

    private void LateUpdate() {
        if (Skill.IsActiveSkill) {

        }
    }
    #endregion MonoBehaviour

    public void BuildGridField() {
        RangedSkill ranged;
        DirectionalSkill directional;
        if ((ranged = _skill as RangedSkill) != null) {
            if (_oldTarget == null || _oldTarget.Value != ranged.Target) {
                _gridField.ClearPoints();

                if (!ranged.IsReadyToFire) {
                    _gridField.AddPoints(ranged.GetRange(), RangeSprite);
                }

                _gridField.AddPoints(ranged.GetAreaOfEffect(), AreaOfEffectSprite);
                _gridField.AddPoints(ranged.GetPath(), PathSprite);
            }
        } else if ((directional = _skill as DirectionalSkill) != null) {
            if (_oldDirection == null || _oldDirection.Value != directional.Direction) {
                _gridField.ClearPoints();
                _gridField.AddPoints(directional.GetAreaOfEffect(), AreaOfEffectSprite);
            }
        }
    }

    public void Clear() {
        _gridField.ClearPoints();
    }
}
