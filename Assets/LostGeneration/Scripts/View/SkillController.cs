using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(PlayerCombatantController))]
[RequireComponent(typeof(PlayerTimelines))]
public class SkillController : MonoBehaviour
{

    [System.Serializable]
    public class SkillEvent : UnityEvent<ISkill> { }
    public SkillSet SkillSet
    {
        get { return _skillSet; }
    }

    public SkillEvent SkillActivated;
    public SkillEvent SkillFired;

    private PlayerCombatantController _playerController;
    private PlayerTimelines _timelines;

    private SkillSet _skillSet;
    private ISkill _activeSkill;

    public void SetActiveSkill(ISkill skill)
    {
        if (_skillSet.HasSkill(skill))
        {
            _activeSkill = skill;
            _timelines.TruncateTimeline(skill.Owner);
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
        _skillSet = combatant.Pawn.GetComponent<SkillSet>();
    }

    public void FireActiveSkill()
    {
        if (_activeSkill != null)
        {
            PawnAction action = _activeSkill.Fire();
            if (action != null)
            {
                _timelines.SetAction(action);
                SkillFired.Invoke(_activeSkill);
                DeactivateSkill();
            }
        }
    }

    private void Awake()
    {
        _playerController = GetComponent<PlayerCombatantController>();
        _timelines = GetComponent<PlayerTimelines>();
    }
}