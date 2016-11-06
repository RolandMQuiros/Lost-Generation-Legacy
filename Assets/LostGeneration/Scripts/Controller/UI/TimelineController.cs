using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using LostGen;

public class TimelineController : MonoBehaviour {
    public UnityEvent Ended;
    public UnityEvent Rewound;

    private Slider _slider;

    private int _currentStep = 0;
    private int _maxSteps = 0;
    private bool _isRewound = false;

    public int DebugStep = 0;

    private List<Timeline> _timelines = new List<Timeline>();

    public void AddCombatant(Combatant combatant) {
        if (_timelines.Find(t => t.Source == combatant) == null) {
            _timelines.Add(new Timeline(combatant));

            combatant.SkillActivated += OnSkillActivated;
            combatant.SkillFired += OnSkillFired;
            combatant.ActionsCleared += OnActionsCleared;
        }
    }
    
    public void RemoveCombatant(Combatant combatant) {
        int index = _timelines.FindIndex(t => t.Source == combatant);
        if (index != -1) {
            _timelines.RemoveAt(index);

            combatant.SkillActivated -= OnSkillActivated;
            combatant.SkillFired -= OnSkillFired;
            combatant.ActionsCleared -= OnActionsCleared;
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
            int difference = step - _currentStep;

            if (step == _maxSteps) {
                _isRewound = false;
                Debug.Log("Ended");
                Ended.Invoke();
            } else if (!_isRewound) {
                _isRewound = true;
                Debug.Log("Rewound");
                Rewound.Invoke();
            }

            for (int i = 0; i < _timelines.Count; i++) {
                _timelines[i].Step = step;
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
        _slider.value = combatant.Actions.Count();
    }

    private void OnSkillFired(Combatant combatant, ISkill skill) {
        CalculateMaxSteps();
        _slider.value = combatant.Actions.Count();
    }

    private void OnActionsCleared(Pawn pawn, IEnumerable<PawnAction> clearedActions) {
        CalculateMaxSteps();
        _slider.value = _maxSteps;
    }

    private void CalculateMaxSteps() {
        _maxSteps = 0;
        for (int i = 0; i < _timelines.Count; i++) {
            int count = _timelines[i].Source.Actions.Count();
            if (_maxSteps < count) {
                _maxSteps = count;
                _isRewound = _currentStep == _maxSteps;
            }
        }

        if (_slider != null) {
            _slider.maxValue = _maxSteps;
        }
    }

    #region MonoBehaviour

    private void Awake() {
        _slider = GetComponent<Slider>();
        CalculateMaxSteps();
    }

    private void Update() {
        DebugStep = _currentStep;
    }

    #endregion MonoBehaviour
}
