using System;

namespace LostGen.Model {
    public class Health : PawnComponent {
        public int Current {
            get { return _current; }
            set {
                _current = Math.Max(0, Math.Min(Maximum, value));
            }
        }
        public int Maximum {
            get { return _max; }
            set {
                _max = value;
                _current = Math.Min(_current, _max);
            }
        }

        private int _current;
        private int _max;

        public Health(int max) {
            _current = _max = max;
        }
    }

}