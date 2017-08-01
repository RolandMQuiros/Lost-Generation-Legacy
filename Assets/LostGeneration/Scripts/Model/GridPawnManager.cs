using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class GridPawnManager : IPawnManager {
        private class Bucket {
            public Point Corner { get; private set; }
            public Point Size { get; private set; }
            private HashSet<Pawn> _pawns = new HashSet<Pawn>();

            public Bucket(Point corner, Point size) {
                Corner = corner;
                Size = size;
            }

            public bool InBounds(Point point) {
                return  point.X >= Corner.X && point.X < Corner.X + Size.X &&
                        point.Y >= Corner.Y && point.Y < Corner.Y + Size.Y &&
                        point.Z >= Corner.Z && point.Z < Corner.Z + Size.Z;                     
            }

            public bool AddPawn(Pawn pawn) {
                return _pawns.Add(pawn);
            }

            public bool RemovePawn(Pawn pawn) {
                return _pawns.Remove(pawn);
            }

            public bool HasPawn(Pawn pawn) {
                return _pawns.Contains(pawn);
            }

            public IEnumerable<Pawn> PawnsAt(Point point) {
                return _pawns.Where(
                    pawn => pawn.Footprint.Where(footprint => footprint + pawn.Position == point)
                                          .Any()
                );
            }

            public IEnumerable<Pawn> PawnsAt(IEnumerable<Point> points) {
                return _pawns.Where(
                    pawn => pawn.Footprint.Select(fp => fp + pawn.Position)
                                          .Intersect(points)
                                          .Any()
                );
            }
        }
        private HashSet<Pawn> _pawns = new HashSet<Pawn>();
        private List<Bucket> _buckets = new List<Bucket>();
        private IBlockManager _blocks;

        public event Action<Pawn> PawnAdded;
        public event Action<Pawn> PawnRemoved;
        public IEnumerable<Pawn> Ordered {
            get { return _pawns.OrderBy(pawn => pawn.Priority); }
        }
        public GridPawnManager(IBlockManager blocks) {
            _blocks = blocks;
        }
        public bool Exists(Pawn pawn) {
            return _pawns.Contains(pawn);
        }
        public IEnumerable<Pawn> FindByName(string name) {
            return _pawns.Where(pawn => pawn.Name == name);
        }
        public IEnumerable<Pawn> At(Point point) {
            return _buckets.Where( bucket => bucket.InBounds(point) )
                           .SelectMany( bucket => bucket.PawnsAt(point) ); 
        }
        public IEnumerable<Pawn> At(IEnumerable<Point> points) {
            return points.SelectMany(point => At(point));
        }
        public bool Add(Pawn pawn) {
            return AddInternal(pawn).Any();
        }
        public bool Remove(Pawn pawn) {
            return RemoveInternal(pawn).Any();
        }

        public bool SetPosition(Pawn pawn, Point newPosition) {
            if (!_blocks.InBounds(newPosition)) {
                return false;
            }
            
            IEnumerable<Point> oldFootprint = pawn.Footprint.Select(f => f + pawn.Position);
            IEnumerable<Point> newFootprint = pawn.Footprint.Select(f => f + newPosition);
            
            IEnumerable<Pawn> oldCollisions = At(oldFootprint).Where(p => p != pawn);
            IEnumerable<Pawn> newCollisions = At(newFootprint).Where(p => p != pawn);

            // Check if the move can be made
            bool willMove = true;
            if (pawn.IsSolid) {
                willMove = newCollisions.Where(p => p.IsSolid).Any();   
            }

            if (willMove) {
                
            }

            return false;
        }

        private IEnumerable<Bucket> BucketsWith(Pawn pawn) {
            return _buckets.Where(bucket => bucket.HasPawn(pawn));
        }

        private IEnumerable<Bucket> AddInternal(Pawn pawn) {
            for (int b = 0; b < _buckets.Count; b++) {
                for (int f = 0; f < pawn.Footprint.Count; f++) {
                    Bucket bucket = _buckets[b];
                    Point footprint = pawn.Footprint[f] + pawn.Position;
                    if (bucket.InBounds(footprint)) {
                        bucket.AddPawn(pawn);
                        yield return bucket;
                    }
                }
            }
        }
        private IEnumerable<Bucket> RemoveInternal(Pawn pawn) {
            for (int b = 0; b < _buckets.Count; b++) {
                Bucket bucket = _buckets[b];
                if (bucket.RemovePawn(pawn)) {
                    yield return bucket;
                }
            }
        }
    }
}