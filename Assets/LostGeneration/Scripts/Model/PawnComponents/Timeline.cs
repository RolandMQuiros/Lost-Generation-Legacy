using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen
{
    /// <summary>
    /// A buffer of PawnActions that the player can scroll through, previewing the results of each action.
    /// </summary>
    public class Timeline : PawnComponent
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
        public PawnAction CurrentAction { get { return (_current != null) ? _current.Action : null; } }
        
        /// <summary>Front of the linked list. In lists with one element, the head and tail are the same.</summary>
        private Node _head = null;
        /// <summary>End of the linked list. In lists with one element, the head and tail are the same.</summary>
        private Node _tail = null;
        /// <summary>The currently observed PawnAction in the timeline. Should always match the current value of Step.</summary>
        private Node _current = null;
        private int _count = 0;
        private int _step = 0;

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
                    _current = _tail;
                }
                else
                {
                    _tail.Next = new Node() { Previous = _tail, Action = action };
                    _tail = _tail.Next;
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
        /// <param name="fromStep">The step from which to truncate the timeline.</param>
        /// <param name="undone">List of PawnActions that were undone by the truncate. Includes all actions between the currently observed Step and the given
        /// step parameter, inclusive.</param>
        public void TruncateAt(int fromStep, List<PawnAction> undone = null)
        {
            if (fromStep >= 0 && fromStep < _count)
            {
                // Delete everything after the given step
                while (_count > fromStep)
                {
                    while (_count - 1 <= _step)
                    {
                        PawnAction action = _current.Action; 
                        action.Undo();
                        if (undone != null) { undone.Add(action); }
                        if (!Back()) { break; }
                    }

                    // Chop off the last node
                    Node previous = _tail.Previous;
                    _tail.Previous = null;
                    _tail.Action = null;
                    _tail = previous;
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
        public bool Next()
        {
            bool hasNext = _current.Next != null; 
            if (hasNext)
            {
                _current = _current.Next;
                _step++;
            }
            return hasNext;
        }

        public bool Back()
        {
            bool hasPrevious = _current.Previous != null;
            if (hasPrevious)
            {
                _current = _current.Previous;
                _step--;
            }
            return hasPrevious;
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

        public void Apply()
        {
            foreach (PawnAction action in GetPawnActions())
            {
                Pawn.PushAction(action);
            }
        }
    }
}