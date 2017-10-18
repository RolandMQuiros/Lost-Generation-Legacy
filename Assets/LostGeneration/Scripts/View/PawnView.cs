using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

///<summary>
/// Receives IPawnMessages from the Model layer and distributes them to each PawnComponentView on the View layer
///</summary>
public class PawnView : MonoBehaviour {
    private List<PawnComponentView> _components;

    /// <summary>
    /// Propagates <see cref="IPawnMessage"/>s from the Model layer to each of the <see cref="PawnComponentView"/>s
    /// owned by the <see cref="GameObject"/>.
    /// </summary>
    /// <param name="message">Message to process</param>
    public void PushMessage(IPawnMessage message) {
        for (int i = 0; i < _components.Count; i++) {
            _components[i].PushMessage(message);
        }
    }
    
    /// <summary>
    /// Calls the ProcessMessages method on each of the <see cref="GameObject"/>'s <see cref="PawnComponentView"/>s.
    /// </summary>
    /// <returns>
    /// A <see cref="Coroutine"/> iterator that finishes when the ProcessMesages coroutines in all the
    /// PawnComponentViews finish</returns>
    public IEnumerator ProcessMessages() {
        yield return this.WaitForCoroutines(_components.Select(c => c.ProcessMessages()));
    }

    #region MonoBehaviour
    private void Start() {
        _components = new List<PawnComponentView>
        (
            GetComponents(typeof(PawnComponentView)).Cast<PawnComponentView>()
        );
    }
    #endregion MonoBehaviour
}
