using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LostGen {
    public class StateOffset {
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
            public ValueUnion Default;
            
            static readonly StateValue Empty = new StateValue() { Active = false };
        }
        
        private Dictionary<string, StateValue> _stateValues;

        public StateOffset(StateOffset offset = null) {
            if (offset == null) {
                _stateValues = new Dictionary<string, StateValue>();
            } else {
                _stateValues = new Dictionary<string, StateValue>(offset._stateValues);
            }
        }

        public void SetStateValue(string key, int value, int defaultValue) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Int;
            stateValue.Value.IntValue = value;
            stateValue.Default.IntValue = defaultValue;

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, float value, float defaultValue) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Float;
            stateValue.Value.FloatValue = value;
            stateValue.Default.FloatValue = defaultValue;

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, bool value, bool defaultValue) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Bool;
            stateValue.Value.BoolValue = value;
            stateValue.Default.BoolValue = defaultValue;

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, Point value, Point defaultValue) {
            StateValue stateValue = new StateValue();
            stateValue.Active = true;
            stateValue.Type = StateValue.ValueType.Point;
            stateValue.Value.PointValue = value;
            stateValue.Default.PointValue = defaultValue;

            _stateValues[key] = stateValue;
        }

        public int GetStateValue(string key, int defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.IntValue;
            } else {
                return defaultValue;
            }
        }

        public float GetStateValue(string key, float defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.FloatValue;
            } else {
                return defaultValue;
            }
        }

        public bool GetStateValue(string key, bool defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.BoolValue;
            } else {
                return defaultValue;
            }
        }

        public Point GetStateValue(string key, Point defaultValue) {
            StateValue stateValue;  
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.Value.PointValue;
            } else {
                return defaultValue;
            }
        }

        public bool IsSubset(StateOffset other) {
            bool isSubset = true;
            foreach (string key in _stateValues.Keys) {
                StateValue value = _stateValues[key];
                StateValue otherVal;
                other._stateValues.TryGetValue(key, out otherVal);


                if ((otherVal.Active && !value.Value.Equals(otherVal.Value)) ||
                    (!otherVal.Active && !value.Value.Equals(value.Default))) {
                    isSubset = false;
                    break;
                }
            }

            return isSubset;
        }

        public static int Heuristic(StateOffset s1, StateOffset s2) {
            IEnumerable<string> intersection = s1._stateValues.Keys.Intersect(s2._stateValues.Keys);
            int difference = 0;

            foreach (string key in intersection) {
                StateValue sv1 = s1._stateValues[key];
                StateValue sv2 = s2._stateValues[key];

                ValueUnion v1 = sv1.Active ? sv1.Value : sv1.Default;
                ValueUnion v2 = sv2.Active ? sv2.Value : sv2.Default;

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

        public static string CombatantKey(Combatant combatant, string append) {
            return combatant.InstanceID + append;
        }

        public static string CombatantHealthKey(Combatant combatant) {
            return combatant.InstanceID + "health";
        }
    }
}
