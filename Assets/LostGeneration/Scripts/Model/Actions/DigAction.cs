using System.Collections.Generic;
using System.Linq;

namespace LostGen {
    public class DigAction : PawnAction {
        private List<Point> _pointsToDig;
        private List<BoardBlock> _blocksDug;
        public DigAction(Pawn owner, IEnumerable<Point> pointsToDig)
        : base(owner) {
            _pointsToDig = new List<Point>(pointsToDig);
            _blocksDug = new List<BoardBlock>();
        }
        
        public override bool Do() {
            Board board = Owner.Board;
            for (int i = 0; i < _pointsToDig.Count; i++) {
                if (board.Blocks.InBounds(_pointsToDig[i])) {
                    BoardBlock block = board.Blocks.At(_pointsToDig[i]);
                    if (block.IsDiggable) {
                        _blocksDug.Add(block);
                        block.IsOpaque = false;
                        block.IsSolid = false;
                        board.Blocks.Set(block);
                    }
                }
            }

            return true;
        }

        public override void Undo() {
            Board board = Owner.Board;
            for (int i = 0; i < _blocksDug.Count; i++) {
                board.Blocks.Set(_blocksDug[i]);
            }
        }

        public override void Commit() {
            if (_blocksDug.Count > 0) {
                Owner.PushMessage(new DigMessage(Owner, _blocksDug.Cast<BlockNode>()));
            }
        }
    }

}