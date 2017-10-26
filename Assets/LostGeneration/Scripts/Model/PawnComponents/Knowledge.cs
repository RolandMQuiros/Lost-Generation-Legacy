using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen.Component {
    public class Knowledge : PawnComponent {
        private HashSet<Pawn> _knownPawns = new HashSet<Pawn>();
        private HashSet<Pawn> _visiblePawns = new HashSet<Pawn>();
        private HashSet<Knowledge> _shared = new HashSet<Knowledge>();
        
        /// <summary>
        /// Returns a collection of <see cref="Pawn"/>s this Pawn knows to exist. If Knowledge is shared with another
        /// Pawn, this returns the union all Pawns known. 
        /// </summary>
        /// <returns>List of known Pawns</returns>
        public IEnumerable<Pawn> GetKnownPawns() {
            return _knownPawns.Union(_shared.SelectMany(k => k._knownPawns));        
        }

        /// <summary>
        /// Returns a collection of <see cref="Pawn"/>s this Pawn can currently see. If Knowledge if shared with other
        /// Pawns, this returns the union of all seen Pawn lists. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pawn> GetVisiblePawns() {
            return _visiblePawns.Union(_shared.SelectMany(k => k._visiblePawns));
        }

        
    }
}