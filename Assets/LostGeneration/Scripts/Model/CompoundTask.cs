
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public class CompoundTask : ITask {
        public delegate int StateScore(WorldState from, WorldState to);

        private readonly WorldState EmptyState = new WorldState();
        private class StateNode : IGraphNode<StateNode> {
            public WorldState State { get { return _state; } }
            private WorldState _state;
            private IEnumerable<ITask> _taskPool;
            private Dictionary<StateNode, ITask> _neighbors = null;
            private StateScore _edgeCost;

            public StateNode(WorldState state, IEnumerable<ITask> taskPool, StateScore edgeCost) {
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
                    foreach (ITask task in _taskPool.Where(t => _state.Count == 0 || _state.IsSubsetOf(t.Postconditions))) {
                        StateNode newNode = new StateNode(
                            _state + task.Preconditions,
                            _taskPool.Where(t => t != task),
                            _edgeCost
                        );
                        _neighbors.Add(newNode, task);
                    }
                }

                return _neighbors.Keys;
            }
            public bool IsMatch(StateNode other) {
                return _state.IsSubsetOf(other._state);
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
        private WorldState _preconditions = null;
        private WorldState _postconditions = null;

        public WorldState Preconditions { get { return _preconditions ?? EmptyState; } }
        public WorldState Postconditions { get { return _postconditions ?? EmptyState; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transitionScore">
        ///     A scoring function that assigns a cost value for transitioning between two <see cref="WorldState"/>s
        /// </param>
        public CompoundTask(StateScore transitionScore) {
            _transitionScore = transitionScore;
        }

        public bool ArePreconditionsMet() {
            // Return true if at least one action has satisfied preconditions
            return _subtasks.Any(t => t.ArePreconditionsMet());
        }

        public bool AddSubtask(ITask subtask) {
            bool added = _subtasks.Add(subtask);
            
            // Add the common state values of the new subtask
            if (added) {
                if (_preconditions == null || _preconditions.Count == 0) {
                    _preconditions = new WorldState(subtask.Preconditions);
                } else {
                    _preconditions = new WorldState(_preconditions.Intersect(subtask.Preconditions));
                }

                if (_postconditions == null || _postconditions.Count == 0) {
                    _postconditions = new WorldState(subtask.Postconditions);
                } else {
                    _postconditions = new WorldState(_postconditions.Intersect(subtask.Postconditions));
                }
            }

            return added;
        }

        public bool RemoveSubtask(ITask subtask) {
            bool removed = _subtasks.Remove(subtask);

            if (removed) {
                foreach (KeyValuePair<string, object> pair in _preconditions.Intersect(subtask.Postconditions)) {
                    _preconditions.Remove(pair.Key);
                }

                foreach (KeyValuePair<string, object> pair in _postconditions.Intersect(subtask.Postconditions)) {
                    _postconditions.Remove(pair.Key);
                }
            }

            return removed;
        }

        public IEnumerable<ITask> Decompose(WorldState from, WorldState to) {
            StateNode start = new StateNode(from, _subtasks, _transitionScore);
            StateNode end = new StateNode(to, _subtasks, _transitionScore);

            StateNode previous = null; 
            foreach (StateNode stateNode in GraphMethods.FindPath<StateNode>(end, start, Heuristic)) {
                if (previous == null) { previous = stateNode; }
                else { yield return stateNode.GetTask(previous); }
            }
        }

        public IEnumerator Do(WorldState start, WorldState goal) {
            foreach (ITask task in Decompose(start, goal)) {
                if (task.ArePreconditionsMet()) {
                    yield return task.Do(goal);
                }
            }
        }

        private int Heuristic(StateNode start, StateNode end) {
            //return WorldState.Heuristic(start.State, end.State);
            return 1;
        }
    }
}