using System;

namespace LostGen.Model {
    public class MoveMessage : IPawnMessage {
        public Pawn Source { get; private set; }
        public Pawn Target { get; private set; }
        public String Text { get; private set; }
        public bool IsCritical {get; private set; }
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