using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Board {
    	public const int WALL_TILE = 0;
    	public const int FLOOR_TILE = 1;

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

        private Pathfinder<Point> _pathfinder;

        public Board(int[,] tiles) {
            _tiles = new int[tiles.GetLength(0), tiles.GetLength(1)];
            Array.Copy(tiles, 0, _tiles, 0, tiles.Length);

            _pathfinder = new Pathfinder<Point>(); 
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