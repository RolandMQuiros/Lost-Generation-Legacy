using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

public class TestSkillManager : ISkillManager {
    public ISkill GetSkill(int skillID, Combatant owner) {
        ISkill skill = null;

        switch (skillID) {
            case 0:
                skill = new WalkSkill(owner);
                break;
            case 1:
                skill = new MeleeAttackSkill(owner, new Point[] {
                    Point.Right,
                    Point.Right * 2
                });
                break;
        }

        return skill;
    }
}