
using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class CompoundTask {
        public delegate int StateScore(StateOffset from, StateOffset to);
        private class StateNode : IGraphNode<StateNode> {
            public StateOffset State { get { return _state; } }
            private StateOffset _state;
            private IEnumerable<ITask> _taskPool;
            private Dictionary<StateNode, ITask> _neighbors = null;
            private StateScore _edgeCost;

            public StateNode(StateOffset state, IEnumerable<ITask> taskPool, StateScore edgeCost) {
                _state = state;
                _taskPool = taskPool;
                _edgeCost = edgeCost;
            }

            public int GetEdgeCost(StateNode neighbor) {
                return _edgeCost(_state, neighbor._state);
            }
            public IEnumerable<StateNode> GetNeighbors() {
            if (_neighbors == null) {
                    _neighbors = new Dictionary<StateNode, ITask>();
                    foreach (ITask task in _taskPool.Where(t => t.ArePreconditionsMet(_state))) {
                        StateNode newNode = new StateNode(
                            task.ApplyPostconditions(_state),
                            _taskPool.Where(t => t != task),
                            _edgeCost
                        );
                        _neighbors.Add(newNode, task);
                    }
                }

                return _neighbors.Keys;
            }
            public bool IsMatch(StateNode other) {
                return other._state.IsSubsetOf(_state);
            }

            public ITask GetTask(StateNode neighbor) {
                ITask task;
                if (!_neighbors.TryGetValue(neighbor, out task)) {
                    throw new ArgumentException("neighbor", "This StateNode at " + neighbor._state + " is not a neighbor of this StateNode");
                }
                return _neighbors[neighbor];
            }
        }
        
        private HashSet<ITask> _subtasks = new HashSet<ITask>();
        private StateScore _transitionScore;

        public CompoundTask(StateScore transitionScore) {
            _transitionScore = transitionScore;
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
            StateNode start = new StateNode(from, _subtasks, _transitionScore);
            StateNode end = new StateNode(to, _subtasks, _transitionScore);

            StateNode previous = null;
            foreach (StateNode stateNode in GraphMethods.FindPath<StateNode>(start, end, Heuristic)) {
                if (previous == null) { previous = stateNode; }
                else { yield return stateNode.GetTask(previous); }
            }
        }
        private int Heuristic(StateNode start, StateNode end) {
            return StateOffset.Heuristic(start.State, end.State);
        }
    }
}