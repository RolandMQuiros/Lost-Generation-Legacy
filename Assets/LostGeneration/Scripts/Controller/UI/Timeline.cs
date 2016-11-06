using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LostGen;

public class Timeline : MonoBehaviour {
    public UnityEvent Ended;
    public UnityEvent Rewound;

    private Slider _slider;

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
    
    public void RemoveCombatant(Combatant combatant) {
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
    }

    public void ToBeginning() {
        _slider.value = 0;
    }

    public void ToEnd() {
        _slider.value = _maxSteps;
    }

    public void SetStep(int step) {
        if (step >= 0 && step <= _maxSteps && step != _currentStep) {
            if (step == _maxSteps) {
                Ended.Invoke();
            } else {
                Rewound.Invoke();
            }
                
            int difference = step - _currentStep;

            if (difference > 0) {
                for (int i = _currentStep; i < step; i++) {
                    for (int j = 0; j < _combatants.Count; j++) {
                        PawnAction action = _combatants[j].Actions.ElementAtOrDefault(i);
                        if (action != null) {
                            action.Do();
                        }
                    }
                } 
            } else if (difference < 0) {
                for (int i = _currentStep; i > step; i--) {
                    for (int j = 0; j < _combatants.Count; j++) {
                        PawnAction action = _combatants[j].Actions.ElementAtOrDefault(i - 1);
                        if (action != null) {
                            action.Undo();
                        }
                    }
                }
            }

            _currentStep = step;
            DebugStep = _currentStep;
        }
    }

    public void OnValueChanged(float value) {
        int newStep = Mathf.RoundToInt(value);
        SetStep(newStep);
    }

    private void OnSkillActivated(Combatant combatant, ISkill skill) {
        ToEnd();
    }

    private void OnSkillFired(Combatant combatant, ISkill skill) {
        FindMaxSteps();
        ToEnd();
    }

    private void OnActionsCleared(Pawn pawn) {
        FindMaxSteps();
    }

    private void FindMaxSteps() {
        _maxSteps = 0;
        for (int i = 0; i < _combatants.Count; i++) {
            int count = _combatants[i].Actions.Count();
            if (_maxSteps < count) {
                _maxSteps = count;
            }
        }

        if (_slider != null) {
            _slider.maxValue = _maxSteps;
        }
    }

    #region MonoBehaviour

    private void Awake() {
        _slider = GetComponent<Slider>();
        FindMaxSteps();
    }

    private void Update() {
        DebugStep = _currentStep;
    }

    #endregion MonoBehaviour
}
