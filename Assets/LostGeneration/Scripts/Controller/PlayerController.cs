﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using LostGen;

[RequireComponent(typeof(RangedSkillController))]
[RequireComponent(typeof(DirectionalSkillController))]
public class PlayerController : MonoBehaviour {
    #region EditorFields
    public BoardCamera Camera;
    public PlayerSkillTray SkillTray;
    public Button GoButton;
    #endregion EditorFields

    #region Events
    public UnityEvent Stepped;
    #endregion Events

    public bool IsReady = false;

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
            CycleCombatantsForward();
        }

        if (AreCombatantsReady()) {
            GoButton.gameObject.SetActive(true);
        }
    }

    #endregion MonoBehaviour

    #region PublicMethods
    public void Go() {
        for (int i = 0; i < _units.Count; i++) {
            _units[i].ActiveSkill.Fire();
            ClearActiveSkill(_units[i]);
        }

        Stepped.Invoke();
    }

    public void AddCombatant(Combatant combatant) {
        _units.Add(combatant);
        SkillTray.AddCombatant(combatant);
        if (_units.Count == 1) {
            _activeUnit = 0;
            SkillTray.Combatant = _units[0];
        }
    }

    public void RemoveCombatant(Combatant combatant) {
        _units.Remove(combatant);
        SkillTray.RemoveCombatant(combatant);
    }
    #endregion PublicMethods

    #region PrivateMethods

    private void CycleCombatantsForward() {
        _activeUnit = (_activeUnit + 1) % _units.Count;
        SkillTray.Combatant = _units[_activeUnit];
        Camera.Pan(_units[_activeUnit].Position, 0.5f);
    }

    private bool AreCombatantsReady() {
        bool isReady = true;
        for (int i = 0; i < _units.Count; i++) {
            int actionPoints = _units.ActionPoints;
            foreach (CombatantAction action in _units[i].Actions) {
                actionPoints -= action.ActionPoints;
            }
            
            isReady &= actionPoints == 0;
        }

        return isReady;
    }

    #endregion PrivateMethods
}
