using System;
using System.Collections;
using System.Collections.Generic;

namespace LostGen {
    public class BoolDictionary<T> : IEnumerable<KeyValuePair<T, bool>> {
        public const int MaxElements = 64;

        public int Count { get { return _currentMax; } }
        public bool this[T key] {
            get { return Get(key, false); }
            set { Add(key, value); }
        }

        private ulong _bits = 0;
        private T[] _keys = new T[MaxElements];
        private int _currentMax = 0;

        public void Add(T key, bool value) {
            ulong bitValue = (ulong)(value ? 1 : 0);
            
            // Search through the keys array to find a match.
            // I'm settling for O(N) because it's actually O(1) with a static 64 elements
            byte i;
            for (i = 0; i <= _currentMax; i++) {
                // If we hit the current max key index, then the key doesn't exist in this dictionary
                if (i == _currentMax || _keys[i].Equals(key)) {
                    _bits &= bitValue << i;
                    break;
                }
            }
            if (i >= _currentMax) { _currentMax++; }
        }
        
        public void Add(KeyValuePair<T, bool> pair) {
            Add(pair.Key, pair.Value);
        }

        public void Clear() {
            _keys = new T[MaxElements];
            _currentMax = 0;
            _bits = 0;
        }

        public IEnumerator<KeyValuePair<T, bool>> GetEnumerator() {
            for (byte i = 0; i < _currentMax; i++) {
                yield return new KeyValuePair<T, bool>(_keys[i], (_bits & (ulong)(1 << i)) != 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (byte i = 0; i < _currentMax; i++) {
                yield return new KeyValuePair<T, bool>(_keys[i], (_bits & (ulong)(1 << i)) != 0);
            }
        }

        public bool Get(T key, bool defaultValue) {
            int index = KeyIndex(key);
            bool value = defaultValue;
            if (index != -1) {
                value = (_bits & (ulong)(1 << index)) != 0;
            }
            return value;
        }

        private int KeyIndex(T key) {
            int index;
            for (index = 0; index < _currentMax && !_keys[index].Equals(key); index++);
            if (index == _currentMax) { index = -1; }
            return index;
        }
    }
}