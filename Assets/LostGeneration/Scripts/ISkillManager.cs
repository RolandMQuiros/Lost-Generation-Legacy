using LostGen;

public interface ISkillManager {
    ISkill GetSkill(int skillID, Combatant owner);    
}
