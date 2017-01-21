using System;
using System.Collections.Generic;

namespace LostGen {
    public class Gravity : PawnComponent {
        public bool HasGravity = true;
        public AxisDirection GravityDirection {
            get { return _gravityDirection; }
            set {
                _gravityDirection = value;
                switch (_gravityDirection) {
                    case AxisDirection.Down:     _gravity = Point.Down;     break;
                    case AxisDirection.Left:     _gravity = Point.Left;     break;
                    case AxisDirection.Up:       _gravity = Point.Up;       break;
                    case AxisDirection.Right:    _gravity = Point.Right;    break;
                    case AxisDirection.Forward:  _gravity = Point.Forward;  break;
                    case AxisDirection.Backward: _gravity = Point.Backward; break;
                }
            }
        }
        public int Weight;
        public event Action<Gravity> LandedUpon;

        private AxisDirection _gravityDirection = AxisDirection.Down;
        private Point _gravity = Point.Down;

        private bool _isFalling = false;
        private List<Pawn> _landedOn;

        public override void OnCollisionEnter(Pawn other) {
            Gravity otherWeight = other.GetComponent<Gravity>();
            if (_isFalling) {
                _landedOn.Add(other);
            } else if (otherWeight._isFalling && LandedUpon != null) {
                LandedUpon(this);
            }
        }

        public override void PostStep() {
            // Move the Pawn in the direction of gravity until it encounters the edge of the 
            if (HasGravity && _gravity != Point.Zero) {
                _isFalling = true;
                _landedOn = new List<Pawn>();
                Point from = Pawn.Position;
                while (Pawn.Offset(_gravity)); // Calls OnCollisionEnter
                Point to = Pawn.Position;

                // If this Pawn actually fell somewhere, emit a message saying from where to where, and
                // on who we landed on
                if (from != to) {
                    Pawn.PushMessage(new FallMessage(Pawn, from, to, _landedOn));
                }
            }
        }
    }
}