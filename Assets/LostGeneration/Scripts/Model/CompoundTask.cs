
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class CompoundTask {
        private class StateNode : IGraphNode {
            public StateOffset State { get { return _state; } }
            private StateOffset _state;
            private HashSet<ITask> _taskPool;
            private Dictionary<StateNode, ITask> _neighbors = null;
            private Func<StateOffset, StateOffset, int> _edgeCost;

            public StateNode(StateOffset state, HashSet<ITask> taskPool, Func<StateOffset, StateOffset, int> edgeCost) {
                _state = state;
                _taskPool = taskPool;
                _edgeCost = edgeCost;
            }

            public int GetEdgeCost(IGraphNode neighbor) {
                StateNode neighborState = ((StateNode)neighbor);
                return _edgeCost(_state, neighborState._state);
            }
            public IEnumerable<IGraphNode> GetNeighbors() {
                if (_neighbors == null) {
                    _neighbors = new Dictionary<StateNode, ITask>();
                    foreach (ITask task in _taskPool) {
                        StateNode newNode = new StateNode(
                            task.ApplyPostconditions(_state),
                            _taskPool,
                            _edgeCost
                        );

                        _neighbors.Add(newNode, task);
                    }
                }

                return _neighbors.Keys.Cast<IGraphNode>();
            }
            public bool IsMatch(IGraphNode other) {
                StateNode otherState = ((StateNode)other);
                return _state.IsSubsetOf(otherState._state);
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
            StateNode start = new StateNode(from, _subtasks, _transitionScore);
            StateNode end = new StateNode(to, _subtasks, _transitionScore);

            StateNode previous = null;
            foreach (StateNode stateNode in GraphMethods<StateNode>.FindPath(start, end, Heuristic)) {
                if (previous == null) { previous = stateNode; }
                else { yield return previous.GetTask(stateNode); }
            }
        }
        private int Heuristic(StateNode start, StateNode end) {
            return StateOffset.Heuristic(start.State, end.State);
        }
    }
}