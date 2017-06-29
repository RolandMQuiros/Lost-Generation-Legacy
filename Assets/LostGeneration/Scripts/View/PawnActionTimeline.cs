using System;
using System.Linq;
using System.Collections.Generic;
using LostGen;

public class PawnActionTimeline
{
    public Pawn Pawn { get; }
    public int Step { get { return _step; } }
    public int Count { get { return _actions.Count; } }
    public IEnumerable<PawnAction> Actions { get { return _actions; } }
    
    private List<PawnAction> _actions = new List<PawnAction>();
    private int _step;

    public void PushAction(PawnAction action)
    {
        _actions.Add(action);
    }

    public void TruncateAt(int step, List<PawnAction> undone = null)
    {
        while (_step >= step)
        {
            PawnAction action = Back();
            if (undone != null)
            {
                undone.Add(action);
            }
        }
        _actions.RemoveRange(_step, _actions.Count - _step);
    }

    public void Clear(List<PawnAction> undone = null)
    {
        while (_step >= 0)
        {
            PawnAction action = Back();
            if (undone != null)
            {
                undone.Add(action);
            }
        }
        _actions.Clear();
    }

    public PawnAction Next()
    {
        PawnAction currentAction = null;
        if (_step >= 0 && _step < _actions.Count)
        {
            currentAction = _actions[_step];
            currentAction.Do();
        }
        _step++;
        return currentAction;
    }

    public PawnAction Back()
    {
        PawnAction currentAction = null;
        if (_step >= 0 && _step < _actions.Count)
        {
            currentAction = _actions[_step];
            currentAction.Undo();
        }
        _step++;
        return currentAction;
    }
}