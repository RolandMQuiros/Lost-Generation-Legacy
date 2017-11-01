using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using LostGen;

namespace Tests.Integration {
    public class CompoundTaskTests {
        private class Wallet {
            public int Money = 0;
            public int Things = 0;
        }

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

            public override IEnumerator Do(WorldState start, WorldState goal) {
                Console.WriteLine("Went to the forest");
                yield break;
            }
        }
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

            public override IEnumerator Do(WorldState start, WorldState goal) {
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

            public override IEnumerator Do(WorldState start, WorldState goal) {
                Console.WriteLine("Went to city");
                yield break;
            }
        }

        private class SellWood : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }
            private Wallet _wallet;

            public SellWood(Wallet wallet) {
                _preconditions = new WorldState() {
                    { "Has Wood", true },
                    { "In City", true },
                    { "Has Money", false }
                };

                _postconditions = new WorldState() {
                    { "Has Money", true }
                };
                
                _wallet = wallet;
            }

            public override bool ArePreconditionsMet() { return true; }

            public override IEnumerator Do(WorldState start, WorldState goal) {
                Console.WriteLine("Sold wood");
                _wallet.Money++;
                yield break;
            }
        }

        private class BuyThing : PrimitiveTask {
            private WorldState _preconditions, _postconditions;
            public override WorldState Preconditions { get { return _preconditions; } }
            public override WorldState Postconditions { get { return _postconditions; } }
            private Wallet _wallet;

            public BuyThing(Wallet wallet) {
                _preconditions = new WorldState() {
                    { "Has Money", true },
                    { "Has Thing", false }
                };

                _postconditions = new WorldState() {
                    { "Has Money", false },
                    { "Has Thing", true }
                };
                
                _wallet = wallet;
            }

            public override bool ArePreconditionsMet() { return true; }

            public override IEnumerator Do(WorldState start, WorldState goal) {
                if (_wallet.Money >= 10) {
                    _wallet.Money -= 10;
                    _wallet.Things++;
                    Console.WriteLine("Bought Thing");
                } else {
                    yield return null;
                }
            } 
        }
        
        [Test]
        public void PathingAndReplanning() {
            Wallet wallet = new Wallet();
            CompoundTask planner = new CompoundTask((from, to) => 1) {
                new GoToForest(),
                new GoToCity(),
                new ChopWood(),
                new SellWood(wallet),
                new BuyThing(wallet)
            };


            // Starting state is generated by sensors, and should be fairly comprehensive and consistent
            // with those of tasks.
            WorldState start = new WorldState() {
                { "In City", true },
                { "In Forest", false },
                { "Has Axe", true },
                { "Has Wood", false },
                { "Has Money", false },
                { "Has Thing", false }
            };

            WorldState goal = new WorldState() {
                { "Has Thing", true }
            };

            Console.WriteLine("Planner preconditions: " + planner.Preconditions);
            Console.WriteLine("Planner postconditions: " + planner.Postconditions);

            IEnumerator plan = planner.Do(start, goal);
            while (plan.MoveNext()) {
                Console.Write("Money: " + wallet.Money + "; ");
                if (plan.Current == null) {
                    plan = planner.Do(start, goal);
                }
            }

            Assert.AreEqual(1, wallet.Things);
        }

        [Test]
        public void NestedCompoundTask() {
            Wallet wallet = new Wallet();
            CompoundTask getMoney = new CompoundTask((from, to) => 1) {
                new GoToForest(),
                new GoToCity(),
                new ChopWood(),
                new SellWood(wallet)
            };

            CompoundTask getThing = new CompoundTask((from, to) => 1) {
                getMoney,
                new BuyThing(wallet)
            };

            WorldState start = new WorldState() {
                { "In City", true },
                { "In Forest", false },
                { "Has Axe", true },
                { "Has Wood", false },
                { "Has Money", false },
                { "Has Thing", false }
            };

            WorldState goal = new WorldState() {
                { "Has Thing", true }
            };

            Console.WriteLine("getMoney preconditions: "  + getMoney.Preconditions);
            Console.WriteLine("getMoney postconditions: " + getMoney.Postconditions);

            Console.WriteLine("getThing preconditions: " + getThing.Preconditions);
            Console.WriteLine("getThing postconditions: " + getThing.Postconditions);

            IEnumerator plan = getThing.Do(start, goal);
            while (plan.MoveNext()) {
                if (plan.Current == null) {
                    Console.WriteLine("Plan failed. Rerouting.");
                    plan = getThing.Do(start, goal);
                }
            }
        }
    }
}