using System;
using System.Collections.Generic;
using LostGen;

public interface ISkillController {
    void StartTargeting(ISkill skill);
    void CancelTargeting();
}
