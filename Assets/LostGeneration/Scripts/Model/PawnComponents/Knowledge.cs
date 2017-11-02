using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen.Component {
    public class Knowledge : PawnComponent {
        private Dictionary<Pawn, Point> _knownPawns = new Dictionary<Pawn, Point>();
        private HashSet<Pawn> _visiblePawns = new HashSet<Pawn>();
        private HashSet<Knowledge> _shared = new HashSet<Knowledge>();
        private Dictionary<string, DijkstraMap> _dijkstraMaps = new Dictionary<string, DijkstraMap>();
        
        /// <summary>
        /// Returns a collection of <see cref="Pawn"/>s this Pawn knows to exist. If Knowledge is shared with another
        /// Pawn, this returns the union all Pawns known. 
        /// </summary>
        /// <returns>List of known Pawns and their last known position</returns>
        public IEnumerable<KeyValuePair<Pawn, Point>> KnownPawns {
            get { return _knownPawns.Union(_shared.SelectMany(k => k._knownPawns)); }        
        }

        /// <summary>
        /// Returns a collection of <see cref="Pawn"/>s this Pawn can currently see. If Knowledge if shared with other
        /// Pawns, this returns the union of all seen Pawn lists. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pawn> VisiblePawns {
            get { return _visiblePawns.Union(_shared.SelectMany(k => k._visiblePawns)); }
        }
        
        public Dictionary<string, DijkstraMap> DijkstraMaps {
            get { return _dijkstraMaps; }
        }

        /// <summary>
        /// Adds a <see cref="Pawn"/> to the owner's vision.
        /// </summary>
        /// <remarks>
        /// Pawns within vision will continuously have their known positions updated.
        /// </remarks>
        /// <param name="pawn">Pawn being added to vision set</param>
        /// <returns><c>true</c> if addition was successful</returns>
        public bool AddVisiblePawn(Pawn pawn) {
            bool added = _visiblePawns.Add(pawn);
            if (added) {
                _knownPawns[pawn] = pawn.Position;
            }
            return added;
        }

        /// <summary>
        /// Removes a <see cref="Pawn"/> from vision.
        /// </summary>
        /// <remarks>
        /// A Pawn removed from vision is still maintained in a set of known pawns, but their location in the known set
        /// is not updated after every step. 
        /// </remarks>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public bool RemoveVisiblePawn(Pawn pawn) {
            bool removed = _visiblePawns.Remove(pawn);
            return removed;
        }

        #region PawnComponent
        public override void PostStep() {
            foreach (Pawn pawn in _visiblePawns) {
                _knownPawns[pawn] = pawn.Position;
            }
        }
        #endregion PawnComponent
    }
}