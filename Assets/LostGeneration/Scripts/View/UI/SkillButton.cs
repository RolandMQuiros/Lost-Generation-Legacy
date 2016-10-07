using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class SkillButton : MonoBehaviour {
    public ISkill Skill;
    public ISkillController SkillController;
    public PlayerController PlayerController;

    public void OnClick() {
        SkillController.Skill = Skill;
        SkillController.StartTargeting();
        PlayerController.SetActiveSkill(Skill);
    }

    public void OnOtherClick() {
        SkillController.Skill = null;
        SkillController.CancelTargeting();
        PlayerController.ClearActiveSkill(Skill.Owner);
    }
}
