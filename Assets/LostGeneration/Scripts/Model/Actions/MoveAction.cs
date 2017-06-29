namespace LostGen {
    public class MoveAction : PawnAction {
        public override int Cost { get { return Point.TaxicabDistance(_start, _end); } }
        public Point Start { get { return _start; } }
        public Point End { get { return _end; } }
        public bool IsContinuous{ get { return _isContinuous; } }
        
        private Point _start;
        private Point _end;
        private bool _isContinuous;
        private bool _moveSuccess;

        private Combatant _combatant;

        public MoveAction(Pawn owner, Point start, Point end, bool isContinuous) 
            : base(owner) {
            _start = start;
            _end = end;
            _isContinuous = isContinuous;
            _combatant = Owner.GetComponent<Combatant>();
        }

        public override void Do() {
            if (_combatant != null) {
                _combatant.ActionPoints -= Cost;
            }
            _moveSuccess = Owner.SetPosition(_end);
        }

        public override void Undo() {
            if (_combatant != null) {
                _combatant.ActionPoints += Cost;
            }
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
