using System;
using System.Collections.Generic;
using LostGen;

public interface ISkillController {
    ISkill Skill { get; set; }
    void StartTargeting();
    void CancelTargeting();
}
