using System;
using UnityEngine;
using System.Collections.Generic;
using LostGen;

public class PawnViewManager : MonoBehaviour {
    public Board Board;

    private Dictionary<Combatant, CombatantView> _combatants = new Dictionary<Combatant, CombatantView>();
    
	public void Start () {
        if (Board == null) {
            throw new NullReferenceException("The PawnViewManager has no active Board reference");
        }

        Board.PawnAdded += OnPawnAdded;
        Board.PawnRemoved += OnPawnRemoved;
	}
	
	private void OnPawnAdded(Pawn pawn) {
        if (pawn is Combatant) {
            // CombatantViewFactory.create(pawn.Name);
            CombatantView view = null;
            _combatants.Add(pawn as Combatant, view);
        }
    }

    private void OnPawnRemoved(Pawn pawn) {
        if (pawn is Combatant) {
            _combatants.Remove(pawn as Combatant);
        }
    }
}
