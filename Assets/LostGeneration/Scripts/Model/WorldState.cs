using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public class WorldState : HashSet<KeyValuePair<string, object>> {
        public WorldState() { }
        public WorldState(IEnumerable<KeyValuePair<string, object>> other) : base (other) { }
        
        public bool Add(string key, object value) {
            return Add(new KeyValuePair<string, object>(key, value));
        }

        public static WorldState operator + (WorldState s1, WorldState s2) {
            return new WorldState(s1.Union(s2));
        }

        public static WorldState operator * (WorldState s1, WorldState s2) {
            // Select the difference between s1 and s2, plus the pairs in s2 that have keys
            // in s1
            return new WorldState(
                s1.Except(s2)
                  .Union(
                      s1.Join(
                          s2,
                          left => left.Key,
                          right => right.Key,
                          (left, right) => right
                      )
                  )
            );
        }

        public override string ToString() {
            string buffer = "{ ";
            foreach (KeyValuePair<string, object> pair in this) {
                buffer += "{ " + pair.Key + " : " + pair.Value.ToString() + " } ";
            }
            buffer += "}";

            return buffer;
        }
    }
}