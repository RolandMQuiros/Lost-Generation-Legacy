using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostGen.Test {
    /*
    public class PlannerTests {
        private class TestDecision : DecisionNode {
            public int Value;
            public int Cost;

            public override bool ArePreconditionsMet(StateOffset state) {
                int val = state.GetStateValue("value", -1);
                return val == Value - 1;
            }

            public override StateOffset GetPostconditions(StateOffset state = null) {
                state.SetStateValue("value", Value);
                return state;
            }

            public override int GetCost(StateOffset state = null) {
                return Value;
            }
        }

        private class OddDecision : TestDecision {
            public override bool ArePreconditionsMet(StateOffset state) {
                int val = state.GetStateValue("value", -1);
                return val < Value && val % 2 == 1;
            }
        }

        private class EvenDecision : TestDecision {
            public override bool ArePreconditionsMet(StateOffset state) {
                int val = state.GetStateValue("value", -1);
                return val < Value && val % 2 == 0;
            }
        }

        private void PrintGraphList(List<DecisionNode> graphList) {
            string print = string.Empty;
            for (int i = 0; i < graphList.Count; i++) {
                TestDecision decision = graphList[i] as TestDecision;

                if (decision != null) {
                    print += decision.Value + ": ";
                    foreach (DecisionNode neighbor in decision.GetNeighbors()) {
                        TestDecision neighborTest = neighbor as TestDecision;
                        if (neighborTest != null) {
                            print += neighborTest.Value + " ";
                        } else {
                            print += "start ";
                        }
                    }
                } else {
                    print += "start ";
                }

                print += "\n";
            }
            Console.Write(print);
        }

        [Test]
        public void PreconditionTest() {
            TestDecision[] decisions = new TestDecision[3];
            for (int i = 0; i < 3; i++) {
                decisions[i] = new TestDecision() { Value = i };
            }

            StateOffset state = new StateOffset();

            for (int i = 0; i < decisions.Length; i++) {
                Assert.IsTrue(decisions[i].ArePreconditionsMet(state));
                decisions[i].GetPostconditions(state);
            }
        }

        [Test]
        public void GraphBuild() {
            Planner planner = new Planner();

            TestDecision[] decisions = new TestDecision[10];
            for (int i = 0; i < 10; i++) {
                decisions[i] = new TestDecision() { Value = i };
                planner.AddDecision(decisions[i]);
            }

            planner.BuildGraph();

            List<DecisionNode> graphList = new List<DecisionNode>(
                Pathfinder<DecisionNode>.FloodFill(decisions[9])
            );

            PrintGraphList(graphList);

            Assert.AreEqual(11, graphList.Count);
        }

        [Test]
        public void EvenOddGraph() {
            Planner planner = new Planner();

            TestDecision[] decisions = new TestDecision[10];
            for (int i = 0; i < 10; i++) {
                if (i % 2 == 0) {
                    decisions[i] = new EvenDecision() { Value = i };
                } else {
                    decisions[i] = new OddDecision() { Value = i };
                }
                planner.AddDecision(decisions[i]);
            }

            // SHIT do we need to rebuild the graph with every state change?
            // Only if you build the graph using the precondition check outside of isolation
            // When building graph

            // Limit 1 occurrence of a decision per path
            // The goal's precondition should check for progress toward goal, not a hard requirement for the goal state
            // Thus, once the goal's precondition is satisfied, the planner can restart the pathfinding plan?

            // A* doesn't cover repeat visits, so
            // Design Goals as inclusive of progress as well as ultimate success
            // e.g. If the ultimate goal is to kill the enemy, for the DecisionNode EnemyDeadOrDying, ArePreconditionsMet returns true
            // if the Enemy's projected HP is lower than their current HP
            planner.BuildGraph();

            List<DecisionNode> graphList = new List<DecisionNode>(
                Pathfinder<DecisionNode>.FloodFill(decisions[9])
            );

            PrintGraphList(graphList);
        }
    } */
}