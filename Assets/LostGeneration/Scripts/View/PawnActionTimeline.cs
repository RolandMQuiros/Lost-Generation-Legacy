using System;
using System.Linq;
using System.Collections.Generic;
using LostGen;

public class PawnActionTimeline
{
    private class Node
    {
        public PawnAction Action;
        public Node Next;
        public Node Previous;
    }

    public Pawn Pawn { get; }
    public int Step { get { return _step; } }
    public int Count { get { return _count; } }
    
    private Node _head = null;
    private Node _tail = null;
    private Node _current = null;
    private int _count = 0;

    private int _step;

    public PawnActionTimeline()
    {
        _head = new Node();
        _tail = new Node() { Previous = _head };
        _head.Next = _tail;
        _current = _tail;
    }

    public void PushAction(PawnAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }
        else
        {
            _tail.Action = action;
            _tail.Next = new Node() { Previous = _tail };
            _tail = _tail.Next;
            _count++;
        }
    }

    public void TruncateAt(int step, List<PawnAction> undone = null)
    {
        if (step >= 0 && step < _count)
        {
            // Delete everything after the given step
            while (_count >= step)
            {
                // If we pass by the current node, move it back in the timeline
                if (_tail == _current)
                {
                    PawnAction action = Back();
                    if (undone != null)
                    {
                        undone.Add(action);
                    }
                }

                // Chop off the last node
                _tail = _tail.Previous;
                _tail.Next.Previous = null;
                _tail.Next = null;
                _count--;
            }
            _tail.Action = null;
        }
    }

    public void Clear(List<PawnAction> undone = null)
    {
        TruncateAt(0, undone);
    }

    public PawnAction Next()
    {
        PawnAction currentAction = null;
        if (_current.Action != null)
        {
            currentAction = _current.Action;
            currentAction.Do();
            _current = _current.Next;
            _step++;
        }
        return currentAction;
    }

    public PawnAction Back()
    {
        PawnAction currentAction = null;
        if (_current.Action != null)
        {
            currentAction = _current.Action;
            currentAction.Undo();
            _current = _current.Previous;
            _step--;
        }
        return currentAction;
    }

    public IEnumerable<PawnAction> GetPawnActions()
    {
        Node node = _head.Next;
        while (node.Next != _tail)
        {
            yield return node.Action;
        }
    }
}