using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(PlayerCombatantController))]
[RequireComponent(typeof(PlayerTimelineController))]
public class SkillController : MonoBehaviour
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
    private PlayerTimelineController _timelines;

    private SkillSet _skillSet;
    private Combatant _combatant;
    private ISkill _activeSkill;

    public void SetActiveSkill(ISkill skill)
    {
        if (_skillSet.HasSkill(skill))
        {
            _activeSkill = skill;
            SkillActivated.Invoke(_activeSkill);
        }
    }

    public void DeactivateSkill()
    {
        _activeSkill = null;
        SkillActivated.Invoke(null);
    }
    
    public void OnCombatantActivated(Combatant combatant)
    {
        _combatant = combatant;
        _skillSet = _combatant.Pawn.GetComponent<SkillSet>();
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
                    _timelines.SetAction(action);
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
    private void Awake()
    {
        _playerController = GetComponent<PlayerCombatantController>();
        _timelines = GetComponent<PlayerTimelineController>();
    }
    #endregion MonoBehaviour
}