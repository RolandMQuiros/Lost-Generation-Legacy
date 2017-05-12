using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LostGen;

/// <summary>
/// Generates GameObjects based on Pawn composition.
/// </summary>
public class PawnManager : MonoBehaviour {
    [TooltipAttribute("Temporary Prefab used to generate Pawns with Combatant components.")]
    public GameObject CombatantPrefab;
    private Dictionary<Pawn, GameObject> _pawnObjects = new Dictionary<Pawn, GameObject>();
    private MessageBuffer _messageBuffer = new MessageBuffer();
    public void OnPawnAdded(Pawn pawn) {
        if (!_pawnObjects.ContainsKey(pawn)) {
            Combatant combatant = pawn.GetComponent<Combatant>();
            if (combatant != null) {
                GameObject combatantObject = GameObject.Instantiate(CombatantPrefab, PointVector.ToVector(pawn.Position), Quaternion.identity);

                // Here, we need to pull Character information and apply it to the MonoBehaviours in combatantObject
                CombatantView view = combatantObject.GetComponent<CombatantView>();
                if (view == null) {
                    throw new Exception("Assigned Combatant Prefab does not have a CombatantView component");
                }
                view.Pawn = pawn;
                
                // Then, attach the MonoBehaviours to the MessageBuffer
                _pawnObjects.Add(pawn, combatantObject);
            }
        }
    }

    public void OnPawnRemoved(Pawn pawn) {
        GameObject pawnGO;
        if (_pawnObjects.TryGetValue(pawn, out pawnGO)) {
            _pawnObjects.Remove(pawn);
            // Add pawnGO to some pooling thing
        }
    }

    public void OnStep(Queue<IPawnMessage> messages) {
        _messageBuffer.PushMessages(messages);
    }
}
