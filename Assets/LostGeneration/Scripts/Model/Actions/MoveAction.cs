using System;

namespace LostGen {
    public class MoveAction : PawnAction {
        public override int Cost { get { return _cost; } }
        public Point Start { get { return _start; } }
        public Point End { get { return _end; } }
        public bool IsContinuous{ get { return _isContinuous; } }
        
        private Point _start;
        private Point _end;
        private int _cost;
        private bool _isContinuous;
        private bool _moveSuccess;

        public MoveAction(Pawn owner, Point start, Point end, int cost, bool isContinuous) 
            : base(owner) {
            _start = start;
            _end = end;
            _cost = cost;
            _isContinuous = isContinuous;
        }

        public override bool Do() {
            _moveSuccess = Owner.SetPosition(_end);
            return _moveSuccess;
        }

        public override void Undo() {
            Owner.SetPositionInternal(_start);
        }

        public override void Commit() {
            if (_moveSuccess) {
                Owner.PushMessage(new MoveMessage(Owner, _start, _end, _isContinuous));
            }
        }

        public override string ToString() {
            return "Move Action: { start: " + _start + "; end: " + _end + "; cost: " + Cost + " }";
        }
    }
}
