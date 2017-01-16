using System;

namespace LostGen {
    public class MoveMessage : MessageArgs {
        public Point From;
        public Point To;
        public bool IsContinuous;

        public MoveMessage(Pawn mover, Point from, Point to, bool isContinuous) {
            Source = mover;
            From = from;
            To = to;
            IsContinuous = isContinuous;

            Text = Source.Name + " moved from " + From + " to " + To;
        }
    }
}