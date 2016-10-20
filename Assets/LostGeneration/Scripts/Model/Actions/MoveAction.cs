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

        public override int ActionPoints { get { return Point.TaxicabDistance(Owner.Position, _destination); } }
        public override Point PostRunPosition {
            get { return _destination; }
        }

        private Point _start;
        private Point _destination;
        private bool _isContinuous;
        private bool _moveSuccess;

        public MoveAction(Combatant owner, Point destination, bool isContinuous) 
            : base(owner) {
            _destination = destination;
            _isContinuous = isContinuous;
        }

        public override void Do() {
            _start = Owner.Position;
            _moveSuccess = Owner.SetPosition(_destination);
        }

        public override void Undo() {
            Owner.SetPositionInternal(_start);
        }

        public override void React() {
            if (_moveSuccess) {
                SendMessage(
                    new Message(Owner, _start, _destination, _isContinuous)
                );
            }
        }
    }
}
