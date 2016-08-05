using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Actions {
    public class Move : Action {
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

        private Point _destination;
        private bool _isContinuous;

        public Move(Pawn owner, Point destination, bool isContinuous) 
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
