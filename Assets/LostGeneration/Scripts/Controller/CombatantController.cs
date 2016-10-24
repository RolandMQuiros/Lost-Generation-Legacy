using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[Serializable]
public class CombatantController : ScriptableObject {
    public Combatant Combatant {
        get { return _combatant; }
    }

    private Combatant _combatant;

    public void Initialize(Combatant combatant) {
        _combatant = combatant;   
    }

    public void ClearActions() {
        _combatant.ClearActions();
    }
}
