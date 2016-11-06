using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

public class Timeline {
    public Pawn Source { get { return _source; } }
    public int Step {
        get { return _currentStep; }
        set { SetStep(value); }
    }

    private Pawn _source;
    private int _currentStep = 0;

    public Timeline(Pawn source) {
        _source = source;

        _source.ActionsCleared += OnActionsCleared;
    }

    public void SetStep(int step) {
        if (step >= 0 && step <= _source.Actions.Count() && step != _currentStep) {
            int difference = step - _currentStep;

            if (difference > 0) {
                for (int i = _currentStep; i < step; i++) {
                    PawnAction action = _source.Actions.ElementAtOrDefault(i);
                    if (action != null) {
                        action.Do();
                    }
                }
            } else if (difference < 0) {
                for (int i = _currentStep; i > step; i--) {
                    PawnAction action = _source.Actions.ElementAtOrDefault(i - 1);
                    if (action != null) {
                        action.Undo();
                    }
                }
            }

            _currentStep = step;
        }
    }

    private void OnActionsCleared(Pawn source, IEnumerable<PawnAction> actionsCleared) {
        for (int i = _currentStep; i > 0; i--) {
            PawnAction action = actionsCleared.ElementAtOrDefault(i);
            if (action != null) {
                action.Undo();
            }
        }
    }
}

