using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

public class Character {
    /// <summary>ID of this character on the central remote database.</summary>
    public int CharacterID { get; private set; }
    /// <summary>Character's first name</summary>
    public string FirstName { get; private set; }
    /// <summary>Character's surname</summary>
    public string LastName { get; private set; }
    /// <summary>Character's nickname</summary>
    public string Nickname { get; private set; }

    public int Neuroticism;
    public int Agreeableness;
    public int Conscientiousness;
    public int Extraversion;
    public int Openness;

    public decimal Money;
    public decimal LoanBalance;
    public decimal InterestRate;

    public Stats BaseStats;

    private ISkillManager _skillManager;
    private HashSet<int> _skills;

    public Character(int characterID, string firstName, string lastName, string nickname, int[] skills, ISkillManager skillManager) {
        CharacterID = characterID;
        FirstName = firstName;
        LastName = lastName;
        Nickname = nickname;
        
        if (skills.Length > 0) {
            _skills = new HashSet<int>(skills);
        } else {
            _skills = new HashSet<int>();
        }
        _skillManager = skillManager;
    }

    public bool AddSkill(int skillID) {
        return _skills.Add(skillID);
    }

    public Combatant CreateCombatant(Board board, Point position) {
        string name;
        if (Nickname != null && Nickname.Length > 0) {
            name = Nickname;
        } else {
            name = FirstName + " " + LastName;
        }

        Combatant combatant = new Combatant(name, board, position);
        foreach (int skillID in _skills) {
            ISkill skill = _skillManager.GetSkill(skillID, combatant);
            combatant.AddSkill(skill);
        }

        combatant.BaseStats = BaseStats;

        return combatant;
    }
}
