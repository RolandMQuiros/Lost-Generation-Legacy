using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Pawn : IComparable<Pawn> {
        public string Name { get; set; }

        protected Board _board;
        public Board Board {
            get { return _board; }
        }

        protected List<Point> _footprint;
        public ReadOnlyCollection<Point> Footprint {
            get { return _footprint.AsReadOnly(); }
        }

        private Point _position;
        public Point Position {
            get { return _position; }
            set { _board.SetPawnPosition(this, value); }
        }

        private LinkedList<Action> _actions = new LinkedList<Action>();
        public int ActionCount {
            get { return _actions.Count; }
        }

        public int Priority = Int32.MinValue;

        public bool IsCollidable;
        public bool IsSolid;
        public bool IsOpaque;

        public event EventHandler<MessageArgs> Messages;

        public delegate void CollisionDelegate(Pawn source, Pawn other);
        public event CollisionDelegate CollisionEntered;
        public event CollisionDelegate CollisionStayed;
        public event CollisionDelegate CollisionExited;

        public Pawn(string name, Board board, Point position, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = false, bool isOpaque = true) {
            Name = name;

            if (board == null) {
                throw new ArgumentNullException("board");
            }

            if (footprint == null) {
                _footprint = new List<Point>();
            } else {
                _footprint = new List<Point>(footprint);
            }

            if (_footprint.Count == 0) {
                _footprint.Add(Point.Zero);
            }

            _board = board;
            IsCollidable = isCollidable;
            IsSolid = isSolid;
            IsOpaque = isOpaque;

            _position = position;
        }

        ///<summary>Used by internally by Board. Do not use.</summary>
        public void SetPositionInternal(Point newPosition) {
            _position = newPosition;
        }

        public bool SetPosition(Point destination) {
            return _board.SetPawnPosition(this, destination);
        }

        public bool Offset(Point offset) {
            return SetPosition(Position + offset);
        }

        public virtual void OnCollisionEnter(Pawn other) {
            if (CollisionEntered != null) {
                CollisionEntered(this, other);
            }
        }

        public virtual void OnCollisionStay(Pawn other) {
            if (CollisionStayed != null) {
                CollisionStayed(this, other);
            }
        }
        public virtual void OnCollisionExit(Pawn other) {
            if (CollisionExited != null) {
                CollisionExited(this, other);
            }
        }

        public void EmitMessage(MessageArgs message) {
            if (Messages != null) {
                Messages(this, message);
            }
        }

        public void PushAction(Action action) {
            _actions.AddLast(action);
        }

        public void PushActionToFront(Action action) {
            _actions.AddFirst(action);
        }

        ///<summary>
		///Pops and runs a single action in the queue
		///</summary>
		public bool Step() {
            Action stepAction;

            if (_actions.Count > 0) {
                stepAction = _actions.Last.Value;
                stepAction.Run();
                _actions.RemoveLast();
            }

            return _actions.Count > 0;
        }

        public int CompareTo(Pawn other) {
            int compare = 0;
            if (Priority < other.Priority) {
                compare = -1;
            } else if (Priority > other.Priority) {
                compare = 1;
            }

            return compare;
        }
    }
}
