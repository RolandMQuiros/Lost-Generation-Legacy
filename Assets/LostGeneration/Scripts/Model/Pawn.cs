using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Pawn : IComparable<Pawn> {
        /// <summary>Counter used to generate unique Pawn ID</summary>
        private static ulong _idCounter;

        #region Properties
        /// <summary>Unique instance identifier number</summary>
        public ulong InstanceID { get; private set; }
        /// <summary>User-facing name. Can also be used to search for Pawns on a Board.</summary>
        public string Name { get; set; }
        /// <summary>String representing what type of Pawn this is. Used to generate the display components for this Pawn.</summary>
        public int CharacterID { get; set; }
        /// <summary>Reference to the Board where this Pawn resides</summary>
        public Board Board { get; private set; }
        /// <summary>Returns a read-only version of the footprint</summary>
        public ReadOnlyCollection<Point> Footprint { get { return _footprint.AsReadOnly(); } }
        /// <summary>Returns the first PawnAction in the queue</summary>
        public PawnAction FirstAction {
            get {
                PawnAction first = null;
                if (_actions.First != null) {
                    first = _actions.First.Value;
                }
                return first;
            }
        }
        /// <summary>Returns the last PawnAction in the queue</summary>
        public PawnAction LastAction {
            get {
                PawnAction last = null;
                if (_actions.Last != null) {
                    last = _actions.Last.Value;
                }
                return last;
            }
        }
        
        public Point Position {
            get { return _position; }
            set { Board.SetPawnPosition(this, value); }
        }

        public IEnumerable<PawnAction> Actions { get { return _actions; } }
        public int ActionCount { get { return _actions.Count; } }
        #endregion Properties

        #region Fields
        /// <summary>
        /// Sorting priority. Determines what order the Board executes the Pawn steps.
        /// 
        /// Combatants run in a particular order based on their Speed stats.
        /// Environmental Pawns usually run at the end of a turn, where Priority = Int32.MaxValue.
        /// </summary>
        public int Priority = Int32.MinValue;

        /// <summary>
        /// Whether or not this Pawn can collide and be collided with other Pawns
        /// </summary>
        public bool IsCollidable;
        /// <summary>
        /// Whether or not this Pawn stops other Pawns from moving into the same square
        /// </summary>
        public bool IsSolid;
        /// <summary>
        /// Whether or not this Pawn stops field of view or light sources
        /// </summary>
        public bool IsOpaque;
        #endregion Fields

        #region Events
        public event EventHandler<MessageArgs> Messages;
        #endregion Events

        protected LinkedList<PawnAction> _actions = new LinkedList<PawnAction>();
        /// <summary>
        /// List of offsets that describe the space this Pawn takes up on the Board. For example, a very large door may take
        /// up four Points.
        /// </summary>
        protected List<Point> _footprint;
        private Point _position;

        public Pawn(string name, Board board, Point position, IEnumerable<Point> footprint = null, bool isCollidable = true, bool isSolid = false, bool isOpaque = true) {
            InstanceID = _idCounter++;

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

            Board = board;
            IsCollidable = isCollidable;
            IsSolid = isSolid;
            IsOpaque = isOpaque;

            _position = position;
        }

        ///<summary>Used by internally by Board. Do not use.</summary>
        public void SetPositionInternal(Point newPosition) {
            Point oldPosition = _position;
            _position = newPosition;
        }

        public bool SetPosition(Point destination) {
            return Board.SetPawnPosition(this, destination);
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

        public virtual void PushActions(IEnumerable<PawnAction> actions) {
            foreach (PawnAction action in actions) {
                PushAction(action);
            }
        }

        public virtual void PushAction(PawnAction action) {
            _actions.AddLast(action);
        }

        public virtual void ClearActions() {
            LinkedList<PawnAction> oldActions = _actions;
            _actions = new LinkedList<PawnAction>();
        }
        
        public virtual void BeginTurn() { }

        ///<summary>
		///Pops and runs a single action in the queue
		///</summary>
		public virtual PawnAction Step() {
            PawnAction stepAction = null;
            if (_actions.Count > 0) {
                stepAction = _actions.First.Value;
                PreprocessAction(stepAction);

                stepAction.Do();
                stepAction.Commit();

                _actions.RemoveFirst();
            }

            return stepAction;
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

        protected virtual void PreprocessAction(PawnAction action) { }
    }
}
