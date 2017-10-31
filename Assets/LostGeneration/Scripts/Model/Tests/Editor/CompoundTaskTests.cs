using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LostGen;

namespace Tests.Integration {
    public class CompoundTaskTests {
        private class GoToForest : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }

            public GoToForest() {
                _preconditions = new WorldState() {
                    { "In Forest", false },
                    { "In City", true }
                };

                _postconditions = new WorldState() {
                    { "In Forest", true },
                    { "In City", false }
                };
            }

            public override bool ArePreconditionsMet() { return true; }

            public override IEnumerator Do(WorldState goal) {
                Console.WriteLine("Went to the forest");
                yield break;
            }
        }
GoToForest, ChopWood, GoToCity, SellWood
        private class ChopWood : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }

            public ChopWood() {
                _preconditions = new WorldState() {
                    { "In Forest", true },
                    { "Has Axe", true },
                    { "Has Wood", false }
                };

                _postconditions = new WorldState() {
                    { "Has Wood", true }
                };
            }

            public override bool ArePreconditionsMet() { return true; }

            public override IEnumerator Do(WorldState goal) {
                Console.WriteLine("Chopped wood");
                yield break;
            }
        }

        private class GoToCity : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }

            public GoToCity() {
                _preconditions = new WorldState() {
                    { "In City", false },
                    { "In Forest", true }
                };

                _postconditions = new WorldState() {
                    { "In City", true },
                    { "In Forest", false }
                };
            }

            public override bool ArePreconditionsMet() { return true; }

            public override IEnumerator Do(WorldState goal) {
                Console.WriteLine("Went to city");
                yield break;
            }
        }

        private class SellWood : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }
            private object _money;

            public SellWood(object money) {
                _preconditions = new WorldState() {
                    { "Has Wood", true },
                    { "In City", true },
                    { "Has Money", false }
                };

                _postconditions = new WorldState() {
                    { "Has Money", true }
                };
                
                _money = money;
            }

            public override bool ArePreconditionsMet() { return (int)_money < 10; }

            public override IEnumerator Do(WorldState goal) {
                Console.WriteLine("Sold wood");
                yield break;
            }
        }
        
        [Test]
        public void Pathing() {
            CompoundTask planner = new CompoundTask((from, to) => 1);
            int money = 0;

            planner.AddSubtask(new GoToForest());
            planner.AddSubtask(new GoToCity());
            planner.AddSubtask(new ChopWood());
            planner.AddSubtask(new GoToCity());
            planner.AddSubtask(new SellWood(money));

            WorldState start = new WorldState() {
                { "In City", true },
                { "In Forest", false },
                { "Has Axe", true },
                { "Has Wood", false },
            };

            WorldState goal = new WorldState() {
                { "Has Money", true }
            };

            Console.WriteLine(planner.Preconditions);

            IEnumerator plan = planner.Do(start, goal);
            while (plan.MoveNext());
        }
    }
}