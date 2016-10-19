using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class SkillView : MonoBehaviour {
    #region EditorFields
    public Sprite RangeSprite;
    public Sprite AreaOfEffectSprite;
    public Sprite PathSprite;
    #endregion EditorFields

    #region References
    private Combatant _combatant;
    private RangedSkill _ranged;
    private DirectionalSkill _directional;
    private BoardGridField _gridField;
    #endregion References

    public void Initialize(Combatant combatant, BoardGridField gridField) {
        _combatant = combatant;
        _combatant.SkillActivated += OnSkillActivated;

        _gridField = gridField;
    }

    public void Clear() {
        _gridField.ClearPoints();
    }

    public void OnTargetingEnd() {
        if (_ranged != null) {
            _gridField.ClearPoints();
            _gridField.AddPoints(_ranged.GetPath(), PathSprite);
            _gridField.AddPoints(_ranged.GetAreaOfEffect(), AreaOfEffectSprite);
            _gridField.RebuildMesh();
        }
    }

    private void OnTargetChanged(Point point) {
        _gridField.ClearPoints();
        _gridField.AddPoints(_ranged.GetRange(), RangeSprite);
        _gridField.AddPoints(_ranged.GetPath(), PathSprite);
        _gridField.AddPoints(_ranged.GetAreaOfEffect(), AreaOfEffectSprite);
        _gridField.RebuildMesh();
    }

    private void OnDirectionChanged(CardinalDirection direction) {
        _gridField.ClearPoints();
        _gridField.AddPoints(_directional.GetAreaOfEffect(_directional.Direction), AreaOfEffectSprite);
        _gridField.RebuildMesh();
    }

    private void OnSkillActivated(Combatant combatant, ISkill skill) {
        // Unbind delegates from old skill
        if (_ranged != null) {
            _ranged.TargetChanged -= OnTargetChanged;
        } else if (_directional != null) {
            _directional.DirectionChanged -= OnDirectionChanged;
        }

        // Figure out what kind of skill it is
        _ranged = skill as RangedSkill;
        _directional = skill as DirectionalSkill;

        // Bind delegates to new skill
        if (_ranged != null) {
            _ranged.TargetChanged += OnTargetChanged;
            OnTargetChanged(_ranged.Target);
        } else if (_directional != null) {
            _directional.DirectionChanged += OnDirectionChanged;
            OnDirectionChanged(_directional.Direction);
        }
    }
}
