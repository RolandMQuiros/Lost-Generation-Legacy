using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

public class Character {
    /// <summary>ID of this character on the central remote database.</summary>
    public string CharacterID { get; private set; }
    /// <summary>Character's first name</summary>
    public string FirstName { get; private set; }
    /// <summary>Character's surname</summary>
    public string LastName { get; private set; }
    /// <summary>Character's nickname</summary>
    public string NickName { get; private set; }

    public int Neuroticism;
    public int Agreeableness;
    public int Conscientiousness;
    public int Extraversion;
    public int Openness;

    public decimal Money;
    public decimal LoanBalance;
    public decimal InterestRate;

    public Stats BaseStats;

    private HashSet<ISkill> _skills = new HashSet<ISkill>();

    public bool AddSkill(ISkill skill) {
        return _skills.Add(skill);
    }

    public Combatant CreateCombatant(Board board, Point position) {
        string name;
        if (NickName != null) {
            name = NickName;
        } else {
            name = FirstName + " " + LastName;
        }

        Combatant combatant = new Combatant(name, board, position);
        foreach (ISkill skill in _skills) {
            combatant.AddSkill(skill);
        }

        return combatant;
    }
}
