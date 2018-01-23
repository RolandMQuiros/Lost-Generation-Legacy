using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
    [RequireComponent(typeof(PawnViewManager))]
    public class PawnViewBuilder : MonoBehaviour {
        public virtual void Attach(GameObject target, Pawn pawn) {}
        public virtual void Detach(GameObject target, Pawn pawn) {}
    }

    /// <summary>
    /// Generates GameObjects based on Pawn composition.
    /// Currently everything's hardcoded. Make it dynamic one of these days.
    /// </summary>
    public class PawnViewManager : MonoBehaviour {
        [SerializeField]private PawnView _pawnViewPrefab;

        [Serializable]public class PawnViewEvent : UnityEvent<PawnView> { }
        public PawnViewEvent PawnViewAdded;
        public PawnViewEvent PawnViewRemoved;
        public UnityEvent MessagesFinished;

        private List<PawnViewBuilder> _builders;
        private Dictionary<Pawn, PawnView> _pawnViews = new Dictionary<Pawn, PawnView>();
        private MessageBuffer _messageBuffer = new MessageBuffer();
        private Queue<IPawnMessage> _messages = new Queue<IPawnMessage>();
                    
        public void OnPawnAdded(Pawn pawn) {
            if (!_pawnViews.ContainsKey(pawn)) {
                GameObject pawnObject = GameObject.Instantiate(
                    _pawnViewPrefab.gameObject, PointVector.ToVector(pawn.Position), Quaternion.identity, transform
                );
                PawnView pawnView = pawnObject.GetComponent<PawnView>();
                pawnObject.name = pawn.Name;
                _builders = _builders ?? new List<PawnViewBuilder>(GetComponents<PawnViewBuilder>());
                _builders.ForEach(b => b.Attach(pawnObject, pawn));
                
                foreach (PawnComponentView componentView in pawnObject.GetComponents<PawnComponentView>()) {
                    componentView.Pawn = pawn;
                }
                
                _pawnViews[pawn] = pawnView;
            }
        }

        public void OnPawnRemoved(Pawn pawn) {
            PawnView pawnView;
            if (_pawnViews.TryGetValue(pawn, out pawnView)) {
                _pawnViews.Remove(pawn);
                _builders.ForEach(b => b.Detach(pawnView.gameObject, pawn));
                PawnViewRemoved.Invoke(pawnView);
            }
        }

        public void OnStep(Queue<IPawnMessage> messages) {
            _messageBuffer.PushMessages(messages);
        }

        public void DistributeMessages() {
            if (_messageBuffer.HasMessages) {
                _messageBuffer.PopMessages(_messages);
                while (_messages.Count > 0) {
                    IPawnMessage message = _messages.Dequeue();
                    PawnView pawnView;
                    if (message.Source != null && _pawnViews.TryGetValue(message.Source, out pawnView)) {
                        pawnView.PushMessage(message);
                    }

                    if (message.Target != null && _pawnViews.TryGetValue(message.Target, out pawnView)) {
                        pawnView.PushMessage(message);
                    }
                }
            }
        }
        
        /// <summary>
        /// Runs every Pawn's HandleMessage coroutine simultaneously
        /// </summary>
        public void HandleMessages() {
            DistributeMessages();
            StartCoroutine(RunMessageCoroutines());
        }

        private IEnumerator RunMessageCoroutines() {
            yield return this.WaitForCoroutines(
                _pawnViews.Values.Select(v => v.ProcessMessages())
            );
            MessagesFinished.Invoke();
        }
    }
}