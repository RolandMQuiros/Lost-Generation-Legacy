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

    /// <summary>
    /// Adds a Combatant to this TimelineController, creating a new Timeline keyed to that Combatant, and binding
    /// the relevant events.
    /// </summary>
    /// <param name="combatant">Combatant to add</param>
    public void AddCombatant(Combatant combatant) {
        if (_timelines.Find(t => t.Source == combatant) == null) {
            _timelines.Add(new Timeline(combatant));
        }
    }
    
    public void RemoveCombatant(Combatant combatant) {
        int index = _timelines.FindIndex(t => t.Source == combatant);
        if (index != -1) {
            _timelines.RemoveAt(index);
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
                Ended.Invoke();
            } else if (!_isRewound) {
                _isRewound = true;                
                // Bug note 12 Dec 2016:
                // I set PlayerController.ClearAllActiveSkills as a handler for this.  This nulled out the activeSkill before it could be fired
                // so the SkillFired event would fire and the BoardGridField wouldn't clear.  
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

    public void OnSkillActivated(Combatant combatant, ISkill skill) {
        _slider.value = combatant.Actions.Count();
    }

    public void OnSkillFired(Combatant combatant, ISkill skill) {
        CalculateMaxSteps();
        _slider.value = combatant.Actions.Count();
    }

    public void OnActionsCleared(Pawn pawn, IEnumerable<PawnAction> clearedActions) {
        CalculateMaxSteps();
        _slider.value = _maxSteps;
    }

    private void CalculateMaxSteps() {
        // Find the Timeline with the largest number of Actions queued
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
