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
        if (_skill.Owner.ActiveSkill == _skill) {
            _skill.Owner.ClearActiveSkill();
        } else {
            _skill.Owner.SetActiveSkill(_skill);
            Activated.Invoke(_skill);
        }
    }
}
