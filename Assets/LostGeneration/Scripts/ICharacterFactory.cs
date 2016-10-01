using UnityEngine;
using LostGen;

public interface ICharacterFactory {
    Character GetCharacter(int characterID);
    GameObject GetCombatantViewObject(int characterID);
}
