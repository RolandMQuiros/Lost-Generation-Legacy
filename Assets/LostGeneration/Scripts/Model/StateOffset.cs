using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LostGen {
    /// <summary>
    /// A Dictionary of world state values, representing potential outcomes of <see cref="IDecisions"/>.
    /// </summary>
    /// <remarks>
    /// StateOffsets are used by <see cref="Planner"/>s as nodes in a graph connected by IDecisions. A StateOffset
    /// represents a world state that is 'offset' from the current one. Internally, a StateOffset is a
    /// <see cref="Dictionary"/> keyed by <see cref="string"/>s, holding values of floats, integers, booleans, or
    /// <see cref="Point"/>s.
    /// </remarks>
    public class StateOffset : IEnumerable {
        [StructLayout(LayoutKind.Explicit)]
        private struct ValueUnion {
            [FieldOffset(0)]
            public float FloatValue;
            [FieldOffset(0)]
            public int IntValue;
            [FieldOffset(0)]
            public bool BoolValue;
            [FieldOffset(0)]
            public Point PointValue;
        }

        private struct StateValue {
            public enum ValueType {
                Float,
                Int,
                Bool,
                Point
            }

            public bool Active;
            public ValueType Type;
            public ValueUnion Value;            
            static readonly StateValue Empty = new StateValue() { Active = false };
        }
        
        private Dictionary<string, StateValue> _stateValues;

        /// <summary>
        /// Create a new StateOffset
        /// </summary>
        /// <param name="offset">
        /// Another StateOffset. This offset's key-value pairs are added to the new StateOffset being created
        /// </param>
        public StateOffset(StateOffset offset = null) {
            if (offset == null) {
                _stateValues = new Dictionary<string, StateValue>();
            } else {
                _stateValues = new Dictionary<string, StateValue>(offset._stateValues);
            }
        }

        /// <summary>
        /// Sets a state value to an integer
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="value">integer value</param>
        public void Add(string key, int value) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Int;
            stateValue.Value.IntValue = value;

            _stateValues[key] = stateValue;
        }

        /// <summary>
        /// Sets a state value to a float
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="value">Float value</param>
        public void Add(string key, float value) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Float;
            stateValue.Value.FloatValue = value;

            _stateValues[key] = stateValue;
        }

        /// <summary>
        /// Sets a state value to a boolean
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="value">Boolean value</param>
        public void Add(string key, bool value) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Bool;
            stateValue.Value.BoolValue = value;

            _stateValues[key] = stateValue;
        }

        /// <summary>
        /// Sets a state value to a <see cref="Point"/>
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="value">Point value</param>
        public void Add(string key, Point value) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Point;
            stateValue.Value.PointValue = value;

            _stateValues[key] = stateValue;
        }

        /// <summary>
        /// Retrieve a state value from this StateOffset. If the value doesn't exist, the function will return the
        /// provided <c>defaultValue</c> parameter, which is typically the current world state value of that particular
        /// characteristic.
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="defaultValue">The value returned if the state key doesn't exist in this StateOffset.</param>
        /// <returns>The state value</returns>

        public int Get(string key, int defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.IntValue;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retrieve a state value from this StateOffset. If the value doesn't exist, the function will return the
        /// provided <c>defaultValue</c> parameter, which is typically the current world state value of that particular
        /// characteristic.
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="defaultValue">The value returned if the state key doesn't exist in this StateOffset.</param>
        /// <returns>The state value</returns>
        public float Get(string key, float defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.FloatValue;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retrieve a state value from this StateOffset. If the value doesn't exist, the function will return the
        /// provided <c>defaultValue</c> parameter, which is typically the current world state value of that particular
        /// characteristic.
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="defaultValue">The value returned if the state key doesn't exist in this StateOffset.</param>
        /// <returns>The state value</returns>

        public bool Get(string key, bool defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.BoolValue;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retrieve a state value from this StateOffset. If the value doesn't exist, the function will return the
        /// provided <c>defaultValue</c> parameter, which is typically the current world state value of that particular
        /// characteristic.
        /// </summary>
        /// <param name="key">State key</param>
        /// <param name="defaultValue">The value returned if the state key doesn't exist in this StateOffset.</param>
        /// <returns>The state value</returns>

        public Point Get(string key, Point defaultValue) {
            StateValue stateValue;  
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.PointValue;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Checks if the current StateOffset's values are subset to another.
        /// </summary>
        /// <remarks>
        /// A StateOffset can be subset to another if the other offset includes a later change to some value that wasn't
        /// previously changed. For example, one <see cref="IDecision"/> moves a Pawn to a certain position, creating a
        /// StateOffset with a state value "PawnPosition" set to that position's point <see cref="Point"/>, then a
        /// subsequent IDecision causes the Pawn to attack an enemy, setting state value "EnemyHealth", since the latter
        /// StateOffset will include the original "PawnPosition" state value as well. This tells us the first
        /// StateOffset occurred before the second, and the Planner can form behaviors by building from previous states.   
        /// </remarks>
        /// <param name="other">The other StateOffset</param>
        /// <returns>Whether or not the current StateOffset is subset to the given one</returns>
        public bool IsSubsetOf(StateOffset other) {
            bool isSubset = true;
            foreach (string key in _stateValues.Keys) {
                StateValue value = _stateValues[key];
                StateValue otherVal;
                other._stateValues.TryGetValue(key, out otherVal);

                if (!otherVal.Active || (otherVal.Active && !value.Value.Equals(otherVal.Value))) {
                    isSubset = false;
                    break;
                }
            }

            return isSubset;
        }

        public IEnumerator GetEnumerator() {
            return _stateValues.GetEnumerator();
        }

        /// <summary>
        /// Returns a JSON-like string representation of this StateOffset. 
        /// </summary>
        /// <returns>String representation of this StateOffset</returns>
        public override string ToString() {
            string buffer = "{ ";
            foreach (KeyValuePair<string, StateValue> pair in _stateValues) {
                buffer += "{ " + pair.Key + " : ";
                switch (pair.Value.Type) {
                    case StateValue.ValueType.Float:
                        buffer += pair.Value.Value.FloatValue;
                        break;
                    case StateValue.ValueType.Int:
                        buffer += pair.Value.Value.IntValue;
                        break;
                    case StateValue.ValueType.Bool:
                        buffer += pair.Value.Value.BoolValue;
                        break;
                    case StateValue.ValueType.Point:
                        buffer += pair.Value.Value.PointValue;
                        break;
                }
                buffer += " } ";
            }
            buffer += "}";

            return buffer;
        }

        /// <summary>
        /// A rough estimate of "difference" between one state and another, measuring how drastic the changes were.
        /// </summary>
        /// <remarks>
        /// Calculates an integer value based on absolute difference between each corresponding value in the two
        /// offsets. For floats and integer state values, this is simply the absolute value of their arithmetic
        /// difference. For booleans, it's the logical difference. And for <see cref="Point"/>s, it's the Taxicab
        /// distance.
        /// </remarks>
        /// <param name="s1">First StateOffset</param>
        /// <param name="s2">Second StateOffset</param>
        /// <returns>An integer estimation of difference between the two states</returns>
        public static int Heuristic(StateOffset s1, StateOffset s2) {
            IEnumerable<string> intersection = s1._stateValues.Keys.Intersect(s2._stateValues.Keys);
            int difference = 0;

            foreach (string key in intersection) {
                StateValue sv1 = s1._stateValues[key];
                StateValue sv2 = s2._stateValues[key];

                ValueUnion v1 = sv1.Value;
                ValueUnion v2 = sv2.Value;

                if (sv1.Type == sv2.Type) {
                    switch (sv1.Type) {
                        case StateValue.ValueType.Float:
                            difference += (int)Math.Abs(v2.FloatValue - v1.FloatValue + 0.5f);
                            break;
                        case StateValue.ValueType.Int:
                            difference += Math.Abs(v2.IntValue - v1.IntValue);
                            break;
                        case StateValue.ValueType.Bool:
                            difference += (v2.BoolValue ? 1 : 0) - (v1.BoolValue ? 1 : 0);
                            break;
                        case StateValue.ValueType.Point:
                            difference += Point.TaxicabDistance(v1.PointValue, v2.PointValue);
                            break;
                    }
                }
            }

            return difference;
        }

        /// <summary>
        /// Adds all the common state values between two StateOffsets. 
        /// </summary>
        /// <param name="s1">First StateOffset</param>
        /// <param name="s2">Second StateOffset</param>
        /// <returns></returns>
        public static StateOffset operator + (StateOffset s1, StateOffset s2) {
            StateOffset sum = new StateOffset(s1);

            foreach (string key in s2._stateValues.Keys) {
                sum._stateValues[key] = s2._stateValues[key];
            }

            return sum;
        }

        public static StateOffset Intersect(StateOffset s1, StateOffset s2) {
            StateOffset intersection = new StateOffset();
            foreach (string key in s1._stateValues.Keys
                                     .Intersect(s2._stateValues.Keys)
                                     .Where(k => s1._stateValues[k].Value.Equals(s2._stateValues[k].Value))) {
                intersection._stateValues[key] = s1._stateValues[key];
            }
            return intersection;
        }
    }
}
