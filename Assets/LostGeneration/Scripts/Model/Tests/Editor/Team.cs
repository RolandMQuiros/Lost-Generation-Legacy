using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using LostGen;

namespace Tests {
    public class TeamTests {

        [Test]
        public void SameTeam() {
            Team teamA = new Team(1, 0, 0, 0);
            Team teamB = new Team(1, 0, 0, 0);

            Assert.IsTrue(teamA.IsFriendly(teamB));
            Assert.IsTrue(teamB.IsFriendly(teamA));
        }
    }
}