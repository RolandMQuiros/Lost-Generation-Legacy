using System.Collections.Generic;

namespace LostGen {
    public class WeightedPawn : Pawn {
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

        private AxisDirection _gravityDirection;
        private Point _gravity;

        private bool _checkLandings = false;
        private List<Pawn> _landedOn;

        public WeightedPawn(string name, Board board, Point position, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = false, bool isOpaque = true)
        : base(name, board, position, footprint, isCollidable, isSolid, isOpaque) { }

        public override void OnCollisionEnter(Pawn other) {
            if (_checkLandings && other.IsSolid) {
                _landedOn.Add(other);
            }
        }

        public virtual void OnLandedUpon(WeightedPawn by, Queue<IPawnMessage> messages) { }
        protected override void PostStep(Queue<IPawnMessage> messages) {
            // Move the Pawn in the direction of gravity until it encounters the edge of the 
            if (HasGravity && _gravity != Point.Zero) {
                _checkLandings = true;
                _landedOn = new List<Pawn>();
                Point from = Position;
                while (Offset(_gravity)); // Calls OnCollisionEnter
                Point to = Position;

                // If this Pawn actually fell somewhere, emit a message saying from where to where, and
                // on who we landed on
                if (from != to) {
                    messages.Enqueue(new FallMessage(this, from, to, _landedOn));

                    // Have pawns react to being landed on, letting them emit their own messages
                    for (int i = 0; i < _landedOn.Count; i++) {
                        WeightedPawn weightedOther = _landedOn[i] as WeightedPawn;
                        if (weightedOther != null) {
                            weightedOther.OnLandedUpon(this, messages);
                        }
                    }
                }
            }
        }
    }
}