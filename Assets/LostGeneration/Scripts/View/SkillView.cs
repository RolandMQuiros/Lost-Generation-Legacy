using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class SkillView : MonoBehaviour {
    public ISkill Skill {
        get { return _skill; }
        set { _skill = value; }
    }

    #region EditorFields
    public Sprite RangeSprite;
    public Sprite AreaOfEffectSprite;
    public Sprite PathSprite;
    #endregion EditorFields

    #region References
    private BoardGridField _gridField;
    private ISkill _skill;
    #endregion References

    private Point? _oldTarget = null;
    private CardinalDirection? _oldDirection = null;
    private bool _wasReadyToFire = false;
      
    public void Initialize(BoardGridField gridField) {
        _gridField = gridField;
    }

    // Wire this to SkillButton.Activated somehow
    public void SetSkill(ISkill skill) {
        _skill = skill;
    }
    
    public void BuildGridField() {
        if (_skill != null) {
            RangedSkill ranged;
            DirectionalSkill directional;
            if ((ranged = _skill as RangedSkill) != null) {
                if (!_oldTarget.HasValue || _oldTarget.Value != ranged.Target || _wasReadyToFire) {
                    _gridField.ClearPoints();

                    if (!ranged.IsReadyToFire) {
                        _gridField.AddPoints(ranged.GetRange(), RangeSprite);
                    }

                    _gridField.AddPoints(ranged.GetPath(), PathSprite);
                    _gridField.AddPoints(ranged.GetAreaOfEffect(), AreaOfEffectSprite);
                    _gridField.RebuildMesh();

                    _wasReadyToFire = ranged.IsReadyToFire;
                    _oldTarget = ranged.Target;
                }
            } else if ((directional = _skill as DirectionalSkill) != null) {
                if (!_oldDirection.HasValue || _oldDirection.Value != directional.Direction) {
                    _gridField.ClearPoints();
                    _gridField.AddPoints(directional.GetAreaOfEffect(directional.Direction), AreaOfEffectSprite);
                    _gridField.RebuildMesh();

                    _oldDirection = directional.Direction;
                }
            }
        } else {
            throw new NullReferenceException("No Combatant was assigned to this SkillView");
        }
    }

    public void Clear() {
        _gridField.ClearPoints();
    }

    #region MonoBehaviour
    private void LateUpdate() {
        if (_skill != null) {
            BuildGridField();
        } else if (!_gridField.IsEmpty) {
            _gridField.ClearPoints();
        }
    }
    #endregion MonoBehaviour
}
