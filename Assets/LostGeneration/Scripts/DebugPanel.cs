using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LostGen;

public class DebugPanel : MonoBehaviour {
    public Text Text;
    public Combatant Combatant {
        get { return _combatant; }
        set {
            if (_combatant != null) {
                _combatant.ActionAdded -= OnActionAdded;
            }

            if (_combatant != value) {
                _combatant = value;
                _combatant.ActionAdded += OnActionAdded;
                OnActionAdded(_combatant, null);
            }
        }
    }

    private Combatant _combatant;
	private void OnActionAdded(Pawn pawn, PawnAction action) {
        string text = string.Empty;
        foreach (PawnAction acts in _combatant.Actions) {
            text += acts + "\n";
        }
        Text.text = text;
    }
}
