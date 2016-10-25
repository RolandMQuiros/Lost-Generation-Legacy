using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LostGen;

[RequireComponent(typeof(Scrollbar))]
public class Timeline : MonoBehaviour {
    public UnityEvent Played;
    public UnityEvent Rewinded;

    private Scrollbar _scrollbar;

    private List<Combatant> _combatants = new List<Combatant>();
    private int _currentStep = 0;
    private int _maxSteps = 0;

    public int DebugStep = 0;

    public void AddCombatant(Combatant combatant) {
        if (!_combatants.Contains(combatant)) {
            _combatants.Add(combatant);
            FindMaxSteps();
            combatant.SkillFired += OnSkillFired;
            combatant.SkillActivated += OnSkillActivated;
            combatant.ActionsCleared += OnActionsCleared;

            for (int i = 0; i < combatant.ActionCount; i++) {
                PawnAction action = combatant.Actions.ElementAtOrDefault(i);
                action.Do();
            }
        }
    }
    
    public bool RemoveCombatant(Combatant combatant) {
        bool removed = _combatants.Remove(combatant);

        if (removed) {
            FindMaxSteps();
            combatant.SkillFired -= OnSkillFired;
            combatant.SkillActivated -= OnSkillActivated;
            combatant.ActionsCleared -= OnActionsCleared;
            for (int i = _currentStep; i > 0; i--) {
                PawnAction action = combatant.Actions.ElementAtOrDefault(i);
                action.Undo();
            }
        }

        return removed;
    }

    public void ResetToPresent() {
        SetStep(_maxSteps);
    }

    public void SetStep(int step) {
        if (step >= 0 && step <= _maxSteps) {
            if (step == _maxSteps) {
                Played.Invoke();
            } else {
                Rewinded.Invoke();
            }

            int difference = step - _currentStep;

            if (difference > 0) {
                for (int i = _currentStep; i < step; i++) {
                    for (int j = 0; j < _combatants.Count; j++) {
                        PawnAction action = _combatants[j].Actions.ElementAtOrDefault(i);
                        if (action != null) { action.Do(); }
                    }
                }
            } else if (difference < 0) {
                for (int i = _currentStep; i >= step; i--) {
                    for (int j = 0; j < _combatants.Count; j++) {
                        PawnAction action = _combatants[j].Actions.ElementAtOrDefault(i);
                        if (action != null) { action.Undo(); }
                    }
                }
            }
            _currentStep = step;
        }
    }

    private void OnValueChanged(float value) {
        int newStep = Mathf.RoundToInt((1f - value) * _maxSteps);
        SetStep(newStep);
    }

    private void OnSkillActivated(Combatant combatant, ISkill skill) {
        ResetToPresent();
    }

    private void OnSkillFired(Combatant combatant, ISkill skill) {
        FindMaxSteps();
    }

    private void OnActionsCleared(Pawn pawn) {
        FindMaxSteps();
    }

    private void FindMaxSteps() {
        ResetToPresent();
        _maxSteps = 0;
        for (int i = 0; i < _combatants.Count; i++) {
            int count = _combatants[i].Actions.Count();
            if (_maxSteps < count) {
                _maxSteps = count;
            }
        }

        if (_scrollbar != null) {
            _scrollbar.numberOfSteps = (_maxSteps + 1);
            _scrollbar.size = 1f / (_maxSteps + 1);
        }
    }

    #region MonoBehaviour

    private void Awake() {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.onValueChanged.AddListener(OnValueChanged);

        FindMaxSteps();
    }

    private void Update() {
        DebugStep = _currentStep;
    }

    #endregion MonoBehaviour
}
