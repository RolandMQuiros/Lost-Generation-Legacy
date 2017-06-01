using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LostGen;

/// <summary>
/// Generates GameObjects based on Pawn composition.
/// </summary>
public class PawnManager : MonoBehaviour
{
    [TooltipAttribute("Temporary Prefab used to generate Pawns with Combatant components.")]
    public GameObject CombatantPrefab;
    private Dictionary<Pawn, PawnView> _pawnViews = new Dictionary<Pawn, PawnView>();
    private MessageBuffer _messageBuffer = new MessageBuffer();
    private Queue<IPawnMessage> _messages = new Queue<IPawnMessage>();

    public void OnPawnAdded(Pawn pawn)
    {
        if (!_pawnViews.ContainsKey(pawn))
        {
            Combatant combatant = pawn.GetComponent<Combatant>();
            if (combatant != null)
            {
                GameObject combatantObject = GameObject.Instantiate(CombatantPrefab, PointVector.ToVector(pawn.Position), Quaternion.identity, transform);
                combatantObject.name = pawn.Name;

                PawnView pawnView = combatantObject.GetComponent<PawnView>();

                // Here, we need to pull Character information and apply it to the MonoBehaviours in combatantObject
                CombatantView combatantView = combatantObject.GetComponent<CombatantView>();
                if (combatantView == null)
                {
                    throw new Exception("Assigned Combatant Prefab does not have a CombatantView component");
                }
                combatantView.Pawn = pawn;

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
        }
    }

    public void OnStep(Queue<IPawnMessage> messages)
    {
        _messageBuffer.PushMessages(messages);
    }

    public bool DistributeMessages()
    {
        if (_messageBuffer.HasMessages)
        {
            _messageBuffer.PopMessages(_messages);

            while (_messages.Count > 0)
            {
                IPawnMessage message = _messages.Dequeue();
                PawnView pawnView;
                if (message.Source != null && _pawnViews.TryGetValue(message.Source, out pawnView))
                {
                    pawnView.HandleMessage(message);
                }

                if (message.Target != null && _pawnViews.TryGetValue(message.Target, out pawnView))
                {
                    pawnView.HandleMessage(message);
                }
            }
        }

        return _messageBuffer.HasMessages;
    }
}
