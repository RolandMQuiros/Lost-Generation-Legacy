using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class SkillController : MonoBehaviour
{

    [System.Serializable]
    public class SkillEvent : UnityEvent<ISkill> { }
    public SkillSet SkillSet
    {
        get { return _skillSet; }
    }
    public SkillEvent SkillActivated;
    private SkillSet _skillSet;
    private ISkill _activeSkill;

    public void SetActiveSkill(ISkill skill)
    {
        if (_skillSet.HasSkill(skill))
        {
            _activeSkill = skill;
            SkillActivated.Invoke(_activeSkill);
        }
    }

    public void OnCombatantActivated(Combatant combatant)
    {
        _skillSet = combatant.Pawn.GetComponent<SkillSet>();
    }
}