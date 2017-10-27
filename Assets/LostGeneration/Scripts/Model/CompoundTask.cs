
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class CompoundTask {
        public delegate int StateHeuristic(StateOffset from, StateOffset to);
        private class StateNode : IGraphNode<StateNode> {
            public StateOffset State { get { return _state; } }
            public ITask Cause { get { return _cause; } }
            private StateOffset _state;
            private ITask _cause;
            private int _taskLevel = 0;
            private HashSet<ITask> _taskPool;
            private Dictionary<StateNode, ITask> _neighbors = null;
            private Func<StateOffset, StateOffset, int> _edgeCost;

            public StateNode(ITask cause, StateOffset state, HashSet<ITask> taskPool, Func<StateOffset, StateOffset, int> edgeCost) {
                _cause = cause;
                _state = state;
                _taskPool = taskPool;
                _edgeCost = edgeCost;
            }

            public int GetEdgeCost(StateNode neighbor) {
                return _edgeCost(_state, neighbor._state);
            }
            public IEnumerable<StateNode> GetNeighbors() {
                if (_taskLevel >= 20) {
                    return Enumerable.Empty<StateNode>();
                }

                if (_neighbors == null) {
                    _neighbors = new Dictionary<StateNode, ITask>();
                    foreach (ITask task in _taskPool.Where(t => t.ArePreconditionsMet(_state))) {
                        StateNode newNode = new StateNode(
                            task,
                            task.ApplyPostconditions(_state),
                            new HashSet<ITask>(_taskPool.Where(t => t != this)),
                            _edgeCost
                        );
                        newNode._taskLevel = _taskLevel + 1;

                        _neighbors.Add(newNode, task);
                    }
                }

                return _neighbors.Keys;
            }
            public bool IsMatch(StateNode other) {
                return other._state.IsSubsetOf(_state);
            }

            public ITask GetTask(StateNode neighbor) {
                return _neighbors[neighbor];
            }
        }
        
        private HashSet<ITask> _subtasks = new HashSet<ITask>();
        private Func<StateOffset, StateOffset, int> _transitionScore;

        public CompoundTask(Func<StateOffset, StateOffset, int> transitionScore) {
            _transitionScore = transitionScore;
        }

        public StateOffset ApplyPreconditions(StateOffset state) {
            StateOffset preconditions =
                _subtasks.Select(s => s.ApplyPreconditions(state))
                         .Aggregate((accumulator, next) => StateOffset.Intersect(accumulator, next));
            return preconditions;
        }

        public StateOffset ApplyPostconditions(StateOffset state) {
            StateOffset postconditions =
                _subtasks.Select(s => s.ApplyPostconditions(state))
                         .Aggregate((accumulator, next) => StateOffset.Intersect(accumulator, next));
            return postconditions;
        }

        public bool AddSubtask(ITask subtask) {
            return _subtasks.Add(subtask);
        }

        public bool RemoveSubtask(ITask subtask) {
            return _subtasks.Remove(subtask);
        }
        public IEnumerable<ITask> Decompose(StateOffset from, StateOffset to) {
            // GOAP goes here
            StateNode start = new StateNode(null, from, _subtasks, _transitionScore);
            StateNode end = new StateNode(null, to, _subtasks, _transitionScore);

            foreach (StateNode stateNode in GraphMethods.FindPath<StateNode>(start, end, Heuristic)) {
                if (stateNode.Cause != null) { yield return stateNode.Cause; }
            }
        }
        private int Heuristic(StateNode start, StateNode end) {
            return StateOffset.Heuristic(start.State, end.State);
        }
    }
}