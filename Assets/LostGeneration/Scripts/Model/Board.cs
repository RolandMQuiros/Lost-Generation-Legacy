using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public struct BoardBlock {
        public bool IsSolid;
        public bool IsOpaque;
    }

    public class Board {
        private BoardBlock[,,] _blocks;

        public Point Size {
            get { return new Point(_blocks.GetLength(0), _blocks.GetLength(1), _blocks.GetLength(2)); }
        }

        public event Action<Pawn> PawnAdded;
        public event Action<Pawn> PawnRemoved;

        private HashSet<Pawn> _pawns = new HashSet<Pawn>();
        private List<Pawn> _pawnOrder = new List<Pawn>();
        private Dictionary<Point, HashSet<Pawn>> _pawnBuckets = new Dictionary<Point, HashSet<Pawn>>();

        public Board(Point size) {
            _blocks = new BoardBlock[size.X, size.Y, size.Z];
        }

        public Board(int width, int height, int depth) {
            _blocks = new BoardBlock[width, height, depth];
        }

        public void SetBlock(BoardBlock block, Point point) {
            if (InBounds(point)) {
                _blocks[point.X, point.Y, point.Z] = block;
            } else {
                throw new ArgumentOutOfRangeException("Given Point " + point + " is outside the bounds of the Board " + Size, "point");
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
                    HashSet<Pawn> pointBucket = GetBucket(point);

                    foreach (Pawn pawn in pointBucket) {
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
                    HashSet<Pawn> pawns = PawnsAt(point);
                    foreach (Pawn pawn in pawns) {
                        if (pawn.IsCollidable && pawn.IsSolid) {
                            isSolid = true;
                            break;
                        }
                    }
                }
            }

            return isSolid;
        }

        public IEnumerator<Pawn> GetPawnIterator() {
            return _pawns.GetEnumerator();
        }

        public bool PawnExists(Pawn pawn) {
            return _pawns.Contains(pawn);
        }
        
        public Pawn FindPawnByName(string name) {
            return _pawnOrder.Find(pawn => pawn.Name == name);
        }

        public IEnumerable<Pawn> FindPawnsByName(string name) {
            return _pawnOrder.FindAll(pawn => pawn.Name == name);
        }

        private HashSet<Pawn> GetBucket(Point position, bool create = false) {
            HashSet<Pawn> bucket;
            _pawnBuckets.TryGetValue(position, out bucket);

            if (create && bucket == null) {
                bucket = new HashSet<Pawn>();
                _pawnBuckets.Add(position, bucket);
            }

            return bucket;
        }

        public HashSet<Pawn> PawnsAt(Point point) {
            return GetBucket(point, true);
        }

        public HashSet<Pawn> PawnsAt(IEnumerable<Point> points) {
            HashSet<Pawn> pawns = new HashSet<Pawn>();
            foreach (Point point in points) {
                pawns.UnionWith(PawnsAt(point));
            }

            return pawns;
        }

        public bool AddPawn(Pawn pawn) {
            bool successful = true;

            if (pawn != null && !_pawns.Contains(pawn)) {
                foreach (Point point in pawn.Footprint) {
                    HashSet<Pawn> bucket = GetBucket(pawn.Position + point, true);
                    successful &= bucket.Add(pawn);
                }

                if (successful) {
                    successful &= _pawns.Add(pawn);
                    if (!_pawnOrder.Contains(pawn)) {
                        _pawnOrder.Add(pawn);
                    }
                }
            }

            if (successful && PawnAdded != null) {
                PawnAdded(pawn);
            }

            return successful;
        }

        public bool RemovePawn(Pawn pawn) {
            bool successful = true;

            if (pawn != null && _pawns.Contains(pawn)) {
                foreach (Point point in pawn.Footprint) {
                    HashSet<Pawn> bucket = GetBucket(pawn.Position + point, true);
                    successful &= bucket.Remove(pawn);
                }

                if (successful) {
                    successful &= _pawns.Remove(pawn);
                    if (_pawnOrder.Contains(pawn)) {
                        _pawnOrder.Remove(pawn);
                    }
                }
            }

            if (successful && PawnRemoved != null) {
                PawnRemoved(pawn);
            }

            return successful;
        }
        
        public bool SetPawnPosition(Pawn pawn, Point newPosition) {
            if (!_pawns.Contains(pawn)) {
                throw new ArgumentException("Pawn " + pawn.Name + " has not been added to this Board", "pawn");
            }

            bool moved = false;

            List<HashSet<Pawn>> toRemove = new List<HashSet<Pawn>>();
            List<HashSet<Pawn>> toAdd = new List<HashSet<Pawn>>();

            HashSet<Pawn> enterCollisions = new HashSet<Pawn>();
            HashSet<Pawn> exitCollisions = new HashSet<Pawn>();

            Point oldPoint;
            Point newPoint;
            bool walled = false;

            // Gather all pawns that were in the original and new position's footprint
            foreach (Point offset in pawn.Footprint) {
                HashSet<Pawn> oldBucket;
                HashSet<Pawn> newBucket;

                oldPoint = pawn.Position + offset;
                newPoint = newPosition + offset;

                // Check if there's a wall at the new point.  If so, early out.
                if (pawn.IsSolid) {
                    if (InBounds(newPoint) && GetBlock(newPoint).IsSolid) {
                        return false;
                    }
                }

                // Add all pawns in the current bucket to the exit collision set
                oldBucket = GetBucket(oldPoint, false);
                if (oldBucket != null) {
                    exitCollisions.UnionWith(oldBucket);
                    toRemove.Add(oldBucket);
                }

                // Add all pawns in the new position to the enter set
                newBucket = GetBucket(newPoint, true);
                enterCollisions.UnionWith(newBucket);
                toAdd.Add(newBucket);
            }

            // If the pawn is solid, and any of the entering collision pawns are solid, don't allow the move
            bool willMove = true;
            if (pawn.IsSolid) {
            	if (walled) {
            		willMove = false;
            	} else {
	                foreach (Pawn other in enterCollisions) {
	                    if (other != pawn && other.IsSolid) {
	                        willMove = false;
	                        break;
	                    }
	                }
	            }
            }

            // Move the pawn between buckets
            if (willMove) {
                for (int i = 0; i < toRemove.Count; i++) {
                    toRemove[i].Remove(pawn);
                }

                for (int i = 0; i < toAdd.Count; i++) {
                    toAdd[i].Add(pawn);
                }

                pawn.SetPositionInternal(newPosition);
                moved = true;
            }

            if (pawn.IsCollidable) {
	            // The stay set consists of pawns that are in both the exit and enter sets
	            HashSet<Pawn> stayCollisions = new HashSet<Pawn>(exitCollisions);
	            stayCollisions.IntersectWith(enterCollisions);

	            // Remove the stay collisions from both the exit and enter sets
	            enterCollisions.ExceptWith(stayCollisions);
	            exitCollisions.ExceptWith(stayCollisions);

	            // Call the collision methods
	            foreach (Pawn other in enterCollisions) {
	            	if (pawn != other && other.IsCollidable) {
		                pawn.OnCollisionEnter(other);
		                other.OnCollisionEnter(pawn);
		            }
	            }

	            foreach (Pawn other in stayCollisions) {
	                if (pawn != other && other.IsCollidable) {
	                	pawn.OnCollisionStay(other);
	                	other.OnCollisionStay(pawn);
	            	}
	            }

	            foreach (Pawn other in exitCollisions) {
	                if (pawn != other && other.IsCollidable) {
	                	pawn.OnCollisionExit(other);
	                	other.OnCollisionExit(pawn);
	            	}
	            }
	        }

            return moved;
        }

        public void BeginTurn() {
            _pawnOrder.Sort();
            for (int i = 0; i < _pawnOrder.Count; i++) {
                _pawnOrder[i].BeginTurn();
            }
        }

        /// <summary>
        /// Calls the Step() function of each Pawn on the Board.
        /// </summary>
        /// <returns>true if a Pawn still has actions to perform in this step, false otherwise</returns>
        public Queue<PawnAction> Step(Queue<IPawnMessage> messages) {
            _pawnOrder.Sort();
            
            Queue<PawnAction> actions = new Queue<PawnAction>();
            for (int i = 0; i < _pawnOrder.Count; i++) {
                PawnAction action = _pawnOrder[i].Step(messages);
                actions.Enqueue(action);
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

        #region SelectAlgs
        
        public bool LineCast(Point start, Point end, HashSet<Pawn> pawns = null, bool passThroughWalls = false, bool passThroughSolids = false) {
            Point[] line = Point.Line(start, end);
            bool stopped = false;

            foreach (Point point in line.OrderBy(p => Point.TaxicabDistance(start, p))) {
                if (InBounds(point)) {
                    if (!passThroughWalls && GetBlock(point).IsSolid) {
                        stopped = true;
                    } else {
                        HashSet<Pawn> pawnsAt = PawnsAt(point);
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