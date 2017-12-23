﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    /// <summary>
    /// Generates GameObjects based on Pawn composition.
    /// Currently everything's hardcoded. Make it dynamic one of these days.
    /// </summary>
    public class PawnViewManager : MonoBehaviour
    {
        [Serializable]
        public class PawnViewEvent : UnityEvent<PawnView> { }

        #region EditorFields
        [TooltipAttribute("Temporary Prefab used to generate Pawns with Combatant components.")]
        [SerializeField]private GameObject _combatantPrefab;
        
        [SerializeField]private PlayerTimelineController _timelines;
        public PawnViewEvent PawnViewAdded;
        public PawnViewEvent PawnViewRemoved;
        public UnityEvent MessagesFinished;

        #endregion EditorFields

        private Dictionary<Pawn, PawnView> _pawnViews = new Dictionary<Pawn, PawnView>();
        private MessageBuffer _messageBuffer = new MessageBuffer();
        private Queue<IPawnMessage> _messages = new Queue<IPawnMessage>();

        public void OnPawnAdded(Pawn pawn)
        {
            if (!_pawnViews.ContainsKey(pawn))
            {
                // TODO: Move this stuff to a combatant manager somehow
                Combatant combatant = pawn.GetComponent<Combatant>();
                if (combatant != null)
                {
                    GameObject combatantObject = GameObject.Instantiate(_combatantPrefab, PointVector.ToVector(pawn.Position), Quaternion.identity, transform);
                    combatantObject.name = pawn.Name;

                    PawnView pawnView = combatantObject.GetComponent<PawnView>();

                    // Here, we need to pull Character information and apply it to the MonoBehaviours in combatantObject
                    CombatantView combatantView = combatantObject.GetComponent<CombatantView>();
                    if (combatantView == null)
                    {
                        throw new Exception("Assigned Combatant Prefab does not have a CombatantView component");
                    }
                    combatantView.Pawn = pawn;

                    // Tie the CombatantActionView to the PlayerTimelines Controller
                    // Enemy Prefabs won't have this, so figure out a better construction scheme by the time you need
                    // to create them!
                    CombatantActionView actionView = combatantObject.GetComponent<CombatantActionView>();
                    if (actionView != null)
                    {
                        actionView.Pawn = pawn;
                        _timelines.AttachActionView(actionView);
                    }

                    // Then, attach the MonoBehaviours to the MessageBuffer
                    _pawnViews.Add(pawn, pawnView);
                }
            }
        }

        public void OnPawnRemoved(Pawn pawn)
        {
            PawnView pawnView;
            if (_pawnViews.TryGetValue(pawn, out pawnView))
            {
                _pawnViews.Remove(pawn);
                // Add pawnGO to some pooling thing

                CombatantActionView actionView = pawnView.GetComponent<CombatantActionView>();
                if (actionView != null)
                {
                    _timelines.DetachActionView(actionView);
                }
            }
        }

        public void OnStep(Queue<IPawnMessage> messages) {
            _messageBuffer.PushMessages(messages);
        }

        public void DistributeMessages()
        {
            if (_messageBuffer.HasMessages)
            {
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