using System;

namespace LostGen.Model {
    public class ActionPoints : PawnComponent, IObservable<ActionPoints> {
        public int Current {
            get { return _current; }
            set { _current = value; }
        }

        public int Max {
            get { return _max; }
            set { _max = value; }
        }

        public event Action<ActionPoints> Changed; 
        private void Notify() { if (Changed != null) { Changed(this); } }

        private int _current;
        private int _max;

        public ActionPoints(int max) {
            _current = _max = max;
        }

        public override void BeginTurn() {
            _current = Math.Max(_current, _max);
        }

        public override void PostAction(PawnAction action) {
            _current -= action.Cost;
        }
    }
}