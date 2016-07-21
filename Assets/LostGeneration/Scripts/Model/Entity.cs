using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Entity {
        protected Board _board;
        protected List<Point> _footprint;
        public ReadOnlyCollection<Point> Footprint {
            get {
                return _footprint.AsReadOnly();
            }
        }
        public Point Position;

        public bool IsSolid;

        public Entity(Board board, Point position, IEnumerable<Point> footprint = null, bool isSolid = false) {
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

        public virtual void OnCollisionEnter(Entity other) { }
        public virtual void OnCollisionStay(Entity other) { }
        public virtual void OnCollisionExit(Entity other) { }
    }
}
