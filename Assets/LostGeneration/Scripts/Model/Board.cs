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
        
        private Dictionary<Point, HashSet<Pawn>> _pawnBuckets = new Dictionary<Point, HashSet<Pawn>>();
        private Dictionary<Pawn, HashSet<Pawn>> _collisions = new Dictionary<Pawn, HashSet<Pawn>>();

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

        public IEnumerator<Pawn> GetPawnIterator() {
            return _collisions.Keys.GetEnumerator();
        }

        public IEnumerable<Pawn> CollisionsAt(Point point) {
            HashSet<Pawn> pawns;

            _pawnBuckets.TryGetValue(point, out pawns);

            return pawns;
        }

        private HashSet<Pawn> GetBucket(Point position) {
            HashSet<Pawn> bucket;
            _pawnBuckets.TryGetValue(position, out bucket);

            if (bucket == null) {
                bucket = new HashSet<Pawn>();
                _pawnBuckets.Add(position, bucket);
            }

            return bucket;
        }

        public bool AddPawn(Pawn pawn) {
            bool successful = true;

            if (pawn != null && !_collisions.ContainsKey(pawn)) {
                foreach (Point point in pawn.Footprint) {
                    HashSet<Pawn> bucket = GetBucket(pawn.Position + point);
                    successful &= bucket.Add(pawn);
                }
                _collisions.Add(pawn, new HashSet<Pawn>());
            }

            return successful;
        }

        // FUCK: what happens when an pawn stays still
        // Try keeping collision logic independent from steps? no thats retarded
        public bool SetPawnPosition(Pawn pawn, Point newPosition) {
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
                _pawnBuckets.TryGetValue(oldPoint, out oldBucket);
                if (oldBucket != null && oldBucket.Contains(pawn)) {
                    exitCollisions.UnionWith(oldBucket);
                    toRemove.Add(oldBucket);
                }

                // Add all pawns in the new position to the enter set
                newBucket = GetBucket(newPosition + pawn.Position);
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
	            	if (other.IsCollidable) {
		                pawn.OnCollisionEnter(other);
		                other.OnCollisionEnter(pawn);
		            }
	            }

	            foreach (Pawn other in stayCollisions) {
	                if (other.IsCollidable) {
	                	pawn.OnCollisionStay(other);
	                	other.OnCollisionStay(pawn);
	            	}
	            }

	            foreach (Pawn other in exitCollisions) {
	                if (other.IsCollidable) {
	                	pawn.OnCollisionExit(other);
	                	other.OnCollisionExit(pawn);
	            	}
	            }
	        }

            return moved;
        }

        public void Step() {
            foreach (Pawn pawn in _collisions.Keys) {
        		pawn.Step();
        	}
        }

        public void Turn() {
        	foreach (Pawn pawn in _collisions.Keys) {
        		pawn.Turn();
        	}
        }
    }
}