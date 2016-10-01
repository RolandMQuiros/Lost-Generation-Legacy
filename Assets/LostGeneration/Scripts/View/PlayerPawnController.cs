using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class PlayerPawnController : MonoBehaviour, IPawnController {
    public event Action<IPawnController> Ready;

    public List<Combatant> _units = new List<Combatant>();
    private int _activeUnit;
    private Dictionary<Combatant, ISkill> _activeSkills = new Dictionary<Combatant, ISkill>();

    public void BeginTurn() {
        foreach (ISkill skill in _activeSkills.Values) {
            skill.Fire();
        }    
    }

    public void Awake() {

    }

    public void Update() {
        CycleUnits();
    }

    public void AddCombatant(Combatant combatant) {
        _units.Add(combatant);
    }

    public void RemoveCombatant(Combatant combatant) {
        _units.Remove(combatant);
    }

    private void CycleUnits() {
        int cycleUnit = (Input.GetButtonDown("Switch Right") ? 1 : 0) -
                        (Input.GetButtonDown("Switch Left") ? 1 : 0);

        if (cycleUnit != 0) {
            _activeUnit = (_activeUnit + cycleUnit) % _units.Count;

        }
    }
}
