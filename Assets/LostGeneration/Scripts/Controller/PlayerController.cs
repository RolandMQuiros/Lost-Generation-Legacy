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
            CycleCombatantsForward();
        }

        if (AreCombatantsReady()) {
            GoButton.gameObject.SetActive(true);
        }
    }

    #endregion MonoBehaviour

    #region PublicMethods
    public void AddCombatant(Combatant combatant) {
        _units.Add(combatant);
        CombatantAdded.Invoke(combatant);
    }

    public void RemoveCombatant(Combatant combatant) {
        _units.Remove(combatant);
        CombatantRemoved.Invoke(combatant);
    }
    #endregion PublicMethods

    #region CombatantMethods
    public void ClearActions() {
        _units[_activeUnit].ClearActions();
    }

    public void ClearActiveSkill() {
        _units[_activeUnit].ClearActiveSkill();
    }

    public void ClearAllActiveSkills() {
        for (int i = 0; i < _units.Count; i++) {
            _units[i].ClearActiveSkill();
        }
    }
    #endregion CombatantMethods

    #region PrivateMethods

    private void CycleCombatantsForward() {
        _activeUnit = (_activeUnit + 1) % _units.Count;
        CombatantSwitched.Invoke(_units[_activeUnit]);

        Camera.Pan(_units[_activeUnit].Position, 0.5f);
        DebugPanel.Combatant = _units[_activeUnit];
    }

    private bool AreCombatantsReady() {
        bool isReady = true;
        for (int i = 0; i < _units.Count; i++) {            
            isReady &= _units[i].ActionQueueCost == _units[i].ActionPoints;
        }

        return isReady;
    }

    #endregion PrivateMethods
}
