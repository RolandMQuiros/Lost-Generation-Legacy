using System;
using System.Linq;
using System.Collections.Generic;
using LostGen;

/// <summary>
/// A buffer of PawnActions that the player can scroll through, previewing the results of each action.
/// </summary>
public class PawnActionTimeline
{
    private class Node
    {
        public PawnAction Action;
        public Node Next;
        public Node Previous;
    }

    /// <summary>
    /// The index of the current action being observed.
    /// </summary>
    public int Step { get { return _step; } }
    /// <summary>
    /// The total number of PawnActions in this timeline
    /// </summary>
    public int Count { get { return _count; } }
    
    /// <summary>Front of the linked list. In lists with one element, the head and tail are the same.</summary>
    private Node _head = null;
    /// <summary>End of the linked list. In lists with one element, the head and tail are the same.</summary>
    private Node _tail = null;
    /// <summary>The currently observed PawnAction in the timeline. Should always match the current value of Step.</summary>
    private Node _current = null;
    private int _count = 0;
    private int _step;

    /// <summary>
    /// Adds a new action to the end of this PawnActionTimeline
    /// </summary>
    /// <param name="action">Action to add</param>
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

    /// <summary>
    /// Removes all PawnActions that occur after, and including, the given step.  If the current value of the Step member is between the step parameter and
    /// the Count member, all PawnActions between Step and step will be undone.
    ///
    /// This is typically called when the player wants to rethink their strategy from a given step, allowing them to redo every action after the step while
    /// preserving what comes before.
    /// </summary>
    /// <param name="step">The step from which to truncate the timeline.</param>
    /// <param name="undone">List of PawnActions that were undone by the truncate. Includes all actions between the currently observed Step and the given
    /// step parameter, inclusive.</param>
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

    /// <summary>
    /// Truncates the entire timeline.
    /// </summary>
    /// <param name="undone"></param>
    public void Clear(List<PawnAction> undone = null)
    {
        TruncateAt(0, undone);
    }

    /// <summary>
    /// Runs the Do() function on the currently observed PawnAction, then moves the pointer to the next.
    /// </summary>
    /// <returns>The action that was Done. null if before the beginning or past the end.</returns>
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