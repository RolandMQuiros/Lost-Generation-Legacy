using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LostGen.Test {
    public class ActionPlannerTests {
        private class MoveToFridge : IDecision {
            public int Cost { get { return 2; } }

            public StateOffset ApplyPostconditions(StateOffset previous = null) {
                StateOffset post = previous ?? new StateOffset();

                post.SetStateValue("NearFridge", true, false);

                return post;
            }

            public StateOffset ApplyPreconditions(StateOffset next = null) {
                StateOffset pre = next ?? new StateOffset();

                pre.SetStateValue("NearFridge", false, false);

                return pre;
            }

            public void Run() {
                throw new NotImplementedException();
            }

            public void Setup() {
                throw new NotImplementedException();
            }
        }

        private class KillFridgeGuardian : IDecision {
            public int Cost { get { return 3; } }
            private int _guardianHealth;
            private int _startHealth;

            public KillFridgeGuardian(int guardianHealth) {
                _startHealth = guardianHealth;
                _guardianHealth = guardianHealth;
            }

            public StateOffset ApplyPostconditions(StateOffset previous = null) {
                StateOffset post = previous ?? new StateOffset();

                bool isGuardianDead = post.GetStateValue("GuardianDead", false);
                if (isGuardianDead) {
                    post.SetStateValue("GuardianHealth", 0, _startHealth);
                    post.SetStateValue("GuardianDead", false, false);
                    post.SetStateValue("FridgeLocked", true, true);
                } else {
                    post.SetStateValue("GuardianHealth", _guardianHealth, _startHealth);
                    post.SetStateValue("GuardianDead", true, false);                                   
                    post.SetStateValue("FridgeLocked", false, true);
                }

                return post;
            }

            public StateOffset ApplyPreconditions(StateOffset next = null) {
                StateOffset pre = next ?? new StateOffset();

                int currentGuardianHealth = next.GetStateValue("GuardianHealth", 0);
                pre.SetStateValue("GuardianHealth", currentGuardianHealth + 1, 0);
                pre.SetStateValue("GuardianDead", false, false);
                pre.SetStateValue("FridgeLocked", true, true);
                pre.SetStateValue("NextToFridge", true, false);

                return pre;
            }

            public void Run() {
                _guardianHealth--;
                if (_guardianHealth > 0) {
                    Console.WriteLine("Attacked Guardian!");
                } else {
                    Console.WriteLine("Killed Guardian!");
                }
            }

            public void Setup() { }
        }

        private class OpenFridgeDecision : IDecision {
            public int Cost { get { return 1; } }

            public StateOffset ApplyPostconditions(StateOffset previous = null) {
                StateOffset post = previous ?? new StateOffset();
                
                post.SetStateValue("FridgeOpen", true, false);

                return post;
            }

            public StateOffset ApplyPreconditions(StateOffset next = null) {
                StateOffset pre = next ?? new StateOffset();

                pre.SetStateValue("GuardianDead", true, false);
                pre.SetStateValue("FridgeLocked", false, false);
                pre.SetStateValue("FridgeOpen", false, false);

                return pre;
            }
            
            public void Run() {
                Console.WriteLine("Opened fridge.");
            }

            public void Setup() { }
        }

        private class GetBananaFromFridge : IDecision {
            public int Cost { get { return 1; } }

            public StateOffset ApplyPostconditions(StateOffset previous = null) {
                StateOffset post = previous ?? new StateOffset();

                post.SetStateValue("HasBanana", true, false);

                return post;
            }

            public StateOffset ApplyPreconditions(StateOffset next = null) {
                StateOffset pre = next ?? new StateOffset();

                pre.SetStateValue("HasBanana", false, false);
                pre.SetStateValue("FridgeOpen", true, false);

                return pre;
            }

            public void Run() {
                Console.Write("Got the Banana!");
            }

            public void Setup() { }
        }

        private ActionPlanner BasicPlanner() {
            IDecision[] decisions = new IDecision[] {
                new MoveToFridge(),
                new KillFridgeGuardian(10),
                new OpenFridgeDecision(),
                new GetBananaFromFridge()
            };

            ActionPlanner planner = new ActionPlanner(StateOffset.Heuristic);
            for (int i = 0; i < decisions.Length; i++) {
                planner.AddDecision(decisions[i]);
            }

            return planner;
        }
        
        [Test]
        public void GoalNodeNeighborIterator() {
            List<IDecision> decisions = new List<IDecision>() {
                new MoveToFridge(),
                new KillFridgeGuardian(1),
                new OpenFridgeDecision(),
                new GetBananaFromFridge()
            };

            StateOffset goal = new StateOffset();
            //goal.SetStateValue("HasBanana", true, false);
            GoalNode end = new GoalNode(goal, decisions);

            int i = decisions.Count - 1;
            foreach (GoalNode neighbor in end.GetNeighbors()) {
                Console.WriteLine(end.GetEdge(neighbor));
                //Assert.AreSame(decisions[i--], end.GetEdge(neighbor));
            }
        }


        [Test]
        public void SingleSolution() {
            //Arrange
            IDecision[] decisions = new IDecision[] {
                new MoveToFridge(),
                new KillFridgeGuardian(3),
                new OpenFridgeDecision(),
                new GetBananaFromFridge()
            };

            ActionPlanner planner = new ActionPlanner(StateOffset.Heuristic);
            for (int i = 0; i < decisions.Length; i++) {
                planner.AddDecision(decisions[i]);
            }

            StateOffset goal = new StateOffset();
            goal.SetStateValue("HasBanana", true, false);

            Queue<IDecision> plan = planner.CreatePlan(goal);
            
            Assert.AreSame(decisions[0], plan.Dequeue()); // MoveToFridge
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGuardian
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGUardian
            Assert.AreSame(decisions[1], plan.Dequeue()); // KillFridgeGuardian
            Assert.AreSame(decisions[2], plan.Dequeue()); // OpenFridge
            Assert.AreSame(decisions[3], plan.Dequeue()); // GetBananaFromFridge

        }
    }
}
