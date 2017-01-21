using System;
using System.Collections.Generic;

namespace LostGen {
    public class FallMessage : IPawnMessage {
        public Pawn Source { get; private set; }
        public Pawn Target { get; private set; }
        public string Text { get; private set; }
        public bool IsCritical { get; private set; }
        
        public Point From { get; private set; }
        public Point To { get; private set; }

        public IEnumerable<Pawn> LandedOn {
            get { return _landedOn.AsReadOnly(); }
        }

        private List<Pawn> _landedOn;

        public FallMessage(Pawn faller, Point from, Point to, IEnumerable<Pawn> landedOn = null) {
            Source = faller;
            From = from;
            To = to;
            if (landedOn != null) {
                _landedOn = new List<Pawn>(landedOn);
            } else {
                _landedOn = new List<Pawn>();
            }

            int distance = Point.TaxicabDistance(From, To);
            Text = faller.Name + " fell " + distance + " meters";
            if (_landedOn.Count > 0) {
                Text += ", landing on ";
                for (int i = 0; i < _landedOn.Count; i++) {
                    Text += _landedOn[i].Name;
                    if (i < _landedOn.Count - 2) {
                        Text += ", ";
                    } else if (i == _landedOn.Count - 2) {
                        Text += ", and ";
                    }
                }
            }
        }
    }
}