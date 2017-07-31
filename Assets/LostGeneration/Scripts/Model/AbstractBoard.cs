using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public abstract class AbstractBoard : IBoard {

        public Point Size {
            get { return new Point(_blocks.GetLength(0), _blocks.GetLength(1), _blocks.GetLength(2)); }
        }

        public Point CellSize {
            get { return _bucketSize; }
        }

        public event Action<Pawn> PawnAdded;
        public event Action<Pawn> PawnRemoved;
        public event Action<BoardBlock, BoardBlock> BlockChanged;
        public event Action<Dictionary<BoardBlock, BoardBlock>> BlocksChanged;

        private BoardBlock[,,] _blocks;
        private Point _bucketSize;
        protected HashSet<Pawn> Pawns = new HashSet<Pawn>();
        protected List<Pawn> PawnOrder = new List<Pawn>();

        public AbstractBoard(Point size) {
            _blocks = new BoardBlock[size.X, size.Y, size.Z];
            for (int x = 0; x < size.X; x++) {
                for (int y = 0; y < size.Y; y++) {
                    for (int z = 0; z < size.Z; z++) {
                        _blocks[x, y, z].Point = new Point(x, y, z);
                    }
                }
            }
        }

        public AbstractBoard(int width, int height, int depth) {
            _blocks = new BoardBlock[width, height, depth];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < depth; z++) {
                        _blocks[x, y, z].Point = new Point(x, y, z);
                    }
                }
            }
        }

        #region BlockFunctions
        public void SetBlock(BoardBlock block) {
            if (InBounds(block.Point)) {
                BoardBlock oldBlock = _blocks[block.Point.X, block.Point.Y, block.Point.Z];
                _blocks[block.Point.X, block.Point.Y, block.Point.Z] = block;
                if (BlockChanged != null) {
                    BlockChanged(oldBlock, block);
                }
            } else {
                throw new ArgumentOutOfRangeException("Given Point " + block.Point + " is outside the bounds of the Board " + Size, "point");
            }
        }

        public void SetBlocks(IEnumerable<BoardBlock> blocks)
        {   
            if (blocks != null && blocks.Any())
            {
                Dictionary<BoardBlock, BoardBlock> changeSet = null;
                if (BlocksChanged != null)
                {
                    changeSet = new Dictionary<BoardBlock, BoardBlock>();
                }
                
                foreach (BoardBlock block in blocks)
                {
                    BoardBlock oldBlock = _blocks[block.Point.X, block.Point.Y, block.Point.Z];
                    if (!oldBlock.Equals(block))
                    {
                        _blocks[block.Point.X, block.Point.Y, block.Point.Z] = block;
                        
                        if (changeSet != null)
                        {
                            changeSet.Add(oldBlock, block);
                        }
                    }
                }

                if (changeSet != null && BlocksChanged != null)
                {
                    BlocksChanged(changeSet);
                }
            }
        }

        public BoardBlock GetBlock(Point point) {
            if (InBounds(point)) {
                return _blocks[point.X, point.Y, point.Z];
            } else {
                throw new ArgumentOutOfRangeException("Given Point " + point + " is outside the bounds of the Board " + Size, "point");
            }
        }

        /// <summary>
        /// Indicates whether or not a given Point is within the bounds of the Board.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool InBounds(Point point) {
            Point size = Size;
            return point.X >= 0 && point.X < size.X &&
                   point.Y >= 0 && point.Y < size.Y &&
                   point.Z >= 0 && point.Z < size.Z;
        }

        /// <summary>
        /// Checks if vision can pass through a Point in the board.  A point is considered opaque if the BoardBlock or any Pawn at that point is opaque.
        /// If the given point is outside the bounds of the Board, returns true.
        /// <param name="point">Point on the board to check</param>
        /// <returns>True if light cannot pass through this point on the Board.  False, otherwise or if point is outside the Board.</returns>
        public bool IsOpaque(Point point) {
            bool isOpaque = true;
            if (InBounds(point)) {
                BoardBlock block = GetBlock(point);
                isOpaque = block.IsOpaque;
                if (!isOpaque) {
                    foreach (Pawn pawn in PawnsAt(point)) {
                        if (pawn.IsOpaque) {
                            isOpaque = true;
                            break;
                        }
                    }
                }
            }

            return isOpaque;
        }

        public bool IsSolid(Point point) {
            bool isSolid = true;
            if (InBounds(point)) {
                isSolid = GetBlock(point).IsSolid;
                if (!isSolid) {
                    foreach (Pawn pawn in PawnsAt(point)) {
                        if (pawn.IsCollidable && pawn.IsSolid) {
                            isSolid = true;
                            break;
                        }
                    }
                }
            }

            return isSolid;
        }
        #endregion BlockFunctions

        #region PawnFunctions
        public virtual IEnumerator<Pawn> GetPawnIterator() {
            for (int i = 0; i < PawnOrder.Count; i++) {
                yield return PawnOrder[i];
            }
        }

        public virtual bool PawnExists(Pawn pawn) {
            return Pawns.Contains(pawn);
        }

        public virtual Pawn FindPawnByName(string name) {
            return PawnOrder.Find(pawn => pawn.Name == name);
        }
        
        public virtual IEnumerable<Pawn> FindPawnsByName(string name) {
            return PawnOrder.FindAll(pawn => pawn.Name == name);
        }

        public abstract IEnumerable<Pawn> PawnsAt(Point point);
        public abstract HashSet<Pawn> PawnsAt(IEnumerable<Point> points);
        public abstract bool AddPawn(Pawn pawn);
        public abstract bool RemovePawn(Pawn pawn);
        public abstract bool SetPawnPosition(Pawn pawn, Point newPosition);
        #endregion PawnFunctions

        #region StepFunctions
        public void BeginTurn() {
            PawnOrder.Sort();
            for (int i = 0; i < PawnOrder.Count; i++) {
                PawnOrder[i].BeginTurn();
            }
        }

        /// <summary>
        /// Calls the Step() function of each Pawn on the Board.
        /// </summary>
        /// <returns>true if a Pawn still has actions to perform in this step, false otherwise</returns>
        public Queue<PawnAction> Step(Queue<IPawnMessage> messages) {
            PawnOrder.Sort();
            
            Queue<PawnAction> actions = new Queue<PawnAction>();
            for (int i = 0; i < PawnOrder.Count; i++) {
                PawnAction action = PawnOrder[i].Step(messages);
                if (action != null) {
                    actions.Enqueue(action);
                }
            }

            return actions;
        }

        public Queue<PawnAction> Turn(Queue<IPawnMessage> messages) {
            Queue<PawnAction> actions = new Queue<PawnAction>();
            
            Queue<PawnAction> step;
            while ((step = Step(messages)).Count != 0) {
                actions.Union(step);
            }

            return actions;
        }
        #endregion StepFunctions

        #region SelectAlgs
        
        public bool LineCast(Point start, Point end, HashSet<Pawn> pawns = null, bool passThroughWalls = false, bool passThroughSolids = false) {
            Point[] line = Point.Line(start, end);
            bool stopped = false;

            foreach (Point point in line.OrderBy(p => Point.TaxicabDistance(start, p))) {
                if (InBounds(point)) {
                    if (!passThroughWalls && GetBlock(point).IsSolid) {
                        stopped = true;
                    } else {
                        HashSet<Pawn> pawnsAt = new HashSet<Pawn>(PawnsAt(point));
                        if (pawns != null) {
                            pawns.UnionWith(pawnsAt);
                        }
                        stopped = passThroughSolids && pawnsAt.FirstOrDefault(pawn => pawn.IsCollidable && pawn.IsSolid) != null;
                    }
                }

                if (stopped) {
                    break;
                }
            }

            return stopped;
        }

        // basic flood fill
        public HashSet<Point> AreaOfEffect(Point point, int range, bool ignoreSolid = false) {
            HashSet<Point> visited = new HashSet<Point>();
            Stack<Point> exploreStack = new Stack<Point>();

            Point current, neighbor;

            visited.Add(point);
            exploreStack.Push(point);

            while (exploreStack.Count > 0) {
                current = exploreStack.Pop();

                if (exploreStack.Count < range) {
                    for (int i = 0; i < Point.Neighbors.Length; i++) {
                        neighbor = current + Point.Neighbors[i];

                        if (!visited.Contains(neighbor) && (ignoreSolid || !IsSolid(neighbor))) {
                            exploreStack.Push(neighbor);
                        }
                    }
                }

                visited.Add(current);
            }
            

            return visited;
        }
        #endregion
    }
}