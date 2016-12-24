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
    public SkillEvent SkillActivated;
    public SkillEvent SkillDeactivated;
    #endregion Events

    public bool IsReady = false;

    private List<Combatant> _units = new List<Combatant>();
    private int _activeUnit;
    private bool _isSkillActive = true;
    private ISkill _latestSkill = null;
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
        DeactivateSkill();
        _units.Add(combatant);
        CombatantAdded.Invoke(combatant);
    }

    public void RemoveCombatant(Combatant combatant) {
        int idx = _units.IndexOf(combatant);
        DeactivateSkill();
        
        if (idx != -1) {
            if (idx == _activeUnit) {
                CycleCombatants(-1);
            }
            _units.RemoveAt(idx);
        }

        CombatantRemoved.Invoke(combatant);
    }

    public void ActivateSkill(ISkill skill) {
        _latestSkill = skill;
        SkillActivated.Invoke(skill);
    }

    public void DeactivateSkill() {
        SkillDeactivated.Invoke(_latestSkill);
        _latestSkill = null;
    }
    public void ToggleSkill(ISkill skill) {
        if (_latestSkill == skill) {
            DeactivateSkill();
        } else if (skill is RangedSkill) {
            _rangedSkillController.StartTargeting(skill);
            ActivateSkill(skill);
        } else if (skill is DirectionalSkill) {
            _directionalSkillController.StartTargeting(skill);
            ActivateSkill(skill);
        }
    }
    #endregion PublicMethods

    #region CombatantMethods
    public void ClearActions() {
        if (_units.Count > 0) {
            _units[_activeUnit].ClearActions();
        }
    }
    #endregion CombatantMethods

    #region PrivateMethods

    private void CycleCombatants(int offset) {
        if (_units.Count > 0) {
            _activeUnit = (_activeUnit + offset) % _units.Count;

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
    #endregion PrivateMethods
}
