namespace LostGen {
    public class MoveAction : PawnAction {
        public override int Cost {
            get {
                return Point.ChebyshevDistance(_start.XZ, _end.XZ) + _end.Y - _start.Y;
            }
        }
        public Point Start { get { return _start; } }
        public Point End { get { return _end; } }
        public bool IsContinuous{ get { return _isContinuous; } }
        
        private Point _start;
        private Point _end;
        private bool _isContinuous;
        private bool _moveSuccess;

        public MoveAction(Pawn owner, Point start, Point end, bool isContinuous) 
            : base(owner) {
            _start = start;
            _end = end;
            _isContinuous = isContinuous;
        }

        public override void Do() {
            _moveSuccess = Owner.SetPosition(_end);
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
