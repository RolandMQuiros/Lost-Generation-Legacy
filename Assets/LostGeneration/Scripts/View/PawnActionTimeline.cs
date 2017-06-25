using System;
using System.Linq;
using System.Collections.Generic;
using LostGen;

public class PawnActionTimeline
{
    public int Step { get { return _step; } }
    public int Count { get { return _actions.Count; } }

    private List<PawnAction> _actions = new List<PawnAction>();
    private int _step;

    public void PushBack(PawnAction action)
    {
        _actions.Add(action);
    }

    public void TruncateAt(int step)
    {
        SetStep(step);
        _actions.RemoveRange(step, _actions.Count - step);
    }

    public void Clear()
    {
        SetStep(0);
        _actions.Clear();
    }

    public void Forward()
    {
        SetStep(_step + 1);
    }

    public void Backward()
    {
        SetStep(_step - 1);
    }

    public void SetStep(int step)
    {
        if (step < _actions.Count)
        {
            int deltaStep = _step - step;
            if (deltaStep > 0)
            {
                for (int i = Math.Min(_step, _actions.Count - 1); i >= step; i--)
                {
                    _actions[i].Undo();
                }
            }
            else if (deltaStep < 0)
            {
                for (int i = Math.Max(0, _step); i < step; i++)
                {
                    _actions[i].Do();
                }
            }
        }
        _step = step;
    }
}