using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class SkillButton : MonoBehaviour {
    public event Action<ISkill> Activated;
    private ISkill _skill;

    public void Initialize(ISkill skill) {
    	_skill = skill;
    }

    public void Activate() {
        Activated.Invoke(_skill);
    }
}
