using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Pawn : IComparable<Pawn> {
        /// <summary>Counter used to generate unique Pawn ID</summary>
        private static ulong _idCounter;
        /// <summary>Unique instance identifier number</summary>
        public ulong InstanceID { get; private set; }
        /// <summary>User-facing name. Can also be used to search for Pawns on a Board.</summary>
        public string Name { get; set; }
        /// <summary>String representing what type of Pawn this is. Used to generate the display components for this Pawn.</summary>
        public string TypeName { get; set; }
        /// <summary>Reference to the Board where this Pawn resides</summary>
        public Board Board { get; private set; }
        /// <summary> Returns a read-only version of the footprint</summary>
        public ReadOnlyCollection<Point> Footprint { get { return _footprint.AsReadOnly(); } }

        
        public Point Position {
            get { return _position; }
            set { Board.SetPawnPosition(this, value); }
        }

        public int ActionCount {
            get { return _actions.Count; }
        }

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

        public event EventHandler<MessageArgs> Messages;

        public delegate void CollisionDelegate(Pawn source, Pawn other);
        public event CollisionDelegate CollisionEntered;
        public event CollisionDelegate CollisionStayed;
        public event CollisionDelegate CollisionExited;

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
            _position = newPosition;
        }

        public bool SetPosition(Point destination) {
            return Board.SetPawnPosition(this, destination);
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

        public void PushActions(IEnumerable<PawnAction> actions) {
            foreach (PawnAction action in actions) {
                _actions.AddLast(action);
            }
        }

        public void PushAction(PawnAction action) {
            _actions.AddLast(action);
        }

        public void ClearActions() {
            _actions.Clear();
        }
        
        public virtual void BeginTurn() { }

        ///<summary>
		///Pops and runs a single action in the queue
		///</summary>
		public virtual bool Step() {
            if (_actions.Count > 0) {
                PawnAction stepAction = _actions.First.Value;
                PreprocessAction(stepAction);
                stepAction.Run();
                _actions.RemoveFirst();
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

        protected virtual void PreprocessAction(PawnAction action) { }
    }
}
