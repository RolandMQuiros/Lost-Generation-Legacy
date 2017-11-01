
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class CompoundTask : ITask, IEnumerable<ITask> {
        public delegate int StateScore(WorldState from, WorldState to); 
        private class StateNode : IGraphNode<StateNode> {
            public WorldState State { get { return _state; } }
            public ITask Cause { get { return _cause; } }
            private WorldState _state;
            private ITask _cause;
            private IEnumerable<ITask> _taskPool;
            private Dictionary<StateNode, ITask> _neighbors = null;
            private StateScore _edgeCost;

            public StateNode(WorldState state, ITask cause, IEnumerable<ITask> taskPool, StateScore edgeCost) {
                _state = state;
                _cause = cause;
                _taskPool = taskPool;
                _edgeCost = edgeCost;
            }

            public int GetEdgeCost(StateNode neighbor) {
                return _edgeCost(_state, neighbor._state);
            }
            public IEnumerable<StateNode> GetNeighbors() {
            if (_neighbors == null) {
                    _neighbors = new Dictionary<StateNode, ITask>();
                    foreach (ITask task in _taskPool.Where(t => t.Preconditions.IsSubsetOf(_state))) {
                        StateNode newNode = new StateNode(
                            _state * task.Postconditions,
                            task,
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
                    throw new ArgumentException("neighbor", "StateNode at\n" + neighbor._state + "\nis not a neighbor of this StateNode at\n" + _state);
                }
                return _neighbors[neighbor];
            }
        }
        
        private HashSet<ITask> _subtasks = new HashSet<ITask>();
        private StateScore _transitionScore;
        private WorldState _preconditions = new WorldState();
        private WorldState _postconditions = new WorldState();

        public WorldState Preconditions { get { return _preconditions; } }
        public WorldState Postconditions { get { return _postconditions; } }

        public CompoundTask(StateScore transitionScore) {
            _transitionScore = transitionScore;
        }
        
        #region IEnumerable
        public bool Add(ITask subtask) {
            bool added = _subtasks.Add(subtask);
            if (added) {
                _preconditions += subtask.Preconditions;
                _postconditions += subtask.Postconditions;
            }
            return added;
        }

        public bool Remove(ITask subtask) {
            bool removed = _subtasks.Remove(subtask);
            if (removed) {
                _preconditions = new WorldState(_subtasks.SelectMany( s => s.Preconditions ));
                _postconditions = new WorldState(_subtasks.SelectMany( s => s.Preconditions ));
            }
            return removed;
        }

        public IEnumerator<ITask> GetEnumerator() {
            return _subtasks.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion IEnumerable

        #region ITask
        public IEnumerable<ITask> Decompose(WorldState from, WorldState to) {
            StateNode start = new StateNode(from, null, _subtasks, _transitionScore);
            StateNode end = new StateNode(to, null, _subtasks, _transitionScore);

            return GraphMethods.FindPath<StateNode>(start, end, Heuristic)
                               .Where(n => n.Cause != null)
                               .Select(n => n.Cause);
        }

        public IEnumerator Do(WorldState start, WorldState goal) {
            foreach (ITask task in Decompose(start, goal)) {
                IEnumerator runner = task.Do(start, goal);
                while (runner.MoveNext()) {
                    if (task.ArePreconditionsMet() && runner.Current != null) {
                        yield return runner.Current;
                    } else {
                        yield return null; // Precondition check failed
                        break;
                    }
                }
            }
        }

        public bool ArePreconditionsMet() {
            return _subtasks.Any(s => s.ArePreconditionsMet());
        }
        #endregion ITask

        private int Heuristic(StateNode start, StateNode end) {
            //return WorldState.Heuristic(start.State, end.State);
            return 1;
        }
    }
}