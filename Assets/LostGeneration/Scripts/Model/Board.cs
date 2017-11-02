using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LostGen {
    public class Board {
        public IBlockManager Blocks { get { return _blocks; } }
        public IPawnManager Pawns { get { return _pawns; } }

        private IBlockManager _blocks;
        private IPawnManager _pawns;

        public Board(IBlockManager blockManager, IPawnManager pawnManager) {
            _blocks = blockManager;
            _pawns = pawnManager;
        }

        /// <summary>
        /// Checks if vision can pass through a Point in the board.  A point is considered opaque if the BoardBlock or any Pawn at that point is opaque.
        /// If the given point is outside the bounds of the Board, returns true.
        /// <param name="point">Point on the board to check</param>
        /// <returns>True if light cannot pass through this point on the Board.  False, otherwise or if point is outside the Board.</returns>
        public bool IsOpaque(Point point) {
            bool isOpaque = true;
            
            if (_blocks.InBounds(point)) {
                isOpaque = _blocks.At(point).IsOpaque ||
                           _pawns.At(point).Where(p => p.IsOpaque).Any();
            }

            return isOpaque;
        }

        public bool IsSolid(Point point) {
            bool isSolid = true;
            if (_blocks.InBounds(point)) {
                isSolid = _blocks.At(point).IsSolid ||
                          _pawns.At(point).Where(p => p.IsSolid).Any();
            }

            return isSolid;
        }

        public bool SetPawnPosition(Pawn pawn, Point newPosition) {
            if (!_pawns.Contains(pawn)) {
                throw new ArgumentException("Pawn " + pawn.Name + " has not been added to this Board", "pawn");
            }

            // If the pawn is solid, and there are any solid blocks in the new position that overlap with the footprint, early out
            IEnumerable<Point> newFootprint = pawn.Footprint.Select(f => f + newPosition);
            if (pawn.IsSolid && _blocks.At(newFootprint)
                                       .Where(b => b.IsSolid)
                                       .Any()) {
                return false;
            }
            IEnumerable<Point> oldFootprint = pawn.Footprint.Select(f => f + pawn.Position);

            // Gather collisions at the old and new positions
            IEnumerable<Pawn> oldCollisions = _pawns.At(oldFootprint)
                                                    .Where(p => p != pawn && p.IsCollidable);
            IEnumerable<Pawn> newCollisions = _pawns.At(newFootprint)
                                                    .Where(p => p != pawn && p.IsCollidable);
            
            // The move can only happen if the pawn is non-solid, or the new position is free from solids
            bool canMove = !pawn.IsSolid || !newCollisions.Where(p => p.IsSolid).Any();
            if (canMove) {
                _pawns.Move(pawn, newPosition);
            }

            // Call pawn's collision handlers
            if (pawn.IsCollidable) {
                // Collisions at the new position that were not at the old position are newly-entered
                IEnumerable<Pawn> enterCollisions = newCollisions.Except(oldCollisions);
                // Collisions at both the old and new position are sustained
                IEnumerable<Pawn> stayCollisions = oldCollisions.Intersect(newCollisions);
                // Collisions at the old position but not at the new are exited
                IEnumerable<Pawn> exitCollisions = oldCollisions.Except(newCollisions);

                // Run collision functions
                foreach (Pawn other in enterCollisions) {
                    pawn.OnCollisionEnter(other);
                    other.OnCollisionEnter(pawn);
                }
                foreach (Pawn other in stayCollisions) {
                    pawn.OnCollisionStay(other);
                    other.OnCollisionStay(pawn);
                }
                foreach (Pawn other in exitCollisions) {
                    pawn.OnCollisionExit(other);
                    other.OnCollisionExit(pawn);
                }
            }

            return canMove;
        }

        public void BeginTurn() {
            foreach (Pawn pawn in _pawns.Ordered) {
                pawn.BeginTurn();
            }
        }

        /// <summary>
        /// Calls the Step() function of each Pawn on the Board.
        /// </summary>
        /// <returns>true if a Pawn still has actions to perform in this step, false otherwise</returns>
        public Queue<PawnAction> Step(Queue<IPawnMessage> messages) {
            Queue<PawnAction> actions = new Queue<PawnAction>();
            foreach (Pawn pawn in _pawns.Ordered) {
                PawnAction action = pawn.Step(messages);
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

        #region SelectAlgs
        
        public bool LineCast(Point start, Point end, HashSet<Pawn> pawns = null, bool passThroughWalls = false, bool passThroughSolids = false) {
            bool stopped = false;

            foreach (Point point in Point.Line(start, end)) {
                if (_blocks.InBounds(point)) {
                    if (!passThroughWalls && _blocks.At(point).IsSolid) {
                        stopped = true;
                    } else {
                        IEnumerable<Pawn> pawnsAt = _pawns.At(point);
                        if (pawns != null) {
                            pawns.UnionWith(pawnsAt);
                        }
                        stopped = passThroughSolids && pawnsAt.Where(pawn => pawn.IsCollidable && pawn.IsSolid).Any();
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