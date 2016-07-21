using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Board {
        public class Entity {
            protected Board _board;
            protected List<Point> _footprint;
            public ReadOnlyCollection<Point> Footprint
            {
                get
                {
                    return _footprint.AsReadOnly();
                }
            }
            public Point Position;

            public bool IsSolid;

            public Entity(Board board, Point position, IEnumerable<Point> footprint = null, bool isSolid = false) {
                if (board == null) {
                    throw new ArgumentNullException("board");
                }

                if (footprint == null) {
                    _footprint = new List<Point>();
                } else {
                    _footprint = new List<Point>(footprint);
                }

                if (_footprint.Count == 0) {
                    _footprint.Add(Point.Zero);
                }

                _board = board;
                Position = position;
                IsSolid = isSolid;
            }

            public virtual void OnCollisionEnter(Entity other) { }
            public virtual void OnCollisionStay(Entity other) { }
            public virtual void OnCollisionExit(Entity other) { }
        }

        public int Width;
        public int Height;

        private Dictionary<Point, HashSet<Entity>> _entityBuckets = new Dictionary<Point, HashSet<Entity>>();
        private Dictionary<Entity, HashSet<Entity>> _collisions = new Dictionary<Entity, HashSet<Entity>>();

        public Board(int width, int height) {
            Width = width;
            Height = height;
        }

        public IEnumerable<Entity> CollisionsAt(Point point) {
            HashSet<Entity> entities;

            _entityBuckets.TryGetValue(point, out entities);

            return entities;
        }

        private HashSet<Entity> GetBucket(Point position) {
            HashSet<Entity> bucket;
            _entityBuckets.TryGetValue(position, out bucket);

            if (bucket == null) {
                bucket = new HashSet<Entity>();
                _entityBuckets.Add(position, bucket);
            }

            return bucket;
        }

        public bool AddEntity(Entity entity) {
            bool successful = true;

            if (entity != null && !_collisions.ContainsKey(entity)) {
                foreach (Point point in entity.Footprint) {
                    HashSet<Entity> bucket = GetBucket(entity.Position + point);
                    successful &= bucket.Add(entity);
                }
                _collisions.Add(entity, new HashSet<Entity>());
            }

            return successful;
        }

        public Point SetEntityPosition(Entity entity, Point newPosition) {
            List<HashSet<Entity>> toRemove = new List<HashSet<Entity>>();
            List<HashSet<Entity>> toAdd = new List<HashSet<Entity>>();

            HashSet<Entity> enterCollisions = new HashSet<Entity>();
            HashSet<Entity> exitCollisions = new HashSet<Entity>();

            Point point;

            // Gather all entities that were in the original and new position's footprint
            foreach (Point offset in entity.Footprint) {
                HashSet<Entity> oldBucket;
                HashSet<Entity> newBucket;

                point = entity.Position + offset;

                // Add all entities in the current bucket to the exit collision set
                _entityBuckets.TryGetValue(point, out oldBucket);
                if (oldBucket != null && oldBucket.Contains(entity)) {
                    exitCollisions.UnionWith(oldBucket);
                    toRemove.Add(oldBucket);
                }

                // Add all entities in the new position to the enter set
                newBucket = GetBucket(newPosition + entity.Position);
                enterCollisions.UnionWith(newBucket);
                toAdd.Add(newBucket);
            }

            // If the entity is solid, and any of the entering collision entities are solid, don't allow the move
            bool willMove = true;
            if (entity.IsSolid) {
                foreach (Entity other in enterCollisions) {
                    if (other.IsSolid) {
                        willMove = false;
                        break;
                    }
                }
            }

            // Move the entity between buckets
            if (willMove) {
                for (int i = 0; i < toRemove.Count; i++) {
                    toRemove[i].Remove(entity);
                }

                for (int i = 0; i < toAdd.Count; i++) {
                    toAdd[i].Remove(entity);
                }

                entity.Position = newPosition;
            }

            // The stay set consists of entities that are in both the exit and enter sets
            HashSet<Entity> stayCollisions = new HashSet<Entity>(exitCollisions);
            stayCollisions.IntersectWith(enterCollisions);

            // Remove the stay collisions from both the exit and enter sets
            enterCollisions.ExceptWith(stayCollisions);
            exitCollisions.ExceptWith(stayCollisions);

            // Call the collision methods
            foreach (Entity other in enterCollisions) {
                entity.OnCollisionEnter(other);
                other.OnCollisionEnter(entity);
            }

            foreach (Entity other in stayCollisions) {
                entity.OnCollisionStay(other);
                other.OnCollisionStay(entity);
            }

            foreach (Entity other in exitCollisions) {
                entity.OnCollisionExit(other);
                other.OnCollisionExit(entity);
            }
        }
    }
}