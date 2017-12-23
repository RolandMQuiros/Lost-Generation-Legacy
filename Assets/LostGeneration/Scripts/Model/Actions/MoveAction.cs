using System;

namespace LostGen.Model {
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
            Owner.Board.Pawns.Move(Owner, _end);
            return true;
        }

        public override void Undo() {
            Owner.Board.Pawns.Move(Owner, _start);
        }

        public override bool Commit() {
            bool moved = Owner.SetPosition(_end);
            if (moved) {
                Owner.PushMessage(new MoveMessage(Owner, _start, _end, _isContinuous));
            }
            return moved;
        }

        public override string ToString() {
            return "Move Action: { start: " + _start + "; end: " + _end + "; cost: " + Cost + " }";
        }
    }
}
