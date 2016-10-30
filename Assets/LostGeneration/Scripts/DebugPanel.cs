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
                _combatant.Moved -= OnMove;
            }

            if (_combatant != value) {
                _combatant = value;
                _combatant.ActionAdded += OnActionAdded;
                _combatant.Moved += OnMove;
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

        text += _combatant.Position;
        text += "Action Points: " + _combatant.ActionPoints + "\n";

        Text.text = text;
    }

    private void OnMove(Pawn pawn, Point from, Point to) {
        string text = string.Empty;
        foreach (PawnAction acts in _combatant.Actions) {
            text += acts + "\n";
        }

        text += _combatant.Position;
        text += "Action Points: " + _combatant.ActionPoints + "\n";

        Text.text = text;
    }
}
