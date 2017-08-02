using System;

namespace LostGen {
    public struct BoardBlock : IEquatable<BoardBlock> {
        public Point Point;
        public bool IsSolid;
        public bool IsOpaque;
        public bool IsDiggable;
        public byte BlockType;

        public bool Equals(BoardBlock other) {
            return Point == other.Point &&
                   IsSolid == other.IsSolid &&
                   IsOpaque == other.IsOpaque &&
                   IsDiggable == other.IsDiggable &
                   BlockType == other.BlockType;
        }

        public static bool operator ==(BoardBlock first, BoardBlock second) {
            return first.Equals(second);
        }

        public static bool operator !=(BoardBlock first, BoardBlock second) {
            return !first.Equals(second);
        }

        public override string ToString() {
            return "{ Point: " + Point + ", IsSolid: " + IsSolid +
                   ", IsOpaque: " + IsOpaque + ", IsDiggable: " + IsDiggable +
                   ", BlockType: " + BlockType + " }";
        }
    }
}