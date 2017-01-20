using System.Collections.Generic;

namespace LostGen {
    public class DigAction : PawnAction {
        private List<Point> _pointsToDig;
        private Dictionary<Point, BoardBlock> _blocksDug;
        public DigAction(Pawn owner, IEnumerable<Point> pointsToDig)
        : base(owner) {
            _pointsToDig = new List<Point>(pointsToDig);
            _blocksDug = new Dictionary<Point, BoardBlock>();
        }
        
        public override void Do() {
            Board board = Owner.Board;
            for (int i = 0; i < _pointsToDig.Count; i++) {
                if (board.InBounds(_pointsToDig[i])) {
                    BoardBlock block = board.GetBlock(_pointsToDig[i]);
                    if (block.IsDiggable) {
                        _blocksDug.Add(_pointsToDig[i], block);
                        block.IsOpaque = false;
                        block.IsSolid = false;
                        board.SetBlock(block, _pointsToDig[i]);
                    }
                }
            }
        }

        public override void Undo() {
            Board board = Owner.Board;
            foreach (KeyValuePair<Point, BoardBlock> pair in _blocksDug) {
                board.SetBlock(pair.Value, pair.Key);
            }
        }

        public override void Commit(Queue<IPawnMessage> messages) {
            
        }
    }

}