using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class SkillButton : MonoBehaviour {
    public ISkill Skill;
    public ISkillController SkillController;
    public PlayerController PlayerController;

    public void OnClick() {
        SkillController.StartTargeting(Skill);
        PlayerController.SetActiveSkill(Skill);
    }

    public void OnOtherClick() {
        SkillController.CancelTargeting();
    }
}
