using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class SkillButton : MonoBehaviour {
    public ISkill Skill { get { return _skill; } }
    public event Action<ISkill> Activated;
    private ISkill _skill;

    public void Initialize(ISkill skill) {
    	_skill = skill;
    }

    public void OnActivate() {
        _skill.Owner.SetActiveSkill(_skill);

        PawnAction lastAction = _skill.Owner.LastAction;
        Point origin;

        if (lastAction != null) {
            origin = lastAction.PostRunPosition;
        } else {
            origin = _skill.Owner.Position;
        }

        Activated.Invoke(_skill);
    }
}
