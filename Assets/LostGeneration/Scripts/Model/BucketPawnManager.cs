using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class BucketPawnManager : IPawnManager {
        private class Bucket {
            private HashSet<Pawn> _pawns = new HashSet<Pawn>();
            private Point _corner;
            private Point _size;

            public Point Corner { get { return _corner; } }
            public Point Size { get { return _size; } }

            public Bucket(Point corner, Point size) {
                _corner = corner;
                _size = size;
            }

            public bool InBounds(Point point) {
                return  point.X >= _corner.X && point.X < _corner.X + _size.X &&
                        point.Y >= _corner.Y && point.Y < _corner.Y + _size.Y &&
                        point.Z >= _corner.Z && point.Z < _corner.Z + _size.Z;                     
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
        private Point _bucketSize;

        public Point BucketSize { get { return _bucketSize; } }
        public event Action<Pawn> Added;
        public event Action<Pawn> Removed;
        public IEnumerable<Pawn> Ordered {
            get { return _pawns.OrderBy(pawn => pawn.Priority); }
        }

        public BucketPawnManager(Point boardSize, Point bucketSize) {
            if (boardSize.X <= 0 || boardSize.Y <= 0 || boardSize.Z <= 0) {
                throw new ArgumentException("Board dimensions given were " + boardSize + ". They must be at least " + Point.One, "boardSize");
            }

            _bucketSize.X = Math.Min(boardSize.X, bucketSize.X);
            _bucketSize.Y = Math.Min(boardSize.Y, bucketSize.Y);
            _bucketSize.Z = Math.Min(boardSize.Z, bucketSize.Z);

            for (int x = 0; x < boardSize.X; x += _bucketSize.X) {
                for (int y = 0; y < boardSize.Y; y += _bucketSize.Y) {
                    for (int z = 0; z < boardSize.Z; z += _bucketSize.Z) {
                        Point corner = new Point(x, y, z);
                        Point bucketFitSize = new Point(
                            Math.Min(boardSize.X - corner.X, _bucketSize.X),
                            Math.Min(boardSize.Y - corner.Y, _bucketSize.Y),
                            Math.Min(boardSize.Z - corner.Z, _bucketSize.Z)
                        );
                        _buckets.Add(new Bucket(corner, bucketFitSize));
                    }
                }
            }
        }

        public BucketPawnManager(Point boardSize, int subdivisions) {
            if (boardSize.X <= 0 || boardSize.Y <= 0 || boardSize.Z <= 0) {
                throw new ArgumentException("Board dimensions given were " + boardSize + ". They must be at least " + Point.One, "boardSize");
            }
            if (subdivisions <= 0) {
                throw new ArgumentException("Board must have at least one subdivision. You gave me " + subdivisions + ". The hell.", "subdivisions");
            }

            _bucketSize = new Point(
                Math.Max(boardSize.X / subdivisions, 1),
                Math.Max(boardSize.Y / subdivisions, 1),
                Math.Max(boardSize.Z / subdivisions, 1)
            );

            for (int x = 0; x < boardSize.X; x += _bucketSize.X) {
                for (int y = 0; y < boardSize.Y; y += _bucketSize.Y) {
                    for (int z = 0; z < boardSize.Z; z += _bucketSize.Z) {
                        Point corner = new Point(x, y, z);
                        _buckets.Add(new Bucket(corner, _bucketSize));
                    }
                }
            }
        }

        public bool Contains(Pawn pawn) {
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
            bool addedToSet = _pawns.Add(pawn);
            bool addedToBucket = false;
            
            if (addedToSet) {
                for (int f = 0; f < pawn.Footprint.Count; f++) {
                    for (int b = 0; b < _buckets.Count; b++) {
                        Bucket bucket = _buckets[b];
                        Point footprint = pawn.Footprint[f] + pawn.Position;
                        if (bucket.InBounds(footprint)) {
                            addedToBucket |= bucket.AddPawn(pawn);
                            pawn.Start();
                        }
                    }
                }
            }

            bool added = addedToSet && addedToBucket;
            if (added && Added != null) {
                Added(pawn);
            }

            return added;
        }
        
        public bool Remove(Pawn pawn) {
            bool removedFromBucket = false;

            if (_pawns.Remove(pawn)) {
                for (int b = 0; b < _buckets.Count; b++) {
                    Bucket bucket = _buckets[b];
                    removedFromBucket |= bucket.RemovePawn(pawn);
                }
            }

            if (removedFromBucket && Removed != null) {
                Removed(pawn);
            }

            return removedFromBucket;
        }

        public void Move(Pawn pawn, Point newPosition) {
            IEnumerable<Bucket> oldBuckets = _buckets.Where(b => b.HasPawn(pawn));
            IEnumerable<Bucket> newBuckets = pawn.Footprint.SelectMany(f => _buckets.Where(b => b.InBounds(f + newPosition))).Distinct();

            IEnumerable<Bucket> exitBuckets = oldBuckets.Except(newBuckets);
            IEnumerable<Bucket> enterBuckets = newBuckets.Except(oldBuckets);
            
            foreach (Bucket bucket in exitBuckets) {
                bucket.RemovePawn(pawn);
            }

            foreach (Bucket bucket in enterBuckets) {
                bucket.AddPawn(pawn);
            }

            pawn.SetPositionInternal(newPosition);
        }
    }
}