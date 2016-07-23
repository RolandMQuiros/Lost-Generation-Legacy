using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Actions {
    public class Move : Action {
        public class Message : MessageArgs {
            public Pawn Target;
            public Point From;
            public Point To;

            public Message(Pawn target, Point from, Point to) {
                Target = target;
                From = from;
                To = to;

                Text = Target.Name + " moved from " + From + " to " + To;
            }
        }

        private Point _destination;

        public Move(Pawn target, Point destination) 
            : base(target) {
            _destination = destination;
        }

        public override void Run() {
            Point start = Target.Position;

            if (Target.SetPosition(_destination)) {
                SendMessage(
                    new Message(Target, start, _destination)
                );
            }
        }


    }
}
