using System.Collections.Generic;

namespace LostGen.Model {
    public class DigMessage : IPawnMessage {
        public Pawn Source { get; private set; }
        public Pawn Target { get; private set; }
        public string Text { get; private set; }
        public bool IsCritical {get; private set; }
        public int Amount { get; private set; }
        public Point Point { get; private set; }
        public IEnumerable<BlockNode> Blocks { get { return _blocks.AsReadOnly(); } }

        private List<BlockNode> _blocks;

        public DigMessage(Pawn source, IEnumerable<BlockNode> blocks) {
            Source = source;
            _blocks = new List<BlockNode>(blocks);
        }

    }
}