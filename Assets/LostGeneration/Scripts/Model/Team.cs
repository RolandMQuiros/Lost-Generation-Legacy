using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostGen {
    public struct Team {
        private long _team;
        private long _friendly;
        private long _neutral;
        private long _hostile;

        public Team(long team, long friendly, long neutral, long hostile) {
            _team = team;
            _friendly = friendly;
            _neutral = neutral;
            _hostile = hostile;
        }

        public bool IsFriendly(Team other) {
            return ((_team | _friendly) & other._team) == other._team;
        }

        public bool IsNeutral(Team other) {
            return ((_team | _neutral) & other._team) == other._team;
        }

        public bool IsHostile(Team other) {
            return ((_team | _hostile) & other._team) == other._team;
        }
    }
}