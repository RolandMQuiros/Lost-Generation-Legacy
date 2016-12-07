using System;
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
    public Button GoButton;
    public DebugPanel DebugPanel;
    #endregion EditorFields

    #region Events
    public UnityEvent Stepped;
    public CombatantEvent CombatantAdded;
    public CombatantEvent CombatantRemoved;
    public CombatantEvent CombatantSwitched;

    /// <summary>Editor Wrapper for Combatant.SkillFired. Invoked after a Skill is fired.</summary>
    public CombatantSkillEvent SkillFired;
    public CombatantSkillEvent SkillActivated;
    public CombatantSkillEvent SkillDeactivated;

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

    private void Update() {
        if (Input.GetButtonDown("Tab")) {
            CycleCombatants(1);
        }

        if (AreCombatantsReady()) {
            GoButton.gameObject.SetActive(true);
        }
    }

    #endregion MonoBehaviour

    #region PublicMethods
    public void AddCombatant(Combatant combatant) {
        _units.Add(combatant);

        if (_units.Count == 1) {
            BindSkillEvents(combatant);
        }

        CombatantAdded.Invoke(combatant);
    }

    public void RemoveCombatant(Combatant combatant) {
        int idx = _units.IndexOf(combatant);

        if (idx != -1) {
            if (idx == _activeUnit) {
                CycleCombatants(-1);
            }
            _units.RemoveAt(idx);
        }

        CombatantRemoved.Invoke(combatant);
    }
    #endregion PublicMethods

    #region CombatantMethods
    public void ClearActions() {
        if (_units.Count > 0) {
            _units[_activeUnit].ClearActions();
        }
    }

    public void ClearActiveSkill() {
        if (_units.Count > 0) {
            _units[_activeUnit].ClearActiveSkill();
        }
    }

    public void ClearAllActiveSkills() {
        for (int i = 0; i < _units.Count; i++) {
            _units[i].ClearActiveSkill();
        }
    }
    #endregion CombatantMethods

    #region PrivateMethods

    private void CycleCombatants(int offset) {
        ClearActiveSkill();

        if (_units.Count > 0) {
            // Cycle forward through list of units, unbinding and binding event listeners
            UnbindSkillEvents(_units[_activeUnit]);
            _activeUnit = (_activeUnit + offset) % _units.Count;
            BindSkillEvents(_units[_activeUnit]);

            CombatantSwitched.Invoke(_units[_activeUnit]);

            Camera.Pan(_units[_activeUnit].Position, 0.5f);
            DebugPanel.Combatant = _units[_activeUnit];
        }
    }

    private bool AreCombatantsReady() {
        bool isReady = true;
        for (int i = 0; i < _units.Count; i++) {            
            isReady &= _units[i].ActionQueueCost == _units[i].ActionPoints;
        }

        return isReady;
    }

    #region SkillEvents
    private void OnSkillFired(Combatant combatant, ISkill skill) {
        SkillFired.Invoke(combatant, skill);
    }

    private void OnSkillActivated(Combatant combatant, ISkill skill) {
        SkillActivated.Invoke(combatant, skill);
    }
    
    private void OnSkillDeactivated(Combatant combatant, ISkill skill) {
        SkillDeactivated.Invoke(combatant, skill);
    }

    private void BindSkillEvents(Combatant combatant) {
        combatant.SkillFired += OnSkillFired;
        combatant.SkillActivated += OnSkillActivated;
        combatant.SkillDeactivated += OnSkillDeactivated;
    }

    private void UnbindSkillEvents(Combatant combatant) {
        combatant.SkillFired -= OnSkillFired;
        combatant.SkillActivated -= OnSkillActivated;
        combatant.SkillDeactivated -= OnSkillDeactivated;
    }
    #endregion SkillEvents

    #endregion PrivateMethods
}
