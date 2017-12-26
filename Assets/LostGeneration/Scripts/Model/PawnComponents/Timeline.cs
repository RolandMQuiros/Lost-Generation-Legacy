using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen.Model
{
    /// <summary>
    /// A buffer of PawnActions that the player can scroll through, previewing the results of each action.
    /// </summary>
    public class Timeline : PawnComponent {
        

        /// <summary>
        /// The index of the current action being observed.
        /// </summary>
        public int Step {
            get { return _step; }
            set { SetStep(value); }
        }
        /// <summary>
        /// The total number of PawnActions in this timeline
        /// </summary>
        public int Count { get { return _count; } }
        public int Cost { get { return _cost; } }
        public IEnumerable<PawnAction> Actions { get { return GetPawnActions(); } }

        public event Action<PawnAction> ActionDone;
        public event Action<PawnAction> ActionUndone;
        public event Action<PawnAction> ActionAdded;
        public event Action<IEnumerable<PawnAction>> ActionsAdded;
        private class Node {
            public PawnAction Action;
            public Node Next;
            public Node Previous;
        }

        /// <summary>Front of the linked list. In lists with one element, the head and tail are the same.</summary>
        private Node _head = null;
        /// <summary>End of the linked list. In lists with one element, the head and tail are the same.</summary>
        private Node _tail = null;
        /// <summary>The currently observed PawnAction in the timeline. Should always match the current value of Step.</summary>
        private Node _current = null;
        private int _count = 0;
        private int _step = 0;
        private int _cost = 0;
        private ActionPoints _actionPoints;

        public Timeline() {
            _head = new Node();
            _tail = _head;
            _current = _head;
        }

        public override void Start() {
            // We might want to merge ActionPoints and Timeline?
            _actionPoints = Pawn.RequireComponent<ActionPoints>();
        }

        /// <summary>
        /// Adds a new action to the end of this PawnActionTimeline
        /// </summary>
        /// <param name="action">Action to add</param>
        public void PushAction(PawnAction action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            else {
                _tail.Action = action;
                _tail.Next = new Node() { Previous = _tail };
                _tail = _tail.Next;
                _count++;
                if (ActionAdded != null) { ActionAdded(action); }
            }
        }

        /// <summary>
        /// Adds multiple actions to the end of this PawnActionTimeline
        /// </summary>
        /// <param name="actions"></param>
        public void PushActions(IEnumerable<PawnAction> actions) {
            foreach (PawnAction action in actions) { PushAction(action); }
            if (ActionsAdded != null) { ActionsAdded(actions); }
        }

        /// <summary>
        /// Sets the current step. Forwards or rewinds the timeline by the difference between the new and old step.
        /// Emits the actions done or undone through <see cref="ActionsDone">ActionsDone</see> or
        /// <see cref="ActionsUndone">ActionsUndone</see>, respectively.
        /// </summary>
        /// <param name="newStep">The new step, clamped between 0 and Count</param>
        public void SetStep(int newStep) {
            if (_count > 0) {
                newStep = Math.Min(Math.Max(newStep, 0), _count);
                if (_step != newStep) {
                    while (_step > newStep) { Back(); }
                    while (_step < newStep) { Next(); }
                }
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
        public void TruncateAt(int fromStep) {
            if (fromStep >= 0 && fromStep < _count) {
                SetStep(fromStep);

                // Unlink the following nodes, and the action leading to them
                _current.Action = null;
                _current.Next = null;
                _tail = _current;
                _count = fromStep;
            }
        }

        public void Truncate() {
            TruncateAt(_step);
        }

        public void SpliceAt(int step, IEnumerable<PawnAction> actions) {
            TruncateAt(step);
            PushActions(actions);
        }

        public void Splice(IEnumerable<PawnAction> actions) {
            TruncateAt(_step);
            PushActions(actions);
        }

        /// <summary>
        /// Truncates the entire timeline.
        /// </summary>
        /// <param name="undone"></param>
        public void Clear() {
            TruncateAt(0);
        }

        /// <summary>
        /// Moves the iterator forward. Does not emit.
        /// </summary>
        /// <returns>The next action</returns>
        public PawnAction Next() {
            PawnAction next = _current.Action;
            if (_current.Next != null) {
                _current = _current.Next;
                _step++;
            }
            if (next != null) {
                next.Do();
                if (ActionDone != null) { ActionDone(next); }
                _actionPoints.Current -= next.Cost;
            }
            return next;
        }

        /// <summary>
        /// Moves the iterator back. Does not emit.
        /// </summary>
        /// <returns>The previous action</returns>
        public PawnAction Back() {
            PawnAction previous = null;
            if (_current.Previous != null) {
                _step--;
                _current = _current.Previous;
                previous = _current.Action;
            }
            if (previous != null) {
                previous.Undo();
                if (ActionUndone != null) { ActionUndone(previous); }
                _actionPoints.Current += previous.Cost;
            }
            return previous;
        }
        public void ToFirst() { SetStep(0); }
        public void ToLast() { SetStep(Count); }

        public IEnumerable<PawnAction> GetPawnActions() {
            Node node = _head;
            while (node != _tail) {
                yield return node.Action;
                node = node.Next;
            }
        }

        public void Apply() {
            Pawn.PushActions(GetPawnActions());
        }
    }
}