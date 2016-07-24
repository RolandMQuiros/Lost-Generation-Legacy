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
            public bool IsContinuous;

            public Message(Pawn target, Point from, Point to, bool isContinuous) {
                Target = target;
                From = from;
                To = to;
                IsContinuous = isContinuous;

                Text = Target.Name + " moved from " + From + " to " + To;
            }
        }

        private Point _destination;
        private bool _isContinuous;

        public Move(Pawn target, Point destination, bool isContinuous) 
            : base(target) {
            _destination = destination;
            _isContinuous = isContinuous;
        }

        public override void Run() {
            Point start = Target.Position;

            if (Target.SetPosition(_destination)) {
                SendMessage(
                    new Message(Target, start, _destination, _isContinuous)
                );
            }
        }


    }
}
