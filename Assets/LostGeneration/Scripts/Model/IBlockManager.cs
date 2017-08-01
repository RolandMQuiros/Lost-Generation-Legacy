using System;
using System.Collections.Generic;

namespace LostGen {
    public interface IBlockManager {
        Point Size { get; }
        event Action<Dictionary<BoardBlock, BoardBlock>> BlocksChanged;
        BoardBlock Get(Point at);
        void Set(BoardBlock block);
        void Set(IEnumerable<BoardBlock> blocks);
        bool InBounds(Point point);
    }
}