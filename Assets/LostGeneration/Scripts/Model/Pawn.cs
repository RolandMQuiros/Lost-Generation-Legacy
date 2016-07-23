using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Pawn {
        public string Name { get; set; }

        protected Board _board;

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

        public bool IsCollidable;
        public bool IsSolid;
        public event EventHandler<MessageArgs> Messages;

        public Pawn(string name, Board board, Point position, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = false) {
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
            Position = position;
            IsSolid = isSolid;
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

        public virtual void OnCollisionEnter(Pawn other) { }
        public virtual void OnCollisionStay(Pawn other) { }
        public virtual void OnCollisionExit(Pawn other) { }

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

        ///<summary>
        ///Runs all the actions in the queue
        ///</summary>
        public void Turn() {
            while (Step()) ;
        }
    }
}
