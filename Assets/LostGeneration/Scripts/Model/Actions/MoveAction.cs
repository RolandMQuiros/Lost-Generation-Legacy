using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public class MoveAction : CombatantAction {
        public class Message : MessageArgs {
            public Pawn Mover;
            public Point From;
            public Point To;
            public bool IsContinuous;

            public Message(Pawn mover, Point from, Point to, bool isContinuous) {
                Mover = mover;
                From = from;
                To = to;
                IsContinuous = isContinuous;

                Text = Mover.Name + " moved from " + From + " to " + To;
            }
        }

        public override int ActionPoints { get { return Point.TaxicabDistance(Owner.Position, _destination); } }

        private Point _destination;
        private bool _isContinuous;

        public MoveAction(Combatant owner, Point destination, bool isContinuous) 
            : base(owner) {
            _destination = destination;
            _isContinuous = isContinuous;
        }

        public override void Run() {
            Point start = Owner.Position;

            if (Owner.SetPosition(_destination)) {
                SendMessage(
                    new Message(Owner, start, _destination, _isContinuous)
                );
            }
        }
    }
}
