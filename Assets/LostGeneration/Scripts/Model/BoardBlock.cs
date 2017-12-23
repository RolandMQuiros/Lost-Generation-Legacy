using System;

namespace LostGen.Model {
    public struct BoardBlock : IEquatable<BoardBlock> {
        public Point Point {
            get { return _point; }
            set { _point = value; UpdateHash(); }
        }
        public bool IsSolid {
            get { return _isSolid; }
            set { _isSolid = value; UpdateHash(); }
        }
        public bool IsOpaque {
            get { return _isOpaque; }
            set { _isOpaque = value; UpdateHash(); }
        }
        public bool IsDiggable {
            get { return _isDiggable; }
            set { _isDiggable = value; UpdateHash(); }
        }
        public byte BlockType {
            get { return _blockType; }
            set { _blockType = value; UpdateHash(); }
        }

        private Point _point;
        private bool _isSolid;
        private bool _isOpaque;
        private bool _isDiggable;
        private byte _blockType;
        private int _hashCode;

        public bool Equals(BoardBlock other) {
            return _point      == other._point &&
                   _isSolid    == other._isSolid &&
                   _isOpaque   == other._isOpaque &&
                   _isDiggable == other._isDiggable &
                   _blockType  == other._blockType;
        }

        public override bool Equals(object obj) {
            return Equals((BoardBlock)obj);
        }

        public override int GetHashCode() {
            return _hashCode;
        }

        public static bool operator ==(BoardBlock first, BoardBlock second) {
            return first.Equals(second);
        }

        public static bool operator !=(BoardBlock first, BoardBlock second) {
            return !first.Equals(second);
        }

        public override string ToString() {
            return "{ Point: " + _point + ", IsSolid: " + _isSolid +
                   ", IsOpaque: " + _isOpaque + ", IsDiggable: " + _isDiggable +
                   ", BlockType: " + _blockType + " }";
        }

        private void UpdateHash() {
            _hashCode = new { _point, _isSolid, _isOpaque, _isDiggable, _blockType }.GetHashCode();
        }
    }
}