using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Board {
    	public const int WALL_TILE = 0;
    	public const int FLOOR_TILE = 1;

        /// <summary>
        /// Graph node representation of a Board coordinate. This connects Points on the Board with adjacent
        /// Points, and sets edge values depending on at those points, or Tile types.
        /// </summary>
        public class Node : GraphNode<Point> {
            /// <summary>
            /// A function that takes a Point on the Board, and evaluates the AP cost of moving to that Point, often
            /// depending on the Tile type or what Pawns occupy that Point.
            /// 
            /// Having a delegate handle the cost evaluation lets different Pawns decide their movement strategy differently.
            /// </summary>
            /// <param name="point">Point to move to. Does not necessarily need to be adjacent to this Node.</param>
            /// <returns>Cost of moving to point</returns>
            public delegate int EdgeCostLookup(Point point);

            /// <summary>Point on the Board</summary>
            protected Point _point;
            /// <summary>Reference to the Board</summary>
            protected Board _board;
            /// <summary>List of adjacent, non-Wall, in-bounds Points on the Board</summary>
            protected List<GraphNode<Point>> _neighbors = new List<GraphNode<Point>>();
            /// <summary>Edge cost callback</summary>
            protected EdgeCostLookup _edgeCostLookup;

            /// <summary>
            /// Construct a new Node
            /// </summary>
            /// <param name="board"></param>
            /// <param name="point"></param>
            /// <param name="lookup"></param>
            public Node(Board board, Point point, EdgeCostLookup lookup = null) {
                if (board == null) { throw new ArgumentNullException("board"); }
                if (lookup == null) { throw new ArgumentNullException("lookup"); }

                _board = board;
                _point = point;
                _edgeCostLookup = lookup;
            }

            public override Point GetData() {
                return _point;
            }

            /// <summary>
            /// Returns the cost of moving from the current Node to an adjacent Node
            /// </summary>
            /// <param name="neighbor">Another Node. Doesn't actually have to be adjacent to this.</param>
            /// <returns></returns>
            public override int GetEdgeCost(GraphNode<Point> neighbor) {
                int cost = 0;
                if (_edgeCostLookup != null) {
                    Point toPoint = neighbor.GetData();
                    cost = _edgeCostLookup(toPoint);
                }
                return cost;
            }

            /// <summary>
            /// Iterates through adjacent Points.  To avoid having to construct the entire Board as a graph,
            /// this function acts as an iterator, and creates neighbor Nodes only as needed.
            /// </summary>
            /// <returns>An IEnumerable that iterates through this Node's neighboring Points</returns>
            public override IEnumerable<GraphNode<Point>> GetNeighbors() {
                for (int i = 0; i < Point.OctoNeighbors.Length; i++) {
                    Point neighborPoint = _point + Point.Neighbors[i];

                    if (_board.InBounds(neighborPoint) && _board.GetTile(neighborPoint) != Board.WALL_TILE) {
                        // Check if a Node already exists for the Point
                        Node neighbor = _neighbors.Find(node => node.GetData().Equals(neighborPoint)) as Node;
                        if (neighbor == null) {
                            // If no Node exists, create one
                            neighbor = new Node(_board, neighborPoint, _edgeCostLookup);
                            _neighbors.Add(neighbor);

                            yield return neighbor;
                        } else {
                            // If a Node exists, return the stored one
                            yield return neighbor;
                        }
                    }
                }
            }
        }

        private int[,] _tiles;
        public int Width {
        	get { return _tiles.GetLength(1); }
        }

        public int Height {
        	get { return _tiles.GetLength(0); }
        }

        private HashSet<Pawn> _pawns = new HashSet<Pawn>();
        private List<Pawn> _pawnOrder = new List<Pawn>();
        private Dictionary<Point, HashSet<Pawn>> _pawnBuckets = new Dictionary<Point, HashSet<Pawn>>();
        
        public Board(int[,] tiles) {
            _tiles = new int[tiles.GetLength(0), tiles.GetLength(1)];
            Array.Copy(tiles, 0, _tiles, 0, tiles.Length);
        }

        public int GetTile(int x, int y) {
        	return _tiles[y, x];
        }

        public int GetTile(Point point) {
            return _tiles[point.Y, point.X];
        }

        public bool InBounds(Point point) {
            return point.X >= 0 && point.X < Width &&
                   point.Y >= 0 && point.Y < Height;
        }

        public bool IsOpaque(Point point) {
            bool isOpaque = GetTile(point) == WALL_TILE;
            if (!isOpaque) {
                HashSet<Pawn> pointBucket = GetBucket(point);

                foreach (Pawn pawn in pointBucket) {
                    if (pawn.IsOpaque) {
                        isOpaque = true;
                        break;
                    }
                }
            }

            return isOpaque;
        }

        public bool IsSolid(Point point) {
            bool isSolid = GetTile(point) == WALL_TILE;
            if (!isSolid) {
                HashSet<Pawn> pawns = PawnsAt(point);
                foreach (Pawn pawn in pawns) {
                    if (pawn.IsCollidable && pawn.IsSolid) {
                        isSolid = true;
                        break;
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
                    if (InBounds(newPoint) && GetTile(newPoint) == WALL_TILE) {
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
	                    if (other.IsSolid) {
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
                    toAdd[i].Remove(pawn);
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

        public bool Step() {
            _pawnOrder.Sort();
            bool actionsLeft = false;
            for (int i = 0; i < _pawnOrder.Count; i++) {
                actionsLeft |= _pawnOrder[i].Step();
            }

            return actionsLeft;
        }

        public void Turn() {
            while (Step());
        }

        #region SelectAlgs

        public HashSet<Point> LineOfSight(Point position, int range) {
            HashSet<Point> visible = ShadowCast.ComputeVisibility(this, position, (float)range);

            return visible;
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