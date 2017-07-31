using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(PlayerCombatantController))]
[RequireComponent(typeof(PlayerTimelineController))]
public class PlayerSkillController : MonoBehaviour
{

    [System.Serializable]
    public class SkillEvent : UnityEvent<ISkill> { }
    public SkillSet SkillSet
    {
        get { return _skillSet; }
    }
    public ISkill ActiveSkill {
        get { return _activeSkill; }
    }

    public SkillEvent SkillActivated;
    public SkillEvent SkillFired;

    private PlayerCombatantController _playerController;
    private PlayerTimelineController _timelineController;

    private SkillSet _skillSet;
    private Combatant _combatant;
    private Timeline _timeline;
    private ISkill _activeSkill;
    [SerializeField]
    private int _activationStep;

    public void SetActiveSkill(ISkill skill)
    {
        if (_skillSet.HasSkill(skill))
        {
            _activeSkill = skill;
            
            // Save the timeline step when skill was activated
            _activationStep = Math.Min(Math.Max(0, _timeline.Count), _timelineController.Step);
            // Move forward all timelines for Combatants that are slower than the active Combatant
            _timelineController.SetSlowerStep(_activationStep + 1, _activeSkill.Pawn);

            SkillActivated.Invoke(_activeSkill);
        }
    }

    public void CancelSkill() {
        if (_activeSkill != null) {
            // Rewind timeline back to when skill was activated
            _timelineController.SetAllStep(_activationStep);
            DeactivateSkill();
        }
    }

    public void DeactivateSkill() {
        _activeSkill = null;
        SkillActivated.Invoke(null);
    }
    
    public void OnCombatantActivated(Combatant combatant)
    {
        _combatant = combatant;
        _skillSet = _combatant.Pawn.GetComponent<SkillSet>();
        _timeline = _combatant.Pawn.GetComponent<Timeline>();
    }

    public void FireActiveSkill()
    {
        if (_activeSkill != null)
        {
            PawnAction action = _activeSkill.Fire();
            if (action != null)
            {
                if (action.Cost <= _combatant.ActionPoints.Current)
                {
                    // Move timeline back to the current Pawn's latest step
                    _timelineController.SetAllStep(Math.Min(Math.Min(_timelineController.Step, _timeline.Count), _activationStep));
                    // Push the new action and step forward
                    _timelineController.SetStepAction(action);
                    SkillFired.Invoke(_activeSkill);
                    DeactivateSkill();
                }
                else
                {
                    // Play error noise
                }
            }
        }
    }

    #region MonoBehaviour
    private void Awake() {
        _playerController = GetComponent<PlayerCombatantController>();
        _timelineController = GetComponent<PlayerTimelineController>();
    }
    #endregion MonoBehaviour
}