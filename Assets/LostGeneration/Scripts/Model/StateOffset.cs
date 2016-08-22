﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LostGen {
    public class StateOffset {
        [StructLayout(LayoutKind.Explicit)]
        private struct StateValue {
            [FieldOffset(0)]
            public bool Active;
            [FieldOffset(sizeof(bool))]
            public float FloatValue;
            [FieldOffset(sizeof(bool))]
            public int IntValue;
            [FieldOffset(sizeof(bool))]
            public bool BoolValue;
            [FieldOffset(sizeof(bool))]
            public Point PointValue;

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

        public void SetStateValue(string key, int value) {
            StateValue stateValue = new StateValue() {
                Active = true,
                IntValue = value
            };

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, float value) {
            StateValue stateValue = new StateValue() {
                Active = true,
                FloatValue = value
            };

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, bool value) {
            StateValue stateValue = new StateValue() {
                Active = true,
                BoolValue = value
            };

            _stateValues[key] = stateValue;
        }

        public void SetStateValue(string key, Point value) {
            StateValue stateValue = new StateValue() {
                Active = true,
                PointValue = value
            };

            _stateValues[key] = stateValue;
        }

        public int GetStateValue(string key, int defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.IntValue;
            } else {
                return defaultValue;
            }
        }

        public float GetStateValue(string key, float defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.FloatValue;
            } else {
                return defaultValue;
            }
        }

        public bool GetStateValue(string key, bool defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.BoolValue;
            } else {
                return defaultValue;
            }
        }

        public Point GetStateValue(string key, Point defaultValue) {
            StateValue stateValue;
            _stateValues.TryGetValue(key, out stateValue);
            if (stateValue.Active) {
                return stateValue.PointValue;
            } else {
                return defaultValue;
            }
        }

        public static string CombatantKey(Combatant combatant, string append) {
            return combatant.ID + append;
        }

        public static string CombatantHealthKey(Combatant combatant) {
            return combatant.ID + "health";
        }
    }
}