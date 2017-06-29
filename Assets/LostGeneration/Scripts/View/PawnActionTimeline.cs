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
    public void PushAction(PawnAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }
        else
        {
            if (_head == null)
            {
                _head = new Node() { Action = action };
                _tail = _head;
                _current = _head;
            }
            else
            {
                _tail.Next = new Node() { Previous = _tail, Action = action };
                _tail = _tail.Next;
            }

            if (_count == _step)
            {
                _current = _tail;
            }
            _count++;
        }
    }

    public void TruncateAt(int step, List<PawnAction> undone = null)
    {
        if (step >= 0 && step < _count)
        {
            // If the current node is out-of-bounds, bring it back in
            if (_step >= _count)
            {
                _current = _tail;
            }

            // Delete everything after the given step
            while (_count > step)
            {
                while (_count <= _step)
                {
                    PawnAction action = Back();
                    if (action != null && undone != null)
                    {
                        undone.Add(action);
                    }
                }

                // Chop off the last node
                Node previous = _tail.Previous;
                _tail.Previous = null;
                _tail = previous;
                if (_tail != null)
                {
                    _tail.Next = null;
                }
                else
                {
                    _head = null;
                }
                _count--;
            }
        }
    }

    public void Clear(List<PawnAction> undone = null)
    {
        TruncateAt(0, undone);
    }

    public PawnAction Next()
    {
        PawnAction currentAction = null;
        if (_current != null)
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
        if (_current != null)
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
        Node node = _head;
        while (node != null)
        {
            yield return node.Action;
            node = node.Next;
        }
    }
}