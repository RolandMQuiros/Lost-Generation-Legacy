using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(RangedSkillController))]
[RequireComponent(typeof(DirectionalSkillController))]
public class PlayerController : MonoBehaviour, IPawnController {
    public BoardController BoardController;
    public PlayerSkillTray SkillTray;

    public bool IsReady = false;
    public event Action<IPawnController> Ready;

    private List<Combatant> _units = new List<Combatant>();
    private int _activeUnit;

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
        for (int i = 0; i < _units.Count; i++) {
            _units[i].ActiveSkill.Fire();
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
        skill.Owner.ActiveSkill = skill;

        IsReady = true;
        for (int i = 0; i < _units.Count; i++) {
            ISkill activeSkill = _units[i].ActiveSkill;
            IsReady &= activeSkill != null && activeSkill.IsReadyToFire;
        }
    }

    public void ClearActiveSkill(Combatant combatant) {
        combatant.ActiveSkill = null;
        IsReady = false;
    }
}
