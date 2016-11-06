using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class MoveAction : CombatantAction {
        public class Message : MessageArgs {
            public Point From;
            public Point To;
            public bool IsContinuous;

            public Message(Pawn mover, Point from, Point to, bool isContinuous) {
                Source = mover;
                From = from;
                To = to;
                IsContinuous = isContinuous;

                Text = Source.Name + " moved from " + From + " to " + To;
            }
        }

        public override int ActionPoints { get { return Point.TaxicabDistance(_start, _end); } }
        public override Point PostRunPosition {
            get { return _end; }
        }

        private Point _start;
        private Point _end;
        private bool _isContinuous;
        private bool _moveSuccess;

        public MoveAction(Combatant owner, Point start, Point end, bool isContinuous) 
            : base(owner) {
            _start = start;
            _end = end;
            _isContinuous = isContinuous;
        }

        public override void Do() {
            Owner.ActionPoints -= ActionPoints;
            _moveSuccess = Owner.SetPosition(_end);
        }

        public override void Undo() {
            Owner.ActionPoints += ActionPoints;
            Owner.SetPositionInternal(_start);
        }

        public override void React() {
            if (_moveSuccess) {
                SendMessage(
                    new Message(Owner, _start, _end, _isContinuous)
                );
            }
        }

        public override string ToString() {
            return "Move Action: { start: " + _start + "; end: " + _end + "; cost: " + ActionPoints + " }";
        }
    }
}
