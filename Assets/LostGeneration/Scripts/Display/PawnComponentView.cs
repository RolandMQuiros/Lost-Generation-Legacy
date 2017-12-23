using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;

namespace LostGen.Display {
    /// <summary>
    /// Holds the logic for processing <see cref="IPawnMessage"/>s from the Model layer. PawnComponentViews are typically
    /// directly mapped to a <see cref="PawnComponent"/>, hence the name, but not always.
    /// </summary>
    public abstract class PawnComponentView : MonoBehaviour {
        /// <summary>The <see cref="Pawn"/> this PawnComponentView is attached to</summary>
        public Pawn Pawn;
        /// <summary>
        /// Adds the given <see cref="IPawnMessage"/> to a collection for later processing by <see cref="ProcessMessages"/>.
        /// </summary>
        /// <param name="message">Message to push</param>
        public abstract void PushMessage(IPawnMessage message);
        /// <summary>
        /// Creates a <see cref="Coroutine"/> iterator from the <see cref="IPawnMessage"/>s buffered through
        /// <see cref="PushMessage"/>
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator ProcessMessages();
    }
}