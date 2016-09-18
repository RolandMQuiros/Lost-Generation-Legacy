using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

public class TestCharacterManager : ICharacterManager {
    private TestSkillManager _skillManager = new TestSkillManager();
    private Dictionary<int, Character> _characterCache;

    public TestCharacterManager() {
        _characterCache = new Dictionary<int, Character>() {
            {
                1,
                new Character(
                    1,
                    "Ezekiel",
                    "Figuero",
                    "MC Books",
                    new int[] { 0, 1 },
                    _skillManager
                ) {
                    Neuroticism = 48,
                    Agreeableness = 41,
                    Conscientiousness = 64,
                    Extraversion = 32,
                    Openness = 77,
                    Money = 3413.26M,
                    LoanBalance = 33924.75M,
                    InterestRate = 9.22M,
                    BaseStats = new Stats() {
                        Health = 24,
                        Attack = 6,
                        Defense = 4,
                        Magic = 7,
                        Agility = 6,
                        Stamina = 10
                    }
                }
            },
            {
                2,
                new Character(
                    2,
                    "Shaolin",
                    "Fantastic",
                    "Shao",
                    new int[] { 0, 1 },
                    _skillManager
                ) {
                    Neuroticism = 71,
                    Agreeableness = 45,
                    Conscientiousness = 44,
                    Extraversion = 68,
                    Openness = 89,
                    Money = 122435.26M,
                    LoanBalance = 433924.75M,
                    InterestRate = 14.58M,
                    BaseStats = new Stats() {
                        Health = 28,
                        Attack = 8,
                        Defense = 3,
                        Magic = 2,
                        Agility = 9,
                        Stamina = 10
                    }
                }
            }
        };
    }

    public Character GetCharacter(int characterID) {
        Character character;
        _characterCache.TryGetValue(characterID, out character);
        return character;
    }
}
