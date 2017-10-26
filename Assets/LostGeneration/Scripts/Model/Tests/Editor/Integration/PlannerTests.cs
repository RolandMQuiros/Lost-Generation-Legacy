using System;
using System.Collections.Generic;
using NUnit.Framework;
using LostGen;

namespace Tests.Integration {
    public class PlannerTests {
        private class CurrentState {
            public int GuardianHealth = 3;
            public bool IsNearFridge = false;
            public bool IsFridgeLocked = true;
            public bool IsFridgeOpen = false;
            public bool HasBanana = false;
        }

        private class MoveToFridge : IDecision {
            public int DecisionCost { get { return 2; } }
            private CurrentState _state;

            public MoveToFridge(CurrentState state) {
                _state = state;
            }

            public StateOffset ApplyPostconditions(StateOffset state) {
                state.Add("NearFridge", true);
                return state;
            }

            public bool ArePreconditionsMet(StateOffset state) {
                return !state.Get("NearFridge", _state.IsNearFridge);
            }

            public void Run() {
                Console.Write("Moved near fridge");
                _state.IsNearFridge = true;
            }

            public void Setup() { }
        }

        private class KillFridgeGuardian : IDecision {
            public int DecisionCost { get { return 3; } }
            private CurrentState _state;

            public KillFridgeGuardian(CurrentState state) {
                _state = state;
            }

            public StateOffset ApplyPostconditions(StateOffset state) {
                int guardianHealth = state.Get("GuardianHealth", _state.GuardianHealth) - 1;
                if (guardianHealth <= 0) {
                    state.Add("GuardianHealth", 0);
                    state.Add("GuardianDead", true);
                    state.Add("FridgeLocked", false);
                } else {
                    state.Add("GuardianHealth", guardianHealth);
                    state.Add("GuardianDead", false);
                    state.Add("FridgeLocked", true);
                }

                return state;
            }

            public bool ArePreconditionsMet(StateOffset state) {
                return state.Get("GuardianHealth", _state.GuardianHealth) > 0 &&
                       state.Get("FridgeLocked", _state.IsFridgeLocked) &&
                       state.Get("NearFridge", _state.IsNearFridge);
            }

            public void Run() {
                _state.GuardianHealth--;
                if (_state.GuardianHealth > 0) {
                    Console.WriteLine("Attacked Guardian!");
                } else {
                    Console.WriteLine("Killed Guardian!");
                }
            }

            public void Setup() { }
        }

        private class OpenFridgeDecision : IDecision {
            public int DecisionCost { get { return 1; } }
            private CurrentState _state;

            public OpenFridgeDecision(CurrentState state) {
                _state = state;
            }

            public StateOffset ApplyPostconditions(StateOffset state) {
                state.Add("FridgeOpen", true);
                return state;
            }

            public bool ArePreconditionsMet(StateOffset state) {
                return !state.Get("FridgeLocked", _state.IsFridgeLocked) &&
                       !state.Get("FridgeOpen", _state.IsFridgeOpen);
            }
            
            public void Run() {
                Console.WriteLine("Opened fridge.");
                _state.IsFridgeOpen = true;
            }

            public void Setup() { }
        }

        private class GetBananaFromFridge : IDecision {
            public int DecisionCost { get { return 1; } }
            private CurrentState _state;

            public GetBananaFromFridge(CurrentState state) {
                _state = state;
            }

            public StateOffset ApplyPostconditions(StateOffset state) {
                state.Add("HasBanana", true);
                return state;
            }

            public bool ArePreconditionsMet(StateOffset state) {
                return state.Get("FridgeOpen", _state.IsFridgeOpen) &&
                       !state.Get("HasBanana", _state.HasBanana);
            }

            public void Run() {
                Console.Write("Got the Banana!");
                _state.HasBanana = true;
            }

            public void Setup() { }
        }

        private Planner BasicPlanner() {
            CurrentState state = new CurrentState();
            IDecision[] decisions = new IDecision[] {
                new MoveToFridge(state),
                new KillFridgeGuardian(state),
                new OpenFridgeDecision(state),
                new GetBananaFromFridge(state)
            };

            Planner planner = new Planner(StateOffset.Heuristic);
            for (int i = 0; i < decisions.Length; i++) {
                planner.AddDecision(decisions[i]);
            }

            return planner;
        }
        
        [Test]
        public void GoalNodeNeighborIterator() {
            CurrentState state = new CurrentState();
            List<IDecision> decisions = new List<IDecision>() {
                new MoveToFridge(state),
                new KillFridgeGuardian(state),
                new OpenFridgeDecision(state),
                new GetBananaFromFridge(state)
            };

            StateOffset goal = new StateOffset();
            //goal.SetStateValue("HasBanana", true, false);
            GoalNode end = new GoalNode(goal, decisions);

//            int i = decisions.Count - 1;
            foreach (GoalNode neighbor in end.GetNeighbors()) {
                Console.WriteLine(end.GetEdge(neighbor));
                //Assert.AreSame(decisions[i--], end.GetEdge(neighbor));
            }
        }


        [Test]
        public void SingleSolution() {
            CurrentState state = new CurrentState();
            IDecision[] decisions = new IDecision[] {
                new MoveToFridge(state),
                new KillFridgeGuardian(state),
                new OpenFridgeDecision(state),
                new GetBananaFromFridge(state)
            };

            Planner planner = new Planner(StateOffset.Heuristic);
            for (int i = 0; i < decisions.Length; i++) {
                planner.AddDecision(decisions[i]);
            }

            StateOffset goal = new StateOffset();
            goal.Add("HasBanana", true);

            Queue<IDecision> plan = planner.CreatePlan(goal);

            Assert.Greater(plan.Count, 0);
            Assert.AreSame(decisions[0], plan.Dequeue()); // MoveToFridge
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGuardian
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGUardian
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGuardian
            Assert.AreSame(decisions[2], plan.Dequeue()); // OpenFridge
            Assert.AreSame(decisions[3], plan.Dequeue()); // GetBananaFromFridge
        }
    }
}
