using LostGen;

public interface ISkillFactory {
    ISkill GetSkill(int skillID, Combatant owner);    
}
