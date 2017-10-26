using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LostGen;

namespace Tests.Integration {
    public class CompoundTaskTests {
        private class GoToForest : PrimitiveTask {
            public override StateOffset ApplyPreconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "In Forest", false },
                    { "In City", true }
                };
            }

            public override bool ArePreconditionsMet(StateOffset state) {
                return !state.Get("In Forest", false) &&
                       state.Get("In City", false) &&
                       state.Get("Money", 0) < 10;
            }

            public override StateOffset ApplyPostconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "In Forest", true },
                    { "In City", false }
                };
            }

            public override void Do() {
                Console.WriteLine("Went to the forest");
            }
        }

        private class ChopWood : PrimitiveTask {
            public override StateOffset ApplyPreconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "In Forest", true },
                    { "Has Axe", true },
                    { "Has Wood", false }
                };
            }

            public override bool ArePreconditionsMet(StateOffset state) {
                return state.Get("In Forest", false) &&
                       state.Get("Has Axe", false) &&
                       !state.Get("Has Wood", false);
            }

            public override StateOffset ApplyPostconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "Has Wood", true }
                };
            }

            public override void Do() {
                Console.WriteLine("Chopped wood");
            }
        }

        private class GoToCity : PrimitiveTask {
            public override StateOffset ApplyPreconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "In City", false },
                    { "In Forest", true }
                };
            }

            public override bool ArePreconditionsMet(StateOffset state) {
                return !state.Get("In City", false) &&
                       state.Get("In Forest", true);
            }

            public override StateOffset ApplyPostconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "In City", true },
                    { "In Forest", false }
                };
            }

            public override void Do() {
                Console.WriteLine("Went to city");
            }
        }

        private class SellWood : PrimitiveTask {
            public override StateOffset ApplyPreconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "Has Wood", true },
                    { "In City", true },
                    { "Money", Math.Max(state.Get("Money", 0) - 1, 0) }
                };
            }

            public override bool ArePreconditionsMet(StateOffset state) {
                return state.Get("Has Wood", false) &&
                       state.Get("In City", false) &&
                       state.Get("Money", 0) < 10;
            }

            public override StateOffset ApplyPostconditions(StateOffset state) {
                return state + new StateOffset() {
                    { "Has Wood", false },
                    { "Money", state.Get("Money", 0) + 1 }
                };
            }

            public override void Do() {
                Console.WriteLine("Sold wood");
            }
        }
        
        [Test]
        public void Pathing() {
            CompoundTask planner = new CompoundTask((from, to) => 1);
            planner.AddSubtask(new GoToForest());
            planner.AddSubtask(new GoToCity());
            planner.AddSubtask(new ChopWood());
            planner.AddSubtask(new GoToCity());
            planner.AddSubtask(new SellWood());

            StateOffset start = new StateOffset() {
                { "In City", true },
                { "Has Axe", true },
                { "Has Wood", false },
                { "Money", 0 }
            };

            StateOffset goal = new StateOffset() {
                { "Money", 10 }
            };
            
            List<ITask> plan = new List<ITask>(planner.Decompose(start, goal));
            Assert.Greater(plan.Count, 0);
            for (int i = 0; i < plan.Count; i++) {
                ((PrimitiveTask)plan[i]).Do();
            }
        }
    }
}