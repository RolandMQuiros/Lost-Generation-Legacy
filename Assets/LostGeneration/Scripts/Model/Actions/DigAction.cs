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
        
        public override void Do() {
            Board board = Owner.Board;
            for (int i = 0; i < _pointsToDig.Count; i++) {
                if (board.InBounds(_pointsToDig[i])) {
                    BoardBlock block = board.GetBlock(_pointsToDig[i]);
                    if (block.IsDiggable) {
                        _blocksDug.Add(block);
                        block.IsOpaque = false;
                        block.IsSolid = false;
                        board.SetBlock(block);
                    }
                }
            }
        }

        public override void Undo() {
            Board board = Owner.Board;
            for (int i = 0; i < _blocksDug.Count; i++) {
                board.SetBlock(_blocksDug[i]);
            }
        }

        public override void Commit(Queue<IPawnMessage> messages) {
            if (messages != null && _blocksDug.Count > 0) {
                messages.Enqueue(new DigMessage(Owner, _blocksDug.Cast<BlockNode>()));
            }
        }
    }

}