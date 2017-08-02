using System;
using System.Collections.Generic;

namespace LostGen {
    public interface IBlockManager {
        Point Size { get; }
        event Action<Dictionary<BoardBlock, BoardBlock>> Changed;
        BoardBlock At(Point at);
        IEnumerable<BoardBlock> At(IEnumerable<Point> at);
        void Set(BoardBlock block);
        void Set(IEnumerable<BoardBlock> blocks);
        bool InBounds(Point point);
    }
}