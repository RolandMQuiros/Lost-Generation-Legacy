using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public class WorldState : Dictionary<string, object> {
        public WorldState() { }
        public WorldState(IDictionary<string, object> dictionary) : base(dictionary) { }
        public WorldState(IEnumerable<KeyValuePair<string, object>> pairs) {
            foreach (KeyValuePair<string, object> pair in pairs) {
                Add(pair.Key, pair.Value);
            }
        }

        public object Get(string key, object defaultValue) {
            object value;
            if (!TryGetValue(key, out value)) {
                value = defaultValue;
            }
            return value;
        }

        public bool IsSubsetOf(WorldState other) {
            if (Count > 0) {
                foreach (KeyValuePair<string, object> pair in this) {
                    object otherValue;
                    if (!other.TryGetValue(pair.Key, out otherValue) || !otherValue.Equals(pair.Value)) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static WorldState operator + (WorldState s1, WorldState s2) {
            WorldState sum = new WorldState(s1);

            foreach (string key in s2.Keys) {
                sum[key] = s2[key];
            }

            return sum;
        }

        public override string ToString() {
            string buffer = "{ ";
            foreach (KeyValuePair<string, object> pair in this) {
                buffer += "{ " + pair.Key + " : " + pair.Value + " } ";
            }
            buffer += "}";

            return buffer;
        }
    }
}