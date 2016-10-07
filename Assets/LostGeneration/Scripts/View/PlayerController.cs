using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class PlayerController : MonoBehaviour, IPawnController {
    public BoardController BoardController;
    public PlayerSkillTray SkillTray;

    public bool IsReady = false;
    public event Action<IPawnController> Ready;

    private List<Combatant> _units = new List<Combatant>();
    private int _activeUnit;
    private Dictionary<Combatant, ISkill> _activeSkills = new Dictionary<Combatant, ISkill>();

    private RangedSkillController _rangedSkillController;
    private DirectionalSkillController _directionalSkillController;

    #region MonoBehaviour

    private void Awake() {
        _rangedSkillController = GetComponent<RangedSkillController>();
        _directionalSkillController = GetComponent<DirectionalSkillController>();
    }

    private void Start() {
        SkillTray.RangedSkillController = _rangedSkillController;
        SkillTray.DirectionalSkillController = _directionalSkillController;
    }

    private void Update() {
        if (Input.GetButtonDown("Tab")) {
            _activeUnit = (_activeUnit + 1) % _units.Count;

            Combatant unit = _units[_activeUnit];
            SkillTray.Combatant = unit;
        }
    }

    #endregion

    public void BeginTurn() {
        foreach (ISkill skill in _activeSkills.Values) {
            skill.Fire();
        }
    }

    public void AddCombatant(Combatant combatant) {
        _units.Add(combatant);
        if (_units.Count == 1) {
            _activeUnit = 0;
            SkillTray.Combatant = _units[0];
        }
    }

    public void RemoveCombatant(Combatant combatant) {
        _units.Remove(combatant);
    }

    public void SetActiveSkill(ISkill skill) {
        _activeSkills[skill.Owner] = skill;

        IsReady = true;
        for (int i = 0; i < _units.Count; i++) {
            IsReady &= (_activeSkills.ContainsKey(_units[i]));
        }
    }

    public void ClearActiveSkill(Combatant combatant) {
        _activeSkills.Remove(combatant);
        IsReady = false;
    }
}
