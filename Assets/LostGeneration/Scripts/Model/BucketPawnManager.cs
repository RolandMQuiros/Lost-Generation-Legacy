using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class BucketPawnManager : IPawnManager {
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
        private Point _bucketSize;

        public Point BucketSize { get { return _bucketSize; } }
        public event Action<Pawn> PawnAdded;
        public event Action<Pawn> PawnRemoved;
        public IEnumerable<Pawn> Ordered {
            get { return _pawns.OrderBy(pawn => pawn.Priority); }
        }

        public BucketPawnManager(Point boardSize, Point bucketSize) {
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
            _bucketSize = boardSize / subdivisions;

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
                        }
                    }
                }
            }

            return addedToSet && addedToBucket;
        }
        
        public bool Remove(Pawn pawn) {
            bool removedFromSet = _pawns.Remove(pawn);
            bool removedFromBucket = false;

            if (removedFromSet) {
                for (int b = 0; b < _buckets.Count; b++) {
                    Bucket bucket = _buckets[b];
                    removedFromBucket |= bucket.RemovePawn(pawn);
                }
            }

            return removedFromBucket;
        }
    }
}